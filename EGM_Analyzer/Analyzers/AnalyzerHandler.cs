using EGM_Analyzer.DataLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EGM_Analyzer.Analyzers
{
    public class AnalyzerHandler
    {
        public IDataLoader DataLoader
        {
            get;
        }

        public IAnalyzer Analyzer
        {
            get;
        }

        public AnalyzerHandler(IDataLoader dataLoader, IAnalyzer analyzer)
        {
            Analyzer = analyzer;
            DataLoader = dataLoader;
        }




    }
}
