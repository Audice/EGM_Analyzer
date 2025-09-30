using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EGM_Analyzer.Analyzers.Classes
{
    public class Noise : IClass
    {
        private byte _classMark = 4;
        public byte ClassMark
        {
            get => _classMark;
        }

        public AnalyzeType AnalyzeType
        {
            get
            {
                return AnalyzeType.Segmentation;
            }
        }

        public Noise(byte mark = 4)
        {
            _classMark = mark;
        }
    }
}
