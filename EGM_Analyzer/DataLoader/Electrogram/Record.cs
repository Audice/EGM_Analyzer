using NumSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace ParseData.Electrogram
{
    internal class Record
    {
        private static readonly string[] HEADERS = { "CheckSum", "RecordingID", "FPosNext",
            "FPosNextRecordingHdr", "RecordingType", "TimeStamp", "Duration", "Label", "FrameStreams",
            "AnalogStreams", "Stream", "DataType", "SubDataType", "Entities"};

        /// <summary>
        /// Список каналов 
        /// </summary>
        private List<Channel> _channels;
        private List<ChannelSecretTail> _tails;

        public ushort NumChannels { get; private set; }

        public int RecordingID { get; private set; }

        public long Duration { get; private set; }

        public long LastDuration { get; private set; }

        public Record(byte[] rawData)
        {
            _channels = new List<Channel>();
            _tails = new List<ChannelSecretTail>();
            ParseRecordInfo(rawData);
        }

        public void ParseRecordInfo(byte[] bytes)
        {
            var str = System.Text.Encoding.Default.GetString(bytes);

            NumChannels = (ushort)ParseFunctions.ParseLong(ParseFunctions.ParseHeader(bytes, Encoding.ASCII.GetBytes(HEADERS[13])));
            RecordingID = (int)ParseFunctions.ParseLong(ParseFunctions.ParseHeader(bytes, Encoding.ASCII.GetBytes(HEADERS[1])));
            Duration = ParseFunctions.ParseLong(ParseFunctions.ParseHeader(bytes, Encoding.ASCII.GetBytes(HEADERS[6])));
        }

        public void AddChannel(byte[] bytes)
        {
            var channel = new Channel();
            channel.ParseChannelDiscription(bytes);
            _channels.Add(channel);
        }

        public bool stopeRead = false;
        public void AppendSignalPart(SignalPart signalPart)
        {
            var channel = _channels.Find(x => x.Entity == signalPart.Entity);
            if (channel != null)
            {
                channel.AppendSignal(signalPart);
            }
            if (signalPart.Entity == _channels.Count)
            {
                stopeRead = channel.NumSamples >= Duration;
            }
        }

        public void AppendTail(ChannelSecretTail secretTail)
        {
            _tails.Add(secretTail);
        }

        public List<double> GetChannelSignal(int channelIndex)
        {
            return _channels[channelIndex].GetSignal();
        }

        public void SaveChannelsData(string phaseFolder)
        {
            double[] refArray = new double[_channels[0].SignalSize];
            foreach (var channel in _channels)
            {
                channel.DoubleRepresentation(refArray);
                np.save(phaseFolder + "/" + channel.Label + ".npy", refArray);
                GC.Collect();
            }
/*            int len = (int)_channels[0].SignalSize;
            double xStep = (double)(_channels[0].Tick) / 1000000;
            double[] xs = new double[len];

            for (int j = 0; j < len; j++)
            {
                xs[j] = j * xStep;
            }
            np.save("X.npy", xs);*/
        }

        public static uint GetRecordInfoSize(string path)
        {////
            uint headerSize = 0;
            byte[] recordIDsBytes = Encoding.ASCII.GetBytes("RecordingID=0");
            byte[] entityBytes = Encoding.ASCII.GetBytes("Entity=");

            int recordIDindex = 0;
            int entityIndex = 0;

            using (StreamReader sr = new StreamReader(path))
            {

                while (!sr.EndOfStream)
                {
                    recordIDindex++;
                    if (sr.Read() == recordIDsBytes[0])
                    {
                        int i = 1;
                        for (i = 1; i < recordIDsBytes.Length; i++)
                        {
                            recordIDindex++;
                            if (sr.Read() != recordIDsBytes[i])
                                break;
                        }
                        if (i != recordIDsBytes.Length)
                        {
                            continue;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                entityIndex = recordIDindex;
                recordIDindex = recordIDindex - recordIDsBytes.Length;
                while (!sr.EndOfStream)
                {
                    entityIndex++;
                    if (sr.Read() == entityBytes[0])
                    {
                        int i = 1;
                        for (i = 1; i < entityBytes.Length; i++)
                        {
                            entityIndex++;
                            if (sr.Read() != entityBytes[i])
                                break;
                        }
                        if (i != entityBytes.Length)
                        {
                            continue;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
            entityIndex = entityIndex - entityBytes.Length;
            headerSize = (uint)(entityIndex - recordIDindex);

            using (StreamReader sr = new StreamReader(path))
            {
                byte[] buffer = new byte[3100];   
                sr.BaseStream.Read(buffer, 0, buffer.Length);
                int s = 0;
            }

            return headerSize;
        }


        public static uint GetChennalInfoSize(string path)
        {////
            uint headerSize = 0;
            byte[] recordIDsBytes = Encoding.ASCII.GetBytes("Entity=1 ");
            byte[] entityBytes = Encoding.ASCII.GetBytes("Entity=2 ");

            int recordIDindex = 0;
            int entityIndex = 0;

            using (StreamReader sr = new StreamReader(path))
            {

                while (!sr.EndOfStream)
                {
                    recordIDindex++;
                    if (sr.Read() == recordIDsBytes[0])
                    {
                        int i = 1;
                        for (i = 1; i < recordIDsBytes.Length; i++)
                        {
                            recordIDindex++;
                            if (sr.Read() != recordIDsBytes[i])
                                break;
                        }
                        if (i != recordIDsBytes.Length)
                        {
                            continue;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                entityIndex = recordIDindex;
                recordIDindex = recordIDindex - recordIDsBytes.Length;
                while (!sr.EndOfStream)
                {
                    entityIndex++;
                    if (sr.Read() == entityBytes[0])
                    {
                        int i = 1;
                        for (i = 1; i < entityBytes.Length; i++)
                        {
                            entityIndex++;
                            if (sr.Read() != entityBytes[i])
                                break;
                        }
                        if (i != entityBytes.Length)
                        {
                            continue;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
            entityIndex = entityIndex - entityBytes.Length;
            headerSize = (uint)(entityIndex - recordIDindex);

            using (StreamReader sr = new StreamReader(path))
            {
                byte[] buffer = new byte[3100];
                sr.BaseStream.Read(buffer, 0, buffer.Length);
                int s = 0;
            }

            return headerSize;
        }

        public void ReadCheckBlock(FileStream sr)
        {
            uint headerSize = 0;
            byte[] entitySizesBytes = Encoding.ASCII.GetBytes("EntitySizes=" + this.NumChannels.ToString());
            byte[] entityBytes = Encoding.ASCII.GetBytes("Entity=" + this.NumChannels.ToString());

            byte[] array = new byte[entitySizesBytes.Length];
            sr.Read(array, 0, entitySizesBytes.Length);
            sr.Position -= entitySizesBytes.Length;
            if (System.Text.Encoding.Default.GetString(array) == ("EntitySizes=" + this.NumChannels.ToString()))
            {
                //Если мы нашли наш заголовок, то прочитаем этот чекер
                uint numBytes = (uint)((this.NumChannels * 2 + 1) * 50); // 50 байт уходит на хранение строки
                array = new byte[numBytes];
                sr.Read(array, 0, array.Length);
                string str = System.Text.Encoding.Default.GetString(array);
            }
        }

    }
}
