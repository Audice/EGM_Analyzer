using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EGM_Analyzer.EventsArgs
{
    public class RecordPartInfoEventArgs : EventArgs
    {
        public byte ChannelNumber { get; set; }
        public uint PartNumber { get; set; }
        public uint PartCount { get; set; }
    }
}
