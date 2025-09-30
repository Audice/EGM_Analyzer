using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EGM_Analyzer.Analyzers.Classes
{
    public class Artefacts : IClass
    {
        private byte _classMark = 3;
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

        public Artefacts(byte mark = 3)
        {
            _classMark = mark;
        }
    }
}
