using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseData.Electrogram
{
    internal class SignalPart
    {
        private static readonly string[] HEADERS = { "Entity", "FPosPrev", "FPosPrevID",
            "FPosNext", "FPosNextID", "TimeStamp", "Size"};

        public ushort Entity { get; private set; }
        public uint Size { get; private set; }
        public long TimeStamp { get; private set; }

        public byte[] RawSignal { get; private set; }

        public SignalPart(byte[] bytes)
        {
            ParseHeaders(bytes);
        }

        private void ParseHeaders(byte[] rawData)
        {
            Entity = (ushort)ParseFunctions.ParseLong(ParseFunctions.ParseHeader(rawData, Encoding.ASCII.GetBytes(HEADERS[0])));
            Size = (uint)ParseFunctions.ParseLong(ParseFunctions.ParseHeader(rawData, Encoding.ASCII.GetBytes(HEADERS[6])));
            TimeStamp = ParseFunctions.ParseLong(ParseFunctions.ParseHeader(rawData, Encoding.ASCII.GetBytes(HEADERS[5])));
        }

        public void SetRawSignal(byte[] rawSignal)
        {
            if (rawSignal.Length == Size)
                RawSignal = rawSignal;

        }


    }
}
