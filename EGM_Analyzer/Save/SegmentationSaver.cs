using EGM_Analyzer.Segmentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Xml.Linq;

namespace EGM_Analyzer.Save
{
    public class JSONSegments
    {
        /// <summary>
        /// Имя сигнала, для которого проводилась разметка
        /// </summary>
        public string SignalName { get; set; }
        /// <summary>
        /// Частота сигнала
        /// </summary>
        public ushort SampleRate { get; set; }
        /// <summary>
        /// Размер сигнала в байтах для идентификации
        /// </summary>
        public long SignalFileSize { get; set; }
        /// <summary>
        /// Имя используемой модели для разметки. Формат: навзание - дата обучения модели
        /// </summary>
        public string? UsedModel { get; set; }
        /// <summary>
        /// Список сегментов, разделённых по каналам
        /// </summary>
        public List<List<Segment>> Segments { get; set; }

        public JSONSegments(string signalName, ushort sampleRate, long signalFileSize, string? usedModel, List<List<Segment>> segments)
        {
            SignalName = signalName;
            SignalFileSize = signalFileSize;
            UsedModel = usedModel;
            Segments = segments;
            SampleRate = sampleRate;
        }
    }

    public class SegmentationSaver
    {

        private const string TemporalSaveFolder = "AutomaticSave";
        private string _temporalPath;
        private byte _saveFilesCount = 3;

        public string? FileName { get; private set; }

        private Queue<string> _pathSavedFiles;

        public string SignalFilename { get; private set; }
        public long SignalFileSize { get; private set; }
        public string? UsedModel { get; private set; }

        public ushort SampleRate { get; set; } = 1;

        public SegmentationSaver(FileInfo signal, ushort sampleRate = 0, string? modelName = null)
        {
            SampleRate = sampleRate;
            if (signal.Extension == "")
            {
                MessageBox.Show("Сигнал без расширения не обрабатывается!", "Ошибка! ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw new Exception("Сигнал без расширения не обрабатывается!");
            }
            FileName = signal.Name.Replace(signal.Extension, "");

            SignalFilename = signal.Name;
            SignalFileSize = signal.Length;
            UsedModel = modelName;
            _temporalPath = Path.Combine(TemporalSaveFolder, FileName);
            if (!Directory.Exists(_temporalPath))
            {
                Directory.CreateDirectory(_temporalPath);
            }
            _pathSavedFiles = new Queue<string>();
        }



        /// <summary>
        /// Метод, устанавливающий число автоматически сохранённых файлов в папке. Старые файлы перезаписываются
        /// </summary>
        /// <param name="count">Число автоматически сохранённых файлов в папке</param>
        public void SetAutomaticSaveFilesCount(byte count)
        {
            _saveFilesCount = count;
        }

        /// <summary>
        /// Сохранение файла разметки по конкретному адресу
        /// </summary>
        /// <param name="filepath">Полный путь до места сохранения</param>
        public async void Save(List<List<Segment>> segments, string filepath)
        {

            if (SampleRate == 0)
            {
                MessageBox.Show("Не установлена частота сигнала!", "Ошибка! ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            try
            {
                JSONSegments incrementedMarkup = new JSONSegments(SignalFilename, SampleRate, SignalFileSize, UsedModel, segments);
                using (FileStream fs = new FileStream(filepath, FileMode.Create))
                {
                    await JsonSerializer.SerializeAsync<JSONSegments>(fs, incrementedMarkup);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка сохранения разметки!", ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }


        public async void AutomaticSave(List<List<Segment>> segments)
        {
            if (SampleRate == 0)
            {
                MessageBox.Show("Не установлена частота сигнала!", "Ошибка! ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DateTime now = DateTime.Now;
            string newFileName = Path.Combine(_temporalPath, $"{SignalFilename} {now.Day}-{now.Month}-{now.Year} {now.Hour}-{now.Minute}-{now.Second}.json");

            if (_pathSavedFiles.Count >= 3)
            {
                try
                {
                    File.Delete(_pathSavedFiles.Dequeue());
                } 
                catch(Exception ex) 
                {
                    MessageBox.Show("Ошибка удаления временного хранилища разметки!", ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }



            try
            {
                JSONSegments incrementedMarkup = new JSONSegments(SignalFilename, SampleRate, SignalFileSize, UsedModel, segments);
                using (FileStream fs = new FileStream(newFileName, FileMode.Create))
                {
                    await JsonSerializer.SerializeAsync<JSONSegments>(fs, incrementedMarkup);
                }
                _pathSavedFiles.Enqueue(newFileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка сохранения разметки!", ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }








    }
}
