using EGM_Analyzer.DataLoader.OldMurkup;
using EGM_Analyzer.Save;
using EGM_Analyzer.Segmentation;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace EGM_Analyzer
{
    public static class _3th_part
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="originSize">Исходный размер элемента</param>
        /// <param name="control">Элемент, который нужно масштабировать</param>
        /// <param name="currentForm">Текущая форма, которую нужно масштабировать</param>
        /// <param name="originFrameSize">Исходный размер формы</param>
        public static void ResizeControl(System.Drawing.Rectangle originSize, Control control, Form currentForm, System.Drawing.Rectangle originFrameSize)
        {
            float xRatio = (float)(currentForm.Width) / (float)(originFrameSize.Width);
            float yRatio = (float)(currentForm.Height) / (float)(originFrameSize.Height);

            int newX = (int)(xRatio * originSize.Location.X);
            int newY = (int)(yRatio * originSize.Location.Y);

            int newWidth = (int)(xRatio * originSize.Width);
            int newHeight = (int)(yRatio * originSize.Height);

            control.Location = new System.Drawing.Point(newX, newY);
            control.Size = new Size(newWidth, newHeight);
        }

        public static async Task<List<List<Segment>>?> LoadXLSXMarkups(string filepath, ushort samplerate)
        {
            List<List<Segment>> markupsData = new List<List<Segment>>();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage(new FileInfo(filepath)))
            {


                var worksheet = package.Workbook.Worksheets[0];
                // get number of rows and columns in the sheet
                int rows = worksheet.Dimension.Rows; // 20
                int columns = worksheet.Dimension.Columns; // 7

                bool rowFind = false;
                // loop through the worksheet rows and columns
                int i = 1, j = 1;
                for (i = 1; i <= rows - 1; i++)
                {
                    for (j = 1; j <= columns; j++)
                    {
                        object? curvalue = worksheet.Cells[i, j].Value;
                        object? nextvalue = worksheet.Cells[i+1, j].Value;
                        if (curvalue == null || nextvalue == null) continue;
                        string content = curvalue.ToString();
                        string nextcontent = nextvalue.ToString();
                        if (content == "A1" && nextcontent == "1")
                        {
                            rowFind = true;
                            break;
                        }
                    }
                    if (rowFind)
                        break;
                }
                //Заполнение каналов
                int colList = j;
                object? value = worksheet.Cells[i + 1, colList].Value;
                while (value != null)
                {
                    markupsData.Add(new List<Segment>());
                    colList++;
                    value = worksheet.Cells[i + 1, colList].Value;
                }
                if (markupsData.Count == 0) return null;
                int targetRow = i + 2;
                for (int numChannels = 0; numChannels < markupsData.Count; numChannels++)
                {
                    int startRow = targetRow;
                    object? microsecVal = worksheet.Cells[startRow, j + numChannels].Value;
                    while (microsecVal != null)
                    {
                        double centermark = (double)(ulong.Parse(microsecVal.ToString())) / 1_000_000;
                        markupsData[numChannels].Add(new Segment((byte)numChannels, SegmentType.AT,
                            (ulong)(samplerate * (centermark - 0.005)),
                            (ulong)(samplerate * (centermark + 0.005)), 
                            SegmentationAgent.AnalyticalAlgorithm, centermark));
                        startRow++;
                        microsecVal = worksheet.Cells[startRow, j + numChannels].Value;
                    }
                }


                bool fds = false;

                int sss = 10 + 5;

                
            }

            return markupsData;
        }

        public static async Task<List<List<Segment>>?> LoadNewFormatMarkup(string filepath, ushort samplerate)
        {
            JSONSegments? markupsData = null;
            try
            {
                using (FileStream fs = new FileStream(filepath, FileMode.Open))
                {
                    markupsData = await JsonSerializer.DeserializeAsync<JSONSegments>(fs);
                }
                if (markupsData == null)
                    throw new ArgumentException("Отсутствуют данные разметки");
                if (markupsData.Segments == null)
                    throw new ArgumentException("Ошибка разбиения разметки на сегменты");
                if (markupsData.SampleRate != samplerate)
                    throw new ArgumentException("Ошибка! Несовпадение частот сигнала и разметки!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " Запуск открытия старого формата.", "Открытие файла сегментации ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
            }
            return markupsData.Segments;
        }

        public static SegmentationAgent SegmentationAgentConvert(string creationStage)
        {
            switch (creationStage)
            {
                case "human_edit":
                    return SegmentationAgent.Human;
                case "peak_search":
                    return SegmentationAgent.AnalyticalAlgorithm;
                default:
                    return SegmentationAgent.AI_Segmentator;
            }
        }

        public static async Task<List<List<Segment>>?> LoadOldFormatMarkup(string filepath, ushort samplerate)
        {
            JSONMurkupsObject? resultsMarkups = null;
            try
            {
                using (FileStream fs = new FileStream(filepath, FileMode.Open))
                {
                    resultsMarkups = await JsonSerializer.DeserializeAsync<JSONMurkupsObject>(fs);
                }
                if (resultsMarkups == null)
                    throw new ArgumentException("Отсутствуют данные разметки");
                if (resultsMarkups.peaks == null)
                    throw new ArgumentException("Не удалось получить сегментацию из загруженного файла");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,
                    "Некорректно сохранённый файл или неверный формат хранения!",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            List<List<Segment>> markupsData = new List<List<Segment>>();
            for (int i = 0; i < resultsMarkups.peaks.Count; i++)
            {
                markupsData.Add(MurkupsToSegments((byte)i, resultsMarkups.peaks[i], samplerate));
            }
            return markupsData;
        }

        private static List<Segment> MurkupsToSegments(byte channelNumber, Murkup[] markups, ushort samplerate)
        {
            List<Segment> segments = [];
            // Коэфициент 0.0025 = 5 / 1000 / 2; 5 - количество мс для участка, 1000 - количество мс в секунде, 2 - половина интервала
            ushort halfComplexSamples = (ushort)(0.0025 * samplerate); //(ushort)((1.0 / (50.0 / samplerate)) / 2 + 1); //??

            for (int i = 0; i < markups.Length; i++)
            {
                
/*                if (markups[i].search_segment != null) {
                    if (markups[i].position > 0)
                    {
                        segments.Add(new Segment(channelNumber, SegmentType.AT,
                            (ulong)markups[i].position - halfComplexSamples,
                            (ulong)markups[i].position + halfComplexSamples,
                            SegmentationAgentConvert(markups[i].creation_stage), (ulong)markups[i].position));
                    }
                    segments.Add(new Segment(channelNumber, SegmentType.AT,
                        (ulong)markups[i].search_segment[0] - halfComplexSamples, 
                        (ulong)markups[i].search_segment[1] + halfComplexSamples,
                         SegmentationAgentConvert(markups[i].creation_stage),
                         (ulong)markups[i].position > 0 ? (ulong)markups[i].position : null));
                    continue;
                }*/
                if (markups[i].position > 0)
                {
                    segments.Add(new Segment(channelNumber, SegmentType.AT,
                        (ulong)(markups[i].position - halfComplexSamples * 3.5), 
                        (ulong)(markups[i].position + halfComplexSamples * 2),
                        SegmentationAgentConvert(markups[i].creation_stage), (ulong)markups[i].position));
                }
                
            }
            return segments;
        }
    }
}
