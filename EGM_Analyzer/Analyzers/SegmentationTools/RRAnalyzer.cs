using EGM_Analyzer.DataLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EGM_Analyzer.Analyzers.SegmentationTools
{
    public class RRAnalyzer : IDataSuplier
    {
        IDataLoader _dataLoader;

        public List<List<Tuple<ulong, double>>> AnalyzedIntervals
        {
            get; private set;
        }

        public List<List<ulong>> ActivationMoments
        {
            get; private set;
        }

        private ushort _sampleRate;

        public RRAnalyzer(IDataLoader dataLoader, ushort sampleRate = 5000)
        {
            _sampleRate = sampleRate;
            AnalyzedIntervals = new List<List<Tuple<ulong, double>>>();
            ActivationMoments = new List<List<ulong>>();
            _dataLoader = dataLoader;
            for (int i=0; i < dataLoader.Data.Count; i++)
            {
                AnalyzedIntervals.Add(new List<Tuple<ulong, double>>());
                ActivationMoments.Add(new List<ulong>());
            }
            FillRRIntervals();
        }

        private void FillRRIntervals()
        {
            FillActivations();
            for (int i = 0; i < ActivationMoments.Count; i++)
            {
                for (int j = 0; j < ActivationMoments[i].Count - 1; j++)
                {
                    AnalyzedIntervals[i].Add(new Tuple<ulong, double>(ActivationMoments[i][j + 1], 
                        (double)(ActivationMoments[i][j + 1] - ActivationMoments[i][j]) / _sampleRate));
                }
            }
        }

        private void FillActivations()
        {
            for (int i=0; i < ActivationMoments.Count; i++)
            {
                var segments = _dataLoader.SegmentsHandler.GetSegments((byte)i);
                foreach (var segment in segments)
                {
                    double maxDelta = double.MinValue;
                    ulong amIndex = segment.StartMark;
                    for (ulong k = segment.StartMark; k < segment.EndMark - 1; k++)
                    {
                        double newDelta = _dataLoader.Data[i][(int)k] - _dataLoader.Data[i][(int)k + 1];
                        if (newDelta > maxDelta)
                        {
                            maxDelta = newDelta;
                            amIndex = k;
                        }
                    }
                    ActivationMoments[i].Add(amIndex);
                }
            }
        }

        public void UpdateTargetPosition(ulong targetPosition)
        {
            _dataLoader.TargetPosition = targetPosition;
        }
    }
}
