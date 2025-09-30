using EGM_Analyzer.DataLoader;
using EGM_Analyzer.DataPrepare;
using EGM_Analyzer.EventsArgs;
using EGM_Analyzer.Save;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace EGM_Analyzer.DatasetBuilder
{
    public class DatasetBuilder
    {
        private const string DATASET_DIR = "Dataset ";
        public short TargetFrameRate { get; private set; }
        public bool NormalizeData { get; private set; }
        public bool UnionFrameRate { get; private set; }
        public uint PartDuration { get; private set; }
        public ushort OverlapDuration { get; private set; }
        private List<PreparingRecord> _preparingRecords;

        public EventHandler<RecordsProcessedEventArgs> RecordsProcessUpdate;

        private void OnRecordsProcessUpdate(RecordsProcessedEventArgs e)
        {
            EventHandler<RecordsProcessedEventArgs> handler = RecordsProcessUpdate;
            handler?.Invoke(this, e);
        }

        public DatasetBuilder(List<PreparingRecord> preparingRecords, uint partDuration, 
            ushort overlapDuration, bool normalizeData, short frameRate = -1)
        {
            _preparingRecords = preparingRecords;
            TargetFrameRate = frameRate;
            PartDuration = partDuration;
            OverlapDuration = overlapDuration;
            NormalizeData = normalizeData;
        }


        public ushort NumRecords { get; private set; }
        public ushort CurRecordNumber { get; private set; }
        public string CurSignalName { get; private set; }




        public Task PrepareDataset()
        {
            return Task.Run(async () =>
            {

                string directoryName = Path.Combine(System.IO.Directory.GetCurrentDirectory(),
                DATASET_DIR + DateTime.Now.ToString("dd-MM-yyyy"));
                DirectoryInfo directoryInfo = Directory.CreateDirectory(directoryName);
                if (directoryInfo.Exists)
                {
                    NumRecords = (ushort)_preparingRecords.Count;
                    CurRecordNumber = 0;
                    // Запуск цикла по всем записям и разметкам
                    foreach (var preparingRecord in _preparingRecords)
                    {
                        CurRecordNumber++;
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
                        FileInfo fileInfo = new FileInfo(preparingRecord.SignalPath);
                        CurSignalName = fileInfo.Name;

                        //Заполнение dataloader'а сигналом и разметкой
                        DataLoader.DataLoader dataLoader = new DataLoader.DataLoader(preparingRecord.SignalPath, 
                            markupsData.SampleRate);
                        await dataLoader.LoadSignal();
                        foreach (var markup in preparingRecord.MarkupsPaths)
                        {
                            await dataLoader.LoadMarkups(markup);
                        }

                        if (markupsData.SampleRate == 0)
                            markupsData.SampleRate = 5000;

                        //
                        RecordPreparer currentRecord = new RecordPreparer(dataLoader, directoryName, preparingRecord.SignalName());
                        currentRecord.RecordPartHandled += PartHandled;
                        currentRecord.PrepareData(markupsData.SampleRate, TargetFrameRate, NormalizeData, PartDuration, OverlapDuration);
                        currentRecord.RecordPartHandled -= PartHandled;


                    }
                    //DataLoader.DataLoader dataLoader = new DataLoader.DataLoader()
                }
            });
        }

        private void PartHandled(object? sender, RecordPartInfoEventArgs e)
        {
            OnRecordsProcessUpdate(new RecordsProcessedEventArgs()
            {
                CurrentRecordName = CurSignalName,
                NumRecords = NumRecords,
                CurrentRecordsNumber = CurRecordNumber,
                CurrentPartNumber = e.PartNumber,
                NumParts = e.PartCount,
                ChannelNumber = e.ChannelNumber,
            });
        }
    }
}
