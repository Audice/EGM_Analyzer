using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EGM_Analyzer.DatasetBuilder.DataBase
{
    public class DatabaseRecord
    {
        [Name("Название")]
        public string? FileName { get; set; }

        [Name("Условия перфузии сердца")]
        public string? CardiacPerfusionConditions { get; set; }

        [Name("Модель матрицы")]
        public string? MatrixModel { get; set; }

        [Name("Частота записи файла")]
        public ushort Framerate { get; set; }

        [Name("Животное, линия")]
        public string? Animal { get; set; }

        [Name("Пол")]
        public string? Gender { get; set; }

        [Name("Возраст")]
        public uint Age { get; set; } 

        [Name("Дата регистрации сигнала")]
        public DateTime RegDate { get; set; }

        [Name("Канал")]
        public uint Channel { get; set; }

        [Name("Участок сигнала")]
        public string? Signal { get; set; }

        [Name("Разметка участка")]
        public string? Markup { get; set; }
    }
}
