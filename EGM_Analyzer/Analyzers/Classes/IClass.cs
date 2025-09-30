using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EGM_Analyzer.Analyzers.Classes
{
    public interface IClass
    {
        /// <summary>
        /// Метка класса
        /// </summary>
        public byte ClassMark { get; }
        /// <summary>
        /// Для какой задачи предназначен
        /// </summary>
        public AnalyzeType AnalyzeType { get; }
    }
}
