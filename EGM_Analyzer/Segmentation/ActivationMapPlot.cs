using EGM_Analyzer.Analyzers;
using EGM_Analyzer.Analyzers.SegmentationTools;
using EGM_Analyzer.DataLoader;
using EGM_Analyzer.EventsArgs;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot.WindowsForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EGM_Analyzer.Segmentation
{
    public class ActivationMapPlot
    {
        PlotView _plotView;
        HeatMapSeries _heatMapSeries;
        PlotModel _plotModel;
        /// <summary>
        /// Поставщик данных для мгновенной отрисовки
        /// </summary>
        IDataLoader _dataSuplier;



        public ActivationMapPlot(PlotView plotView, Tuple<byte, byte> matrixSize, IDataLoader dataSuplier) 
        {
            _dataSuplier = dataSuplier;
            _dataSuplier.CurrentSegmentChanged += UpdateActivationPlot;
            _plotView = plotView;

            _plotModel = new PlotModel { Title = "Activation Map" };

            // Создаем оси
            _plotModel.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Bottom,
            });

            _plotModel.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
            });
            var customPalette = new OxyPalette(new[]
            {
                OxyColors.Blue,
                OxyColors.Green,
                OxyColors.Yellow
            });
            //А какие максимумы и минимумы
            _plotModel.Axes.Add(new LinearColorAxis
            {
                Position = AxisPosition.Right,
                Minimum = 0,
                Maximum = 10,
                Palette = customPalette
            });

            double[,] data = new double[matrixSize.Item1, matrixSize.Item2];


            _heatMapSeries = new HeatMapSeries()
            {
                X0 = 0,
                X1 = matrixSize.Item1,
                Y0 = 0,
                Y1 = matrixSize.Item2,
                Interpolate = true,
                Data = data
            };

            _plotModel.Series.Add(_heatMapSeries);
            _plotView.Model = _plotModel;

        }

        private void UpdateActivationPlot(object? sender, SegmentEventArgs e)
        {
            if (_heatMapSeries.X1 * _heatMapSeries .Y1 == e.DataMatrix.Length)
            {
                double[,] data = new double[(int)_heatMapSeries.Y1, (int)_heatMapSeries.X1];
                for (int i = 0; i < e.DataMatrix.Length; i++)
                {
                    data[i / data.GetLength(0), i % data.GetLength(1)] = e.DataMatrix[i];
                }
                _heatMapSeries.Data = data;
                _plotView.InvalidatePlot(true);
            }
        }

        public PlotModel CreateHeatMap()
        {
            var model = new PlotModel { Title = "Карта активаций" };

            // Создаем оси
            model.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Bottom,
            });

            model.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
            });

            // Создание кастомной палитры
            var customPalette = new OxyPalette(new[]
            {
                OxyColors.Blue,
                OxyColors.Green,
                OxyColors.Yellow
            });

            model.Axes.Add(new LinearColorAxis
            {
                Position = AxisPosition.Right,
                Minimum = 0,
                Maximum = 10,
                Palette = customPalette
            });

            // Создаем данные для heat map
            int width = 10;
            int height = 10;
            double[,] data = new double[width, height];

            // Заполняем данные (пример: 2D гауссово распределение)
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    data[x, y] = new Random().Next(0, 5);
                }
            }

            // Создаем heat map series
            var heatMapSeries = new HeatMapSeries
            {
                X0 = 0,
                X1 = width,
                Y0 = 0,
                Y1 = height,
                Data = data,
                Interpolate = true // Интерполяция для плавного отображения
            };

            model.Series.Add(heatMapSeries);
            return model;
        }
    }
}
