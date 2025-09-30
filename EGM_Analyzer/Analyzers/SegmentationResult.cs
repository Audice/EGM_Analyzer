using EGM_Analyzer.Analyzers.Classes;
using EGM_Analyzer.Segmentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EGM_Analyzer.Analyzers
{
    public class SegmentationResult: IAnalysisResult
    {
        // Список списков хранит список каналов, а каждый канал описывает принадлежность к одному из классов
        public List<List<byte>>? Results
        {
            get;
        }

        public List<IClass>? Marks
        {
            get; private set;
        }

        public long[]? AnalyzeShape { 
            get; private set; 
        }

        private long _signalPart = 0;
        private long _channelSize = 0;

        public SegmentationResult(byte channelCount, long[] analyzeShape, List<IClass> marks)
        {
            AnalyzeShape = analyzeShape;
            Results = new List<List<byte>>();
            Marks = marks;
            for (int i = 0; i < channelCount; i++)
            {
                Results.Add(new List<byte>());
            }
            _signalPart = AnalyzeShape[1] * AnalyzeShape[2];
            _channelSize = AnalyzeShape[2];
        }

        public void AppendPart(long startSample, long endSample, float[] prediction)
        {

            for (int i = 0; i < _signalPart; i++)
            {
                List<float> floats = new List<float>();
                for (int j = 0; j < Marks.Count; j++)
                {
                    floats.Add(prediction[i + j * _signalPart]);
                }
                float max = floats.Max();
                Results[(int)(i / _channelSize)].Add((byte)(floats.FindIndex(x => x == max)));
            }

            // Алгоритм такой.
            // 1 - Расчитываем первый участок
            // 2 - Возвращаемся назад на 100 отсчётов. Если это не фон, то ищем фон, а затем ищем первый не фон
            // 3 - опять идём назад до тех пор, пока не встретим что-то, отличное от фона
            // 4.1 - если всё фон, то следующий участок начинается с отметки = конец прошлого - нахлёст
            // 4.2 - если нашли не фон, то от его конца берём влево 100 отсчётов и получаем новый участок
            // 5 - добавлять в итоговый список новую часть будем с конца последнего найденного не фона. И добавлять будем с 100 семпла
        }

        public void Concatinate(IAnalysisResult analysisResult)
        {
            if (!(analysisResult is SegmentationResult))
                throw new ArgumentException("Попытка объединить два разных вида анализа");
            if (analysisResult.Results.Count != Results.Count)
                throw new ArgumentException("Попытка объединить разные размерности");

            try
            {
                for (int i = 0; i < Results.Count; i++)
                {
                    Results[i].AddRange(analysisResult.Results[i]);
                }
            }
            catch (Exception e)
            {
                throw new ArgumentException("Ошибка в объединении");
            }

        }

        public List<List<Segment>> GetSegments()
        {
            //Костыль...
            FillSpot();

            if (Results == null)
                throw new ArgumentException("Результат анализа сегментации пустой!");
            List<List<Segment>> segments = new List<List<Segment>>();
            for (int i = 0; i < Results.Count; i++)
            {
                segments.Add(new List<Segment>());
                int segmentsStart = -1;
                byte segmentCode = 0;
                for (int j = 0; j < Results[i].Count; j++)
                {
                    // Поправить
                    if (segmentsStart == -1 && Results[i][j] == 0)
                        continue;

                    if (segmentsStart == -1)
                    {
                        segmentsStart = j;
                        segmentCode = Results[i][j];
                        continue;
                    }

                    if (Results[i][j] != segmentCode)
                    {
                        segments[i].Add(new Segment((byte)i, (SegmentType)(segmentCode),
                            (ulong)segmentsStart, (ulong)j, SegmentationAgent.AI_Segmentator, null));
                        segmentsStart = -1;
                    }
                }
            }
            return segments;
        }

        private void FillSpot()
        {
            for (int i = 0; i < Results.Count; i++)
            {
                int indChange = -1;

                for (int j = 0; j < Results[i].Count - 1; j++)
                {
                    if (Results[i][j] != 0 && Results[i][j+1] == 0)
                    {
                        indChange = j;
                    }
                    if (Results[i][j] == 0 && Results[i][j + 1] != 0 && indChange != -1)
                    {
                        if (j - indChange < 10)
                        {
                            for (int k = indChange; k <= j; k++)
                            {
                                Results[i][k] = Results[i][j + 1];
                            }
                        }
                    }
                }
            }
        }

    }
}
