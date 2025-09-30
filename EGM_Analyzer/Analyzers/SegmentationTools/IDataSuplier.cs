using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EGM_Analyzer.Analyzers.SegmentationTools
{
    public interface IDataSuplier
    {
        public List<List<Tuple<ulong, double>>> AnalyzedIntervals
        {
            get;
        }

        public void UpdateTargetPosition(ulong targetPosition);


    }
}
