using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EGM_Analyzer.Analyzers.Classes
{
    internal class PreActivate : IClass
    {
        private byte _classMark = 1;
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
    }
}
