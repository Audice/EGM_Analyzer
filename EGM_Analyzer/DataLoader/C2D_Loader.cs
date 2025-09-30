using EGM_Analyzer.Analyzers;
using EGM_Analyzer.DataLoader.OldMurkup;
using EGM_Analyzer.EventsArgs;
using EGM_Analyzer.Save;
using EGM_Analyzer.Segmentation;
using NumSharp;
using ParseData.Electrogram;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace EGM_Analyzer.DataLoader
{
    public class C2D_Loader : IDataLoader
    {
        public string? SignalFilepath
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

        public ReadOnlyCollection<ReadOnlyCollection<double>> Data
        {
            get; private set;
        }

        public LoadSignalState LoadSignalState
        {

            get; private set;
        }

        public ulong CurrentSegment => throw new NotImplementedException();

        public ulong TargetPosition { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public event EventHandler<LoadingStateEventArgs> LoadStateChanged;
        public event EventHandler<SegmentEventArgs> CurrentSegmentChanged;

        public C2D_Loader(string signalPath, ushort sampleRate)
        {
            SignalFilepath = signalPath;
            LoadSignalState = DetermineLoadSignalState(signalPath);
            SampleRate = sampleRate;
        }

        public LoadSignalState DetermineLoadSignalState(string signalFilepath)
        {
            FileInfo fileInfo = new FileInfo(signalFilepath);
            switch (fileInfo.Extension)
            {
                case ".c2d":
                    return LoadSignalState.Full;
                case ".bin":
                    return LoadSignalState.Part;
                default:
                    throw new ArgumentException("DetermineLoadSignalState: Неопределённый формат сигнала!");
            }
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

        public Task LoadSignal()
        {
            return Task.Run(() =>
            {
                FileInfo fileInfo = new FileInfo(SignalFilepath);
                if (fileInfo.Extension != ".c2d")
                {
                    MessageBox.Show("Некорректный формат сигнала.", "Ошибка открытия файла! Формат не соответствует .c2d", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                ReadHandler readHandler = new ReadHandler(SignalFilepath);

                List<ReadOnlyCollection<double>> data = new List<ReadOnlyCollection<double>>();
                for (int i = 0; i < readHandler.CurrentRecord.NumChannels; i++)
                {
                    var a = readHandler.CurrentRecord.GetChannelSignal(i);
                    if (a == null)
                        throw new Exception("Ошибка загрузки c2d!");
                    data.Add(new ReadOnlyCollection<double>(a));
                }
                Data = new ReadOnlyCollection<ReadOnlyCollection<double>>(data);
                SegmentsHandler = new SegmentsHandler((byte)readHandler.CurrentRecord.NumChannels, new FileInfo(SignalFilepath), SampleRate);
            });
        }

        public double[] LoadSignalPart(byte leadNumber, long startIndex, int count)
        {
            var targetLead = Data[leadNumber];
            return targetLead.ToList().GetRange((int)startIndex, count).ToArray();
        }

        public Task ResaveSignal(string filepath)
        {
            return Task.Run(() =>
            {
                if (LoadSignalState == LoadSignalState.Full)
                {
                    FileInfo fileInfo = new FileInfo(filepath);
                    switch (fileInfo.Extension)
                    {
                        case ".npy":
                            double[,] data = new double[Data.Count, Data[0].Count];
                            for (int i = 0; i < Data.Count; i++)
                            {
                                for (int j = 0; j < Data[0].Count; j++)
                                {
                                    data[i, j] = Data[i][j];
                                }
                            }
                            np.save(filepath, data);
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

        private void OnLoadStateChange(LoadingStateEventArgs e)
        {
            LoadStateChanged?.Invoke(this, e);
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
            return Data[channel].ToList().GetRange((int)startIndex, (int)count).ToArray();
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
                for (int j = 0; j < count; j++)
                {
                    array.SetValue((float)(Data[i][(int)(startIndex + j)]), i, j);
                }
            }
            res.Signal = array;
            return res;
        }

        public double[] GetSamples(ulong index)
        {
            throw new NotImplementedException();
        }
    }
}
