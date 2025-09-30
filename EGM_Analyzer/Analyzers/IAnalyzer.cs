using EGM_Analyzer.DataLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EGM_Analyzer.Analyzers
{
    public enum AnalyzeType
    {
        Segmentation, Classification
    }
    /// <summary>
    /// Общий интерфейс для всех анализаторов
    /// </summary>
    public interface IAnalyzer
    {
        public IAnalysisResult? AnalysisResult { get; }
        public long[]? AnalyzeShape { get; }
        // Тип анализатора
        public AnalyzeType AnalyzeType { get; }
        // Путь до модели
        public string? ModelPath { get; }
        // Инициализация модели
        public void ModelInitialization();
        
        public Task Analyze(IDataLoader dataLoader);

    }
}
