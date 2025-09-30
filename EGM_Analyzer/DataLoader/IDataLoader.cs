using EGM_Analyzer.Analyzers;
using EGM_Analyzer.EventsArgs;
using EGM_Analyzer.Segmentation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EGM_Analyzer.DataLoader
{
    public enum LoadSignalState { Full, Part }
    public interface IDataLoader
    {
        public string? SignalFilepath { get; }

        public ushort SampleRate { get; }

        public ulong TargetPosition { get; set; }

        public event EventHandler<LoadingStateEventArgs> LoadStateChanged;

        public event EventHandler<SegmentEventArgs> CurrentSegmentChanged;

        public SegmentsHandler? SegmentsHandler { get; }

        public LoadSignalState LoadSignalState { get; }

        public ReadOnlyCollection<ReadOnlyCollection<double>> Data { get; }

        public Task LoadMarkups(string filepath);

        public Task LoadXLSXMarkups(string filepath);

        public Task LoadSignal();

        public Task LoadMarkups(string filepath, byte startChannek, byte endChannel);

        public double[] LoadSignalPart(byte leadNumber, long startIndex, int count);
        public Task ResaveSignal(string filepath);
        public Task AppendSegments(string filepath);
        public double[] GetSubSignal(byte channel, uint startIndex, uint count);

        public LoadSignalState DetermineLoadSignalState(string signalFilepath);

        public Data<float> GetSubSignal(long startIndex, long count);

        public double[] GetSamples(ulong index);
    }
}
