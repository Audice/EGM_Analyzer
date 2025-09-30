using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseData.Electrogram
{
    internal class Channel
    {
        private static readonly string[] HEADERS = { "Entity", "FPosNextID", "FPosIndex",
            "ChID", "Label", "RawDataType", "ADZero", "Unit", "Exponent", "ConversionFactor", "Tick", "HighPassFilterType","HighPassFilterOrder","HighPassFilterCutOffFrequency","LowPassFilterType","LowPassFilterOrder","LowPassFilterCutOffFrequency"};
        
        public ushort Entity { get; private set; }
        public string Label { get; private set; }

        public ushort ChannelID { get; private set; }

        public int ADZero { get; private set; }

        public string Unit { get; private set; }
        public short Exponent { get; private set; }
        public double ConversionFactor { get; private set; }

        public byte SampleBytes { get; private set; }

        public ushort Tick { get; private set; }

        public long NumSamples { get; private set; }

        List<double> _signal;

        public int SignalSize { get => _signal?.Count ?? 0; }

        public List<double> GetSignal()
        {
            return _signal;
        }

        public Channel()
        {
            _signal = new List<double>();
        }

        public void ParseChannelDiscription(byte[] array)
        {
            Entity = (ushort)ParseFunctions.ParseLong(ParseFunctions.ParseHeader(array, Encoding.ASCII.GetBytes(HEADERS[0])));
            Label = System.Text.Encoding.Default.GetString(ParseFunctions.ParseHeader(array, Encoding.ASCII.GetBytes(HEADERS[4])));
            ChannelID = (ushort)ParseFunctions.ParseLong(ParseFunctions.ParseHeader(array, Encoding.ASCII.GetBytes(HEADERS[3])));
            ADZero = (int)ParseFunctions.ParseLong(ParseFunctions.ParseHeader(array, Encoding.ASCII.GetBytes(HEADERS[6])));
            Unit = System.Text.Encoding.Default.GetString(ParseFunctions.ParseHeader(array, Encoding.ASCII.GetBytes(HEADERS[7])));
            Exponent = (short)ParseFunctions.ParseLong(ParseFunctions.ParseHeader(array, Encoding.ASCII.GetBytes(HEADERS[8])));
            ConversionFactor = (double)ParseFunctions.ParseLong(ParseFunctions.ParseHeader(array, Encoding.ASCII.GetBytes(HEADERS[9])));
            Tick = (ushort)ParseFunctions.ParseLong(ParseFunctions.ParseHeader(array, Encoding.ASCII.GetBytes(HEADERS[10]))); //?????
            DefineTypeSamples(System.Text.Encoding.Default.GetString(ParseFunctions.ParseHeader(array, Encoding.ASCII.GetBytes(HEADERS[5]))));
        }

        private void DefineTypeSamples(string strType)
        {
            if (strType.Contains("UShort"))
                SampleBytes = 2;
            if (strType.Contains("Int"))
                SampleBytes = 4;

        }

        public void DoubleRepresentation(double[] refArray)
        {
            for (int i=0; i < refArray.Length; i++)
            {
                refArray[i] = (double)((_signal[i] - ADZero) * ConversionFactor / 1000);
            }
        }

        public void AppendSignal(SignalPart signalPart)
        {
            var output = new ushort[signalPart.Size / 2];
            Buffer.BlockCopy(signalPart.RawSignal, 0, output, 0, (int)signalPart.Size);
            for (int i = 0; i < output.Length; i++)
            {
                _signal.Add(output[i]);// .AddRange(output);
            }
            

/*            for (int i=0; i < output.Length; i++)
            {
                _signal.Add((double)((output[i] - ADZero) * ConversionFactor / 1000));
            }*/

            //

            NumSamples = _signal.Count * Tick;
        }

    }
}
