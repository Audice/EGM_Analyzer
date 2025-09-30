using EGM_Analyzer.Analyzers.SegmentationTools;
using EGM_Analyzer.EventsArgs;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot.WindowsForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EGM_Analyzer.Segmentation
{
    /// <summary>
    /// Класс-обработчик, который позволяет выполнить навигацию по сигналу
    /// на интересующее событие
    /// </summary>
    public class NavigationPlot
    {
        private IDataSuplier? _dataSuplier;

        private PlotView _plotView;

        private PlotModel _model;

        private LineSeries _signalSeries;

        private ComboBox _channelsList;

        private int _selectedIndex = 0;

        public EventHandler<TargetPositionEventArgs> TargetPositionUpdated;

        private void OnTargetPositionUpdate(TargetPositionEventArgs e)
        {
            TargetPositionUpdated?.Invoke(this, e);
        }

        public NavigationPlot(PlotView plotView, ComboBox channelsList)
        {
            _plotView = plotView;
            _channelsList = channelsList;
            _channelsList.SelectedIndexChanged += channelChanged;

            _model = new OxyPlot.PlotModel();
            _plotView.Model = _model;

            _signalSeries = new OxyPlot.Series.LineSeries()
            {
                Color = OxyPlot.OxyColors.Blue,
                StrokeThickness = 2,
            };

            _model.Series.Add(_signalSeries);
            var controller = new PlotController();
            // Или оставляем только нужные взаимодействия (например, только zoom)
            controller.UnbindAll();
            controller.BindMouseDown(OxyMouseButton.Left, new DelegatePlotCommand<OxyMouseDownEventArgs>((view, controller, args) =>
            {
                if (_dataSuplier != null)
                {
                    var position = args.Position;
                    OxyPlot.ElementCollection<OxyPlot.Axes.Axis> axisList = _model.Axes;
                    Axis? xAxis = axisList.FirstOrDefault(ax => ax.Position == AxisPosition.Bottom);
                    Axis? yAxis = axisList.FirstOrDefault(ax => ax.Position == AxisPosition.Left);
                    var dataPoint = OxyPlot.Axes.Axis.InverseTransform(new ScreenPoint(position.X, position.Y), xAxis, yAxis);
                    _dataSuplier.UpdateTargetPosition(dataPoint.X >= 0 ? (ulong)dataPoint.X : 0);
                }
            }));

            _plotView.Controller = controller;
        }



        public void UpdateNavigationPlot()
        {

        }

        private void PrintNavigation(int selectedIndex)
        {
            _signalSeries.Points.Clear();
            var data = _dataSuplier.AnalyzedIntervals[selectedIndex];
            double max = data[0].Item2;
            double min = data[0].Item2;

            for (int i = 0; i < data.Count; i++)
            {
                _signalSeries.Points.Add(new OxyPlot.DataPoint(i, data[i].Item2));
                if (max < data[i].Item2) max = data[i].Item2;
                if (min > data[i].Item2) min = data[i].Item2;
            }

            var leftAxes = _model.Axes[1];

            double offset = double.Max(Math.Abs(max), (Math.Abs(min)));
            leftAxes.AbsoluteMaximum = max + offset * 0.5;
            leftAxes.AbsoluteMinimum = min - offset * 0.5;
            leftAxes.Maximum = max + offset * 0.5;
            leftAxes.Minimum = min - offset * 0.5;

            _plotView.InvalidatePlot(true);
        }

        public void SetDataSuplier(IDataSuplier dataSuplier)
        {
            _dataSuplier = dataSuplier;
            if (_selectedIndex >= 0 && _selectedIndex < _dataSuplier.AnalyzedIntervals.Count)
                PrintNavigation(_selectedIndex);
        }

        private void channelChanged(object? sender, EventArgs e)
        {
            if (_dataSuplier == null) return;
            _selectedIndex = (sender as ComboBox).SelectedIndex;
            if (_selectedIndex >= 0 && _selectedIndex < _dataSuplier.AnalyzedIntervals.Count)
                PrintNavigation(_selectedIndex);
        }
    }
}
