using OxyPlot;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseData.Electrogram
{
    public class ExperementInfo
    {
        private enum Types { DATE_TIME, INT, DOUBLE, STRING }

        delegate object Parser(byte[] rawData);

        private static readonly string[] HEADERS = { "FileID", "FileVersion", "FPosFirstRecordingHdr",
            "MeaName", "MeaSN", "Date", "DateTicks", "ProgramName", "ProgramVersion", "Guid", "Comment" }; //17 версия

        


        


        public string FileID { get; private set; }
        public DateTime Date { get; private set; }
        public ushort FileVersion { get; private set; }
        public string DeviceName { get; private set; }
        public int DeviceID { get; private set; }
        public string ProgramName { get; private set; }
        public string ProgramVersion { get; private set; }
        public Guid GUID { get; private set; }
        public string Description { get; private set; }
        public int FPosFirstRecordingHdr { get; private set; }

        public ExperementInfo(byte[] rawData)
        {
            FindHeadersData(rawData);
        }

        public static uint GetHeaderSize(string path)
        {
            uint headerSize = 0;
            using (StreamReader sr = new StreamReader(path))
            {
                string sizeHeaderStr = "";
                do
                {
                    string line = sr.ReadLine();
                    if (line.Contains("FPosFirstRecordingHdr=")) { sizeHeaderStr = line; break; }
                } while (!sr.EndOfStream);
                int indexOfNumber = sizeHeaderStr.IndexOf("FPosFirstRecordingHdr=") + ("FPosFirstRecordingHdr=").Length;
                string number = "";
                for (int i = indexOfNumber; i < sizeHeaderStr.Length; i++)
                    if ((byte)sizeHeaderStr[i] > 47 && (byte)sizeHeaderStr[i] < 58)
                        number += sizeHeaderStr[i];
                    else
                        break;
                if (!uint.TryParse(number, out headerSize))
                    return 0;
            }
            return headerSize;
        }

        private void FindHeadersData(byte[] sourceData)
        {
            FileID = System.Text.Encoding.Default.GetString(ParseFunctions.ParseHeader(sourceData, Encoding.ASCII.GetBytes(HEADERS[0])));
            FileVersion = (ushort)ParseFunctions.ParseLong(ParseFunctions.ParseHeader(sourceData, Encoding.ASCII.GetBytes(HEADERS[1])));
            FPosFirstRecordingHdr = (int)ParseFunctions.ParseLong(ParseFunctions.ParseHeader(sourceData, Encoding.ASCII.GetBytes(HEADERS[2])));
            DeviceName = System.Text.Encoding.Default.GetString(ParseFunctions.ParseHeader(sourceData, Encoding.ASCII.GetBytes(HEADERS[3])));
            //DeviceID = (int)ParseFunctions.ParseLong(ParseFunctions.ParseHeader(sourceData, Encoding.ASCII.GetBytes(HEADERS[4])));
            Date = ParseFunctions.ParseDate(ParseFunctions.ParseHeader(sourceData, Encoding.ASCII.GetBytes(HEADERS[5])), FileVersion);
            ProgramName = System.Text.Encoding.Default.GetString(ParseFunctions.ParseHeader(sourceData, Encoding.ASCII.GetBytes(HEADERS[7])));
            ProgramVersion = System.Text.Encoding.Default.GetString(ParseFunctions.ParseHeader(sourceData, Encoding.ASCII.GetBytes(HEADERS[8])));
            GUID = ParseFunctions.ParseGuid(ParseFunctions.ParseHeader(sourceData, Encoding.ASCII.GetBytes(HEADERS[9])));
            Description = System.Text.Encoding.Default.GetString(ParseFunctions.ParseHeader(sourceData, Encoding.ASCII.GetBytes(HEADERS[10])));
        }




    }
}
