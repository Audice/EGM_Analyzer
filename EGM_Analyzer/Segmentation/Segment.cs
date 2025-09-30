using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EGM_Analyzer.Segmentation
{
    public enum SegmentType
    {
        AT,
        RT,
        Artifact,
        Noise
    }
    public enum SegmentationAgent
    {
        Human,
        AnalyticalAlgorithm,
        AI_Segmentator
    }
    public class Segment
    {
        /// <summary>
        /// Номер контакта с мультиэлектродной матрицы
        /// </summary>
        public byte Channel {  get; set; }
        /// <summary>
        /// Тип сегмента: AT, RT, Artifact, Noise
        /// </summary>
        public SegmentType Type { get; set; }
        /// <summary>
        /// Сэмпл начала сегмента
        /// </summary>
        public ulong StartMark { get; set; }
        /// <summary>
        /// Сэмпл конца сегмента
        /// </summary>
        public ulong EndMark { get; set; }
        /// <summary>
        /// Кто размечал: прямой алгоритм, человек или нейросеть
        /// </summary>
        public SegmentationAgent SegmentationAgent { get; set; }
        /// <summary>
        /// Конкретизирующая метка
        /// </summary>
        public double? ComplexMark { get; set; }

        public Segment(byte channel, SegmentType type, ulong startMark, ulong endMark, SegmentationAgent segmentationAgent, double? complexMark)
        {
            Channel = channel;
            Type = type;
            StartMark = startMark;
            EndMark = endMark;
            SegmentationAgent = segmentationAgent;
            ComplexMark = complexMark;
        }
    }
}
