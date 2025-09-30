using CsvHelper;
using CsvHelper.Configuration;
using EGM_Analyzer.DataPrepare;
using EGM_Analyzer.Save;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace EGM_Analyzer.DatasetBuilder.DataBase
{
    public class RawRecord
    {
        public double[] Signal { get; set; }
        public byte[] Markup { get; set; }
        public ushort Channel { get; set; }
        public ushort BaseFramerate { get; set; }
        public bool IsValidRecord { get; set; } = true;
    }
    public class DatabaseBuilder
    {
        private string _savePath = "";

        private string _dataPath = "";

        private StreamWriter _writer;

        private CsvWriter _csvWriter;

        private List<PreparingRecord> _preparingRecords;

        public bool NormalizeData { get; private set; }
        public uint PartDuration { get; private set; }
        public ushort OverlapDuration { get; private set; }

        public DatabaseBuilder(string dataPath, string savePath, bool normalizeData, 
            uint partDuration, ushort overlapDuration)
        {
            NormalizeData = normalizeData;
            PartDuration = partDuration;
            OverlapDuration = overlapDuration;

            _dataPath = dataPath;
            _savePath = savePath;

            string basename = DateTime.Now.ToString("dd-MM-yyyy_hh-mm-ss") + ".csv";
            _writer = new StreamWriter(_savePath + $"\\{basename}", false, Encoding.UTF8);

            var config = new CsvConfiguration(CultureInfo.CurrentCulture) 
            {   
                Delimiter = ";", 
                Encoding = Encoding.ASCII 
            };

            _csvWriter = new CsvWriter(_writer, config);


            _csvWriter.WriteHeader<DatabaseRecord>();
            _csvWriter.NextRecord();
            _csvWriter.Flush();
        }

        public Task DataToBase()
        {
            return Task.Run(async () =>
            {
                // Запуск цикла по всем записям и разметкам
                foreach (var preparingRecord in _preparingRecords)
                {
                    JSONSegments? markupsData = null;
                    try
                    {
                        using (FileStream fs = new FileStream(preparingRecord.MarkupsPaths[0], FileMode.Open))
                        {
                            markupsData = await JsonSerializer.DeserializeAsync<JSONSegments>(fs);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Не удаётся открыть файл разметки для получения частоты семплирования сигнала!",
                                        "Ошибка формата сегментации!",
                                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    //Заполнение dataloader'а сигналом и разметкой
                    DataLoader.DataLoader dataLoader = new DataLoader.DataLoader(preparingRecord.SignalPath,
                        markupsData.SampleRate);
                    await dataLoader.LoadSignal();
                    foreach (var markup in preparingRecord.MarkupsPaths)
                    {
                        await dataLoader.LoadMarkups(markup);
                    }

                    RecordPreparer currentRecord = new RecordPreparer(dataLoader, 
                        preparingRecord.SignalPath, preparingRecord.SignalName());
                    if (markupsData.SampleRate == 0)
                        currentRecord.InitMetadates(5000, 5000,
                        NormalizeData, PartDuration, OverlapDuration);
                    else
                        currentRecord.InitMetadates(markupsData.SampleRate, markupsData.SampleRate,
                            NormalizeData, PartDuration, OverlapDuration);

                    RawRecord? curRecord = currentRecord.GetRecord();
                    while (curRecord != null)
                    {
                        if (curRecord.IsValidRecord)
                            AppendRecord(currentRecord.SignalName, curRecord.Signal, curRecord.Markup, curRecord.Channel, curRecord.BaseFramerate);
                        curRecord = currentRecord.GetRecord();
                        if (curRecord.Channel > 0)
                            curRecord = curRecord;
                    }
                    int s = 0;
                    curRecord = null;
                }

            });
        }

        public void PrepareSignalMarkupsList()
        {
            _preparingRecords = GetPreparingRecords(_dataPath);//new List<PreparingRecord>();

/*            var signalsPaths = FilesByExtension(_dataPath, "npy");
            var markupsPaths = FilesByExtension(_dataPath, "json");

            for (int i = 0; i < signalsPaths.Count; i++)
            {
                FileInfo fileInfo = new FileInfo(signalsPaths[i]);
                string signalName = (fileInfo.Name.Replace(fileInfo.Extension, ""));

                List<string> markupPaths = markupsPaths.FindAll(x => x.Contains(signalName));
                if (markupPaths.Count == 0) { continue; }

                _preparingRecords.Add(new PreparingRecord(signalsPaths[i], markupPaths));
            }*/
        }

        public static List<PreparingRecord> GetPreparingRecords(string folderPath)
        {
            List<PreparingRecord> result = new List<PreparingRecord>();
            var signalsPaths = FilesByExtension(folderPath, "npy");
            var markupsPaths = FilesByExtension(folderPath, "json");

            for (int i = 0; i < signalsPaths.Count; i++)
            {
                FileInfo fileInfo = new FileInfo(signalsPaths[i]);
                string signalName = (fileInfo.Name.Replace(fileInfo.Extension, ""));
                List<string> markupPaths = markupsPaths.FindAll(x => x.Contains(signalName));
                if (markupPaths.Count == 0) { continue; }
                result.Add(new PreparingRecord(signalsPaths[i], markupPaths));
            }
            return result;
        }

        private static List<string> FilesByExtension(string folderPath, string extension)
        {
            List<string> paths = new List<string>();
            paths.AddRange(Directory.EnumerateFiles(folderPath, $"*.{extension}", SearchOption.TopDirectoryOnly));
            return paths;
        }


        private void AppendRecord(DatabaseRecord databaseRecord)
        {
            _csvWriter.WriteRecord(databaseRecord);
            _csvWriter.NextRecord();
            _csvWriter.Flush();
        }

        private void AppendRecord(string filename, double[] signal, byte[] markup, ushort channel, ushort baseFramerate)
        {
            string strSignal = "";
            string strMarkup = "";
            for (int i=0; i < signal.Length; i++)
            {
                strSignal += ((int)signal[i]).ToString();
                strMarkup += markup[i].ToString();
                if (i + 1 < signal.Length)
                {
                    strSignal += ", ";
                    strMarkup += ", ";
                }
            }

            DatabaseRecord databaseRecord = new DatabaseRecord()
            {
                FileName = filename,
                Channel = channel,
                Framerate = baseFramerate,
                Markup = strMarkup,
                Signal = strSignal
            };
            _csvWriter.WriteRecord(databaseRecord);
            _csvWriter.NextRecord();
            _csvWriter.Flush();
        }




    }
}
