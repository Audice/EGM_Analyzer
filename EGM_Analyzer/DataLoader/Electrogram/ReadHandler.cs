using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseData.Electrogram
{
    internal class ReadHandler
    {
        private const uint EXPEREMENT_INFO_BYTE_COUNT = 1910; //1600;
        private const uint RECORD_INFO_BYTE_COUNT = 650;
        private const uint CHECKSUM_INFO_BYTE_COUNT = 50;
        private const uint CHANNEL_INFO_BYTE_COUNT = 850;
        private const uint SIGNAL_PART_INFO_BYTE_COUNT = 350;
        private const uint ENTITY_INDEX_BYTE_COUNT = 100;
        private readonly byte[] RECORD_TITLE = Encoding.ASCII.GetBytes("RecordingID");

        private ExperementInfo _experementInfo;
        public Record CurrentRecord { get; private set; }
        public List<Record> Records { get; private set; }
        public ReadHandler(string path = "03_01_24 Lcar 1_MEA 1.c2d")
        {
            byte countRecord = GetPhaseCount(path);
            uint sizeRecordHeader = Electrogram.ExperementInfo.GetHeaderSize(path);
            uint sizereched = Record.GetRecordInfoSize(path);
            uint sizeChennalInfo = Record.GetChennalInfoSize(path);
            Records = new List<Record>();

            using (FileStream sr = new FileStream(path, FileMode.Open))
            {
                //1. Считытваем шапку файла с информацией об эксперементе
                byte[] buffer = new byte[sizeRecordHeader];
                sr.Read(buffer, 0, buffer.Length);
                ExperementInfo(buffer);

/*                buffer = new byte[CHECKSUM_INFO_BYTE_COUNT];
                sr.Read(buffer, 0, buffer.Length);*/
                for (int record = 0; record < countRecord; record++)
                {
                    //2. Считываем информацию о записи
                    buffer = new byte[sizereched];
                    sr.Read(buffer, 0, buffer.Length);

                    RecordInfo(buffer);
                    //3. Считываем информацию о каждом канале записи
                    for (int i = 0; i < CurrentRecord.NumChannels; i++)
                    {
                        buffer = new byte[sizeChennalInfo];
                        sr.Read(buffer, 0, buffer.Length);
                        CurrentRecord.AddChannel(buffer);
                    }
                    //4.Считываем хвост, 1250... Не совсем... Нужно дочитать до строки Comment, и уже потом до 13 10
                    /*
                    byte[] commentsBytes = Encoding.ASCII.GetBytes("Comments");
                    int curByte = -1;
                    int numReadBytes = 0;
                    while (true)
                    {
                        curByte = sr.ReadByte();
                        numReadBytes++;
                        if (commentsBytes[0] == curByte)
                        {
                            int i = 0;
                            for (i = 1; i <  commentsBytes.Length; i++)
                            {
                                curByte = sr.ReadByte();
                                numReadBytes++;
                                if (curByte != commentsBytes[i]) break;
                            }
                            if (i == commentsBytes.Length) break;
                        }
                        if (numReadBytes > 10000) throw new Exception("Bad format");
                    }
                    */


                    while (true)
                    {
                        int maybeR = sr.ReadByte();
                        if (maybeR == 13 && sr.ReadByte() == 10)
                            break;
                    }

                    //Считываем сигнал... долго считываем
                    while (!CurrentRecord.stopeRead)
                    {
                        //6.1 считываем шапку сигнала
                        buffer = new byte[SIGNAL_PART_INFO_BYTE_COUNT];
                        sr.Read(buffer, 0, buffer.Length);

                        string str = System.Text.Encoding.Default.GetString(buffer);


                        SignalPart signalPart = new SignalPart(buffer);
                        //6.2 считываем часть сигнала
                        buffer = new byte[signalPart.Size];
                        sr.Read(buffer, 0, buffer.Length);
                        signalPart.SetRawSignal(buffer);
                        //6.3 пихаем в общий список
                        CurrentRecord.AppendSignalPart(signalPart);
                    }

                    //В версии 23 года появляется контроль считанных байт
                    CurrentRecord.ReadCheckBlock(sr);

                    //7. Считываем хвост - UNDEFINE
                    for (int i = 0; i < CurrentRecord.NumChannels; i++)
                    {
                        //6.1 считываем шапку сигнала
                        buffer = new byte[ENTITY_INDEX_BYTE_COUNT];
                        sr.Read(buffer, 0, buffer.Length);

                        string str = System.Text.Encoding.Default.GetString(buffer);

                        ChannelSecretTail channelSecretTail = new ChannelSecretTail(buffer); //Остановился тут!
                        //6.2 считываем часть сигнала
                        buffer = new byte[channelSecretTail.Size];
                        sr.Read(buffer, 0, buffer.Length);
                        channelSecretTail.SetData(buffer);
                        //6.3 пихаем в общий список
                        CurrentRecord.AppendTail(channelSecretTail);
                    }
                    Records.Add(CurrentRecord);
                }
                

                int s = 0;
                s += 12;
            }
        }

        public void SaveRecords()
        {
            for (int i = 0; i < Records.Count; i++)
            {
                string folderName = "phase" + i.ToString();
                Directory.CreateDirectory(folderName);
                Records[i].SaveChannelsData(folderName);
            }
        }

        private byte GetPhaseCount(string path)
        {
            byte count = 0;
            using (StreamReader sr = new StreamReader(path))
            {
                do
                {
                    string line = sr.ReadLine();
                    if (line.Contains("RecordingID")) count++;
                } while (!sr.EndOfStream);
            }
            return count;
        }

        private void ExperementInfo(byte[] rawBytes)
        {
            _experementInfo = new ExperementInfo(rawBytes);
        }

        private void RecordInfo(byte[] rawBytes)
        {
            CurrentRecord = new Record(rawBytes);
        }

    }
}
