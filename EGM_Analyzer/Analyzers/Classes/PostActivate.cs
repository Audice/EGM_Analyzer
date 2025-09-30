using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EGM_Analyzer.Analyzers.Classes
{
    internal class PostActivate : IClass
    {

        private byte _classMark = 2;
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

        public PostActivate(byte classMark = 2)
        {
            _classMark = classMark;
        }
    }
}
