using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EGM_Analyzer.EventsArgs
{
    public class SegmentEventArgs : EventArgs
    {
        public required double[] DataMatrix { get; set; }
    }
}
