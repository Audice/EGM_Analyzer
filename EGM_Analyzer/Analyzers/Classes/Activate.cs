using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EGM_Analyzer.Analyzers.Classes
{
    public class Activate : IClass
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

        public Activate(byte mark = 1)
        {
            _classMark = mark;
        }



    }
}
