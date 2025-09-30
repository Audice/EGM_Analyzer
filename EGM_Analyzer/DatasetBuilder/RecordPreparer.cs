using EGM_Analyzer.DataLoader;
using EGM_Analyzer.DatasetBuilder.DataBase;
using EGM_Analyzer.EventsArgs;
using EGM_Analyzer.Segmentation;
using NumSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EGM_Analyzer.DatasetBuilder
{
    public class RecordPreparer
    {
        IDataLoader _dataLoader;

        public string SignalName
        {
            get;
            private set;
        }
        private string _signalPath;
        private string _rootDir;

        private ushort _partID = 0;


        public RecordPreparer(IDataLoader dataLoader, string rootPath, string signalName)
        {
            _dataLoader = dataLoader;
            _rootDir = rootPath;
            SignalName = signalName;
        }

        public RecordPreparer(IDataLoader dataLoader, string signalPath)
        {
            _dataLoader = dataLoader;
            _signalPath = signalPath;
        }


        private uint PartsStart = 0;
        bool _isMetadateInit = false;

        public RawRecord? GetRecord()
        {
            if (!_isMetadateInit)
            {
                throw new Exception("Не принициализированы поля. Сначала выполните функцию InitMetadates");
            }
            if (_channel >= _dataLoader.Data.Count)
            {
                _isMetadateInit = false;
                return null;
            }

            uint partSize = 0;
            double[]? subSignal = null;
            byte[]? subMask = null;
            RawRecord rawRecord = new RawRecord()
            {
                Channel = _channel,
                BaseFramerate = _targetFrameRate,
                IsValidRecord = true
            };


            if (PartsStart + _partSamples < _signalSize)
            {
                subSignal = new double[_partSamples];
                subMask = new byte[_partSamples];
                Array.Copy(_dataLoader.GetSubSignal(_channel, PartsStart, _partSamples), 0, subSignal, 0, _partSamples);
                Array.Copy(_markups, PartsStart, subMask, 0, _partSamples);
                //Проходим дальше по сигналу
                PartsStart += _partSamples;
            }
            else //Если у нас не хватает на целый кусок, то дробим
            {
                uint cutSamples = _signalSize - PartsStart;
                if (cutSamples > 0)
                {
                    subSignal = new double[cutSamples];
                    subMask = new byte[cutSamples];
                    Array.Copy(_dataLoader.GetSubSignal(_channel, PartsStart, cutSamples), 0, subSignal, 0, cutSamples);
                    Array.Copy(_markups, PartsStart, subMask, 0, cutSamples);
                }
                _channel++;
                _markups = SegmentsToArray((ulong)_signalSize, _dataLoader.SegmentsHandler.GetSegments(_channel));
                PartsStart = 0;
            }

            if (subMask != null && subSignal != null && subMask.Max() != 0)
            {
                rawRecord.Signal = subSignal;
                rawRecord.Markup = subMask;
            } 
            else
            {
                rawRecord.Signal = null;
                rawRecord.Markup = null;
                rawRecord.IsValidRecord = false;
            }

            return rawRecord;
        }

        private ushort _baseFramerate;
        private ushort _targetFrameRate;
        private bool _normalizeData;
        private uint _partDuration;
        private ushort _overlapDuration;
        private uint _step;
        private uint _partSamples;
        private uint _signalSize;
        private byte _channel;
        private byte[] _markups;

        public void InitMetadates(ushort baseFramerate, ushort targetFrameRate, 
            bool normalizeData, uint partDuration, ushort overlapDuration)
        {
            _baseFramerate = baseFramerate;
            _targetFrameRate = targetFrameRate;
            _normalizeData = normalizeData;
            _partDuration = partDuration;
            _overlapDuration = overlapDuration;

            _partSamples = (uint)((targetFrameRate * partDuration) / 1000.0);

            uint overlapSamples = (uint)((targetFrameRate * overlapDuration) / 1000.0);

            _step = _partSamples - overlapSamples;

            _signalSize = (uint)_dataLoader.Data[0].Count;
            _channel = 0;

            _markups = SegmentsToArray((ulong)_signalSize, _dataLoader.SegmentsHandler.GetSegments(_channel));

            _isMetadateInit = true;
        }

        byte _currentChannel = 0;

        public EventHandler<RecordPartInfoEventArgs> RecordPartHandled;
        private void OnRecordPartHandled(RecordPartInfoEventArgs e)
        {
            EventHandler<RecordPartInfoEventArgs> handler = this.RecordPartHandled;
            handler?.Invoke(this, e);
        }

        public void PrepareData(ushort baseFramerate, short targetFrameRate, 
            bool normalizeData, uint partDuration, ushort overlapDuration)
        {
            string preparedSignal = Path.Combine(_rootDir, SignalName + "_" + baseFramerate.ToString());
            DirectoryInfo directoryInfo = Directory.CreateDirectory(preparedSignal);
            if (!directoryInfo.Exists)
                throw new Exception("Не удаётся создать директорию для сохранения размечанных данных");

            for (int i = 0; i < _dataLoader.Data.Count; i++)
            {
                _currentChannel = (byte)(i + 1);

                string channelDir = Path.Combine(preparedSignal, "channel_" + (i + 1).ToString());
                DirectoryInfo channelDirInfo = Directory.CreateDirectory(channelDir);
                if (!channelDirInfo.Exists)
                    throw new Exception("Не удаётся создать директорию для сохранения канала");

                var signal = _dataLoader.Data[i].ToArray();
                var markups = SegmentsToArray((ulong)signal.Length, _dataLoader.SegmentsHandler.GetSegments((byte)i));

                if (targetFrameRate > 0)
                {
                    throw new Exception("Не реализовано");
                }
                if (normalizeData)
                {
                    Normalize(signal);
                }
                Cutting(signal, markups, channelDir, baseFramerate, partDuration, overlapDuration);
            }
        }

        private void Cutting(double[] signal, byte[] mask, string channelsFolder,
            ushort framerate, uint partDuration, ushort overlapDuration)
        {
            uint partID = 0;
            uint partSamples = (uint)((framerate * partDuration) / 1000.0);
            uint overlapSamples = (uint)((framerate * overlapDuration) / 1000.0);
            if (overlapSamples > partSamples)
                throw new Exception("Протяжённость сдвига превышает участок сигнала");
            uint step = partSamples - overlapSamples;

            uint partcount = (uint)(signal.Length / step);

            uint startIndex = 0;

            while (startIndex + partSamples < signal.Length)
            {
                SaveSubRecord(signal, mask, partID, channelsFolder, startIndex, partSamples);
                partID++;
                startIndex += step;

                OnRecordPartHandled(new RecordPartInfoEventArgs()
                {
                    ChannelNumber = _currentChannel,
                    PartNumber = partID,
                     PartCount = partcount
                });

            }
            //Добавить последний кусок в выборку
            SaveSubRecord(signal, mask, partID, channelsFolder, (uint)(signal.Length - partSamples - 1), partSamples);
        }

        private void SaveSubRecord(double[] signal, byte[] mask, uint id, string channelPath, uint startIndex, uint count)
        {
            double[] subSignal = new double[count];
            byte[] subMask = new byte[count];
            Array.Copy(signal, startIndex, subSignal, 0, count);
            Array.Copy(mask, startIndex, subMask, 0, count);

            string subSignalFile = Path.Combine(channelPath, id.ToString() + ".npy");
            np.save(subSignalFile, subSignal);
            string subMaskFile = Path.Combine(channelPath, id.ToString() + "_mask.npy");
            np.save(subMaskFile, subMask);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">Числовые типы данных</typeparam>
        /// <param name="signal">Сигнал, который необходимо нормализовать</param>
        private double[] Normalize(double[] signal)
        {
            double[] result = new double[signal.Length];
            double maximum = signal.Max();
            double minimum = signal.Min();
            //Если у максимума и минимума дельта в районе ошибки, то это прямая
            if (Math.Abs(minimum - maximum) > 0.00000001)
            {
                for (int i = 0; i < signal.Length; i++)
                {
                    result[i] = (signal[i] - minimum) / (maximum - minimum);
                }
            }
            return result;
        }


        private void BringToFrequency(Array array, ushort baseSampleRate, ushort targetSampleRate)
        {
            for (int i = 0; i < array.Length; i++)
            {
                //int a = array[i];
            }
        }

        public byte[] SegmentsToArray(ulong signalSize, List<Segment> segments)
        {
            byte[] arraySegments = new byte[signalSize];
            foreach (var segment in segments)
            {
                if (segment.StartMark > (ulong)arraySegments.Length 
                    || segment.EndMark > (ulong)arraySegments.Length)
                    continue;

                for (ulong i = segment.StartMark; i < segment.EndMark; i++)
                {
                    arraySegments[i] = (byte)(segment.Type + 1);
                }
            }
            return arraySegments;
        }

         


    }
}
