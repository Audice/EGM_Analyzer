using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EGM_Analyzer.DataLoader.OldMurkup
{
    
    public class MarkupMetaInformation
    {
        public double threshold { get; set; } = 0.0;
        public string path_to_model { get; set; } = "";
        public string path_to_signal { get; set; } = "";

        public MarkupMetaInformation(double threshold, string path_to_model, string path_to_signal)
        {
            this.threshold = threshold;
            this.path_to_model = path_to_model;
            this.path_to_signal = path_to_signal;
        }

    }
    public class JSONMurkupsObject
    {
        public List<Murkup[]> peaks { get; set; }
        public MarkupMetaInformation meta { get; set; }

        public JSONMurkupsObject(MarkupMetaInformation meta, List<Murkup[]> peaks)
        {
            this.meta = meta;
            this.peaks = peaks;
        }
    }
}
