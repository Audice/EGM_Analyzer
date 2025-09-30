using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseData.Electrogram
{
    internal class ChannelSecretTail
    {
        private static readonly string[] HEADERS = { "EntityIndex", "Size"};
        public ushort EntityIndex { get; private set; }
        public uint Size { get; private set; }
        public List<byte> Data { get; private set; }

        public List<long> TailSamples { get; private set; }
        public ChannelSecretTail(byte[] rawData)
        {
            EntityIndex = (ushort)ParseFunctions.ParseLong(ParseFunctions.ParseHeader(rawData, Encoding.ASCII.GetBytes(HEADERS[0])));
            Size = (uint)ParseFunctions.ParseLong(ParseFunctions.ParseHeader(rawData, Encoding.ASCII.GetBytes(HEADERS[1]))) * 8; // В файле под хвост уходит 8 байт
        }

        public void SetData(byte[] data)
        {
            Data = new List<byte>(data);

            var output = new long[Data.Count / 8];
            Buffer.BlockCopy(Data.ToArray(), 0, output, 0, Data.Count);
            TailSamples = new List<long>(output);

        }

    }
}
