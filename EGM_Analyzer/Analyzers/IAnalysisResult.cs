using EGM_Analyzer.Segmentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EGM_Analyzer.Analyzers
{
    // Сделать отдельный интерфейс для разных типов задач
    public interface IAnalysisResult
    {
        public List<List<byte>>? Results
        {
            get;
        }
        public void AppendPart(long startSample, long endSample, float[] prediction);
        public void Concatinate(IAnalysisResult analysisResult);

        public List<List<Segment>> GetSegments();
    }
}
