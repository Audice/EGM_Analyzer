using EGM_Analyzer.Analyzers;
using EGM_Analyzer.DataLoader.OldMurkup;
using EGM_Analyzer.EventsArgs;
using EGM_Analyzer.Save;
using EGM_Analyzer.Segmentation;
using NumSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace EGM_Analyzer.DataLoader
{
    public class DataLoader : IDataLoader
    {
        private BinaryReader? _binarySignalStream = null;
        public ReadOnlyCollection<ReadOnlyCollection<double>> Data
        {
            get; private set;
        }

        public ushort SampleRate 
        { 
            get; private set;
        }

        public SegmentsHandler? SegmentsHandler
        {
            get; private set;
        }

        public string SignalFilepath { get; private set; }

        public LoadSignalState LoadSignalState
        {
            get; private set;
        }

        private ulong _targetPosition = 0;
        public ulong TargetPosition
        {
            get
            {
                return _targetPosition;
            }
            set
            {
                _targetPosition = value;
                CurrentSegmentChanged?.Invoke(this, new SegmentEventArgs() 
                { 
                    DataMatrix = GetSamples(_targetPosition)
                });
            }
        }



        public event EventHandler<LoadingStateEventArgs> LoadStateChanged;

        public event EventHandler<SegmentEventArgs> CurrentSegmentChanged;

        public DataLoader(string signalPath, ushort sampleRate)
        {
            SignalFilepath = signalPath;
            LoadSignalState = DetermineLoadSignalState(SignalFilepath);
            SampleRate = sampleRate;
        }

        public LoadSignalState DetermineLoadSignalState(string signalFilepath)
        {
            FileInfo fileInfo = new FileInfo(signalFilepath);
            switch (fileInfo.Extension)
            {
                case ".npy":
                    return LoadSignalState.Full;
                case ".bin":
                    return LoadSignalState.Part;
                default:
                    throw new ArgumentException("DetermineLoadSignalState: Неопределённый формат сигнала!");
            }
        }

        private void OnLoadStateChange(LoadingStateEventArgs e)
        {
            LoadStateChanged?.Invoke(this, e);
        }

        public Task LoadMarkups(string filepath)
        {
            return Task.Run(async () =>
            {
                // Реализуем загрузку для нескольких форматов
                // Попытка загрузить разметку в новом формате
                var markupsData = await _3th_part.LoadNewFormatMarkup(filepath, SampleRate);
                if (markupsData == null)
                {
                    // В случае неудачи попытаемся загрузить в старом формате
                    markupsData = await _3th_part.LoadOldFormatMarkup(filepath, SampleRate);
                }
                if (markupsData == null)
                {
                    return;
                }
                if (SegmentsHandler != null)
                {
                    if (markupsData.Count != SegmentsHandler.ChannelsCount)
                    {
                        MessageBox.Show("Число каналов в сигнале не совпадает с числом каналов в разметке!", 
                            "Ошибка загрузки сегментации!", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                else
                {
                    SegmentsHandler = new SegmentsHandler((byte)markupsData.Count, new FileInfo(SignalFilepath), SampleRate);
                }
                SegmentsHandler.AppendSegments(markupsData);
            });
        }

        public Task LoadMarkups(string filepath, byte startChannel, byte endChannel)
        {
            return Task.Run(async () =>
            {
                // Реализуем загрузку для нескольких форматов
                // Попытка загрузить разметку в новом формате
                var markupsData = await _3th_part.LoadNewFormatMarkup(filepath, SampleRate);
                if (markupsData == null)
                {
                    // В случае неудачи попытаемся загрузить в старом формате
                    markupsData = await _3th_part.LoadOldFormatMarkup(filepath, SampleRate);
                }
                if (markupsData == null)
                {
                    return;
                }
                if (SegmentsHandler != null)
                {
                    if (markupsData.Count != SegmentsHandler.ChannelsCount)
                    {
                        MessageBox.Show("Число каналов в сигнале не совпадает с числом каналов в разметке!",
                            "Ошибка загрузки сегментации!",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                else
                {
                    SegmentsHandler = new SegmentsHandler(
                        (byte)markupsData.Count, 
                        new FileInfo(SignalFilepath), 
                        SampleRate);
                }
                SegmentsHandler.AppendSegments(markupsData, startChannel, endChannel);
            });
        }

        public Task LoadSignal()
        {
            return Task.Run(() =>
            {
                //Загрузка сигнала возможно только для numpy массивов
                if (LoadSignalState != LoadSignalState.Full)
                    return;

                var fileInfo = new FileInfo(SignalFilepath);
                // Библиотека не может читать большие файлы
                if (fileInfo.Length > 2_147_483_648)
                    throw new ArgumentException("Файл превышает размерв 2GB. Из-за особенности библиотеки NumSharp прочитать файл такого размера не получится. Пожалуйста, разделите его на части использую Python.");

                double[,]? data = np.Load<double[,]>(SignalFilepath);
                if (data == null)
                    throw new Exception("LoadSignal: чтение файла с сигналом не удалось.");
                //Заполнение коллекции каналов сигнала
                List<ReadOnlyCollection<double>> tmpList = new List<ReadOnlyCollection<double>>();
                for (int i = 0; i < data.GetLength(0); i++)
                {
                    tmpList.Add(new ReadOnlyCollection<double>(Enumerable.Range(0, data.GetLength(1)).Select(x => data[i, x]).ToArray()));
                }
                Data = new ReadOnlyCollection<ReadOnlyCollection<double>>(tmpList);
                data = null;
                //Создание обработчика сегментов
                SegmentsHandler = new SegmentsHandler((byte)Data.Count, new FileInfo(SignalFilepath), SampleRate);
            });
        }

        public double[] LoadSignalPart(byte leadNumber, long startIndex, int count)
        {
            if (_binarySignalStream != null)
            {
                var fileInfo = new FileInfo(SignalFilepath);
                if (fileInfo.Length % leadNumber != 0)
                    throw new ArgumentException("Бинарный файл имеет некорректный формат");
                long byteSamplePerLead = fileInfo.Length / leadNumber;
                //Расчитываем стартовый байт. Умножение на 8 = размер double в байтах
                long startByte = leadNumber * byteSamplePerLead + startIndex * 8;
                _binarySignalStream.BaseStream.Seek(startByte, SeekOrigin.Begin);

                byte[] buffer = new byte[count];
                int bufSize = _binarySignalStream.Read(buffer, 0, count);


                double[] values = new double[bufSize / 8];
                Buffer.BlockCopy(buffer, 0, values, 0, bufSize);

                return values;
            }
            else
            {
                var targetLead = Data[leadNumber];
                return targetLead.ToList().GetRange((int)startIndex, count).ToArray();
            }
        }

        public Task ResaveSignal(string filepath)
        {
            //OnLoadStateChange(new LoadingStateEventArgs() { State = LoadingState.StartLoading });
            return Task.Run(() =>
            {
                if (LoadSignalState == LoadSignalState.Full)
                {
                    FileInfo fileInfo = new FileInfo(filepath);
                    switch (fileInfo.Extension)
                    {
                        case ".npy":
                            MessageBox.Show("Сигнал был загружен как Numpy массив. В сохранении отказано",
                                            "Ошибка сохранения файла!",
                                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                            break;
                        case ".bin":
                            using (BinaryWriter writer = new BinaryWriter(File.Open(filepath, FileMode.CreateNew)))
                            {
                                foreach (var channel in Data)
                                {
                                    double[] sourceChannel = channel.ToArray();
                                    byte[] buffer = new byte[sourceChannel.Length * 8];
                                    Buffer.BlockCopy(sourceChannel, 0, buffer, 0, buffer.Length);
                                    writer.Write(buffer);
                                }
                            }
                            break;
                        default:
                            MessageBox.Show("Некорректный формат хранения сигнала.",
                                "Ошибка сохранения файла!",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            break;
                    }
                }
                else
                {
                    MessageBox.Show("В настоящий момент сохранение частично загруженных файлов не поддерживается.",
                                    "Ошибка сохранения файла!",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            });
        }

        public Task AppendSegments(string filepath)
        {
            return Task.Run(async () =>
            {
                if (SegmentsHandler == null)
                {
                    MessageBox.Show("Отсутствует исходная сегментация. Загрузите сначала её.", "Ошибка сегментации!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                var markupsData = await _3th_part.LoadNewFormatMarkup(filepath, SampleRate);
                if (markupsData == null)
                {
                    // В случае неудачи попытаемся загрузить в старом формате
                    markupsData = await _3th_part.LoadOldFormatMarkup(filepath, SampleRate);
                }
                if (markupsData == null)
                {
                    return;
                }
                if (markupsData.Count != SegmentsHandler.ChannelsCount)
                {
                    MessageBox.Show("Число каналов в сигнале не совпадает с числом каналов в разметке!",
                        "Ошибка загрузки сегментации!",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                SegmentsHandler.AppendSegments(markupsData);
            });
        }

        public Task LoadXLSXMarkups(string filepath)
        {
            return Task.Run(async () =>
            {
                if (SegmentsHandler == null)
                {
                    MessageBox.Show("Отсутствует исходная сегментация. Загрузите сначала её.", "Ошибка сегментации!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                var markupsData = await _3th_part.LoadXLSXMarkups(filepath, SampleRate);
                if (markupsData == null)
                {
                    MessageBox.Show("Не удалось прочитать файд XLSX с разметкой",
                                    "Ошибка загрузки сегментации!",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (markupsData.Count != SegmentsHandler.ChannelsCount)
                {
                    MessageBox.Show("Число каналов в сигнале не совпадает с числом каналов в разметке!",
                        "Ошибка загрузки сегментации!",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                SegmentsHandler.AppendSegments(markupsData);
            });
        }

        public double[] GetSubSignal(byte channel, uint startIndex, uint count)
        {
            double[] res = new double[count];
            for (int i = 0; i < res.Length; i++)
            {
                res[i] = Data[channel][(int)startIndex + i];
            }
            return res;
        }

        public Data<float> GetSubSignal(long startIndex, long count)
        {
            if (startIndex + count > Data[0].Count)
                throw new IndexOutOfRangeException("Запрошено больше данных, чем есть!");

            Data<float> res = new Data<float>()
            {
                Shape = [1, Data.Count, count],
                EndSample = startIndex + count,
                StartSample = startIndex
            };
            Array array = Array.CreateInstance(typeof(float), Data.Count, count);
            
            

            for (int i = 0; i < Data.Count; i++)
            {
                float max = (float)(Data[i][(int)(startIndex)]);
                float min = (float)(Data[i][(int)(startIndex)]);

                for (int j = 0; j < count; j++)
                {
                    array.SetValue((float)(Data[i][(int)(startIndex+j)]), i, j);
                }
            }
            res.Signal = array;
            return res;
        }

        //TODO: реализовать корректный возврат каналов
        public double[] GetSamples(ulong index)
        {
            double [] res = new double[Data.Count];
            if (index < (ulong)Data[0].Count)
            {
                for (int i = 0; i < Data.Count; i++)
                {
                    res[i] = Data[i][(int)(index)];
                }
            }
            return res;
        }
    }
}
