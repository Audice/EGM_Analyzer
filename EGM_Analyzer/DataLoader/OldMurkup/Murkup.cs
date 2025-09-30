using EGM_Analyzer.Segmentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EGM_Analyzer.DataLoader.OldMurkup
{
    public class Murkup
    {
        public double position { get; set; } = 0.0;
        public string creation_stage { get; set; } = "";
        public double[] search_segment { get; set; } = {0.0, 1.1};

        public Murkup(double position, string creation_stage, double[] search_segment)
        {
            this.position = position;
            this.creation_stage = creation_stage;
            this.search_segment = search_segment;
        }


    }
}
