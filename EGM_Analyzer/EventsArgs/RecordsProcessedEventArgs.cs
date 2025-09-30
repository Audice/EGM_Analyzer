using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EGM_Analyzer.EventsArgs
{
    public class RecordsProcessedEventArgs : EventArgs
    {
        public ushort NumRecords { get; set; }
        public ushort CurrentRecordsNumber { get; set; }
        public uint NumParts { get; set; }
        public uint CurrentPartNumber { get; set; }
        public byte ChannelNumber { get; set; }
        public string? CurrentRecordName { get; set; }

    }
}
