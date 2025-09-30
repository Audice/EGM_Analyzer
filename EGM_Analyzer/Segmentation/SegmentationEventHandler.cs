using OxyPlot.Axes;
using OxyPlot;
using OxyPlot.WindowsForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OxyPlot.Series;
using static System.Windows.Forms.LinkLabel;
using OxyPlot.Annotations;
using System.Reflection;
using EGM_Analyzer.DataLoader;
using System.Threading.Channels;
using ParseData.Electrogram;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Net;
using EGM_Analyzer.Analyzers.SegmentationTools;

namespace EGM_Analyzer.Segmentation
{
    public class SegmentationEventHandler : IDisposable
    {
        // В секундах
        private const double MAX_SEGMENT_WIDTH = 0.09;
        private const double MIN_SEGMENT_WIDTH = 0.002;
        private const int STEP_SEGMENT_WIDTH = 1000;

        /// <summary>
        /// Максимальный временной промежуток, на который пролистываем сигнал
        /// </summary>
        private const double MAX_SCROLL_STEP = 2.0;
        /// <summary>
        /// Минимальный временной промежуток, на который пролистываем сигнал
        /// </summary>
        private const double MIN_SCROLL_STEP = 0.2;
        /// <summary>
        /// Шаг пролистивания слайдером
        /// </summary>
        private const int STEP_SCROLL_STEP = 100;

        /// <summary>
        /// Переменная для отслеживания координаты X мышки
        /// </summary>
        private int _xCoord = -1;
        /// <summary>
        /// Переменная для отслеживания координаты Y мышки
        /// </summary>
        private int _yCoord = -1;

        public byte Channel { get; private set; }


        private static Dictionary<SegmentType, OxyPlot.OxyColor> _segmentsColor = new Dictionary<SegmentType, OxyColor>()
        {
            {SegmentType.AT,  OxyPlot.OxyColors.Red},
            {SegmentType.RT,  OxyPlot.OxyColors.Green },
            {SegmentType.Artifact,  OxyPlot.OxyColors.Blue },
            {SegmentType.Noise,  OxyPlot.OxyColors.Gray }
        };
        private PlotView _plotView;

        private const byte TransparentLevel = 80;

        private PlotModel _model;

        private LineSeries _signalSeries;

        // Отображение точек
        private ScatterSeries _scatterSeries;

        private SegmentType _currentSegmentType = SegmentType.AT;

        private IDataLoader? _signalSupplier;

        private IDataSuplier? _dataSuplier;

        private ComboBox _channels;
        private Label _sampleRateLabel;
        private TrackBar _atWidthTrackbar;
        private TrackBar _stepTrackbar;

        /// <summary>
        /// Ширина сегмента в миллисекундах
        /// </summary>
        public double SegmentWidth
        {
            get; private set;
        } = MIN_SEGMENT_WIDTH;

        public double ScrollStep 
        { 
            get; private set; 
        } = MIN_SCROLL_STEP;

        /// <summary>
        /// Максиму текущего сигнала
        /// </summary>
        private double _signalsMax = 0;
        /// <summary>
        /// Минимум текущего сигнала
        /// </summary>
        private double _signalsMin = 0;

        public EventHandler OnWidthSegmentChange;
        public EventHandler OnScrollStepChange;

        RectangleAnnotation? _currentAnnotation;

        HScrollBar _hScrollBar;



        public SegmentationEventHandler(PlotView plot, ComboBox comboBox, Label sampleRateLabel, 
            TrackBar? widthTrackbar, TrackBar? stepTrackbar)
        {
            if (stepTrackbar != null)
            {
                _stepTrackbar = stepTrackbar;
                _stepTrackbar.Minimum = (int)(MIN_SCROLL_STEP * STEP_SCROLL_STEP);
                _stepTrackbar.Maximum = (int)(MAX_SCROLL_STEP * STEP_SCROLL_STEP);
                _stepTrackbar.Value = (int)(ScrollStep * STEP_SCROLL_STEP);
                _stepTrackbar.ValueChanged += ScrollValueUpdate;
            }


            _channels = comboBox;
            _channels.SelectedIndexChanged += СhannelsChange;
            _sampleRateLabel = sampleRateLabel;

            //Обновление отмечаемого пробелом сегмента
            if (widthTrackbar != null)
            {
                _atWidthTrackbar = widthTrackbar;
                _atWidthTrackbar.ValueChanged += SegmentWidthChange;
                _atWidthTrackbar.Maximum = (int)(MAX_SEGMENT_WIDTH * STEP_SEGMENT_WIDTH);
                _atWidthTrackbar.Minimum = (int)(MIN_SEGMENT_WIDTH * STEP_SEGMENT_WIDTH);
                _atWidthTrackbar.Value = (int)(SegmentWidth * STEP_SEGMENT_WIDTH);
            }


            _plotView = plot;
            _plotView.MouseDown += SetStartMarkup;
            _plotView.MouseUp += SetEndMarkup;
            _plotView.MouseMove += VisualizeMarkupArea;
            _plotView.KeyDown += SpaceMark;

            _model = new OxyPlot.PlotModel();

            _model.Axes.Add(new LinearAxis() { Position = AxisPosition.Bottom, 
                AbsoluteMinimum = 0, Minimum = 0, 
                MinimumRange = 300, Title = "Time, sec."
            });
            _model.Axes.Add(new LinearAxis() { Position = AxisPosition.Left, 
                AbsoluteMaximum = 20000, AbsoluteMinimum = -20000, Title = "Voltage, µV."
            });


            _model.Axes[0].KeyDown += KeyboardScroll;


            _model.TrackerChanged += HideTracker;
            _signalSeries = new OxyPlot.Series.LineSeries()
            {
                Color = OxyPlot.OxyColors.Blue,
                StrokeThickness = 2,
            };

            _scatterSeries = new ScatterSeries()
            {
                MarkerType = MarkerType.Circle,
                MarkerSize = 5,
                MarkerStroke = OxyColors.Red,
                MarkerFill = OxyColors.Red
            };

            _model.Series.Add(_scatterSeries);
            _model.Series.Add(_signalSeries);
            _plotView.Model = _model;

            _plotView.MouseMove += MouseMove;

            _plotView.Enabled = false;


            /// Отслеживать обновление границ отображаемой области
            //???_model.TrackerChanged
        }

        public void InitScrollBar(HScrollBar hScrollBar)
        {
            _hScrollBar = hScrollBar;
            _hScrollBar.ValueChanged += UpdateSignalVisualisation;
        }

        private void UpdateSignalVisualisation(object? sender, EventArgs e)
        {
            if (_signalSupplier == null) return;

            HScrollBar hScrollBar = sender as HScrollBar;

            if (hScrollBar == null) return;

            var bottomAxes = _model.Axes[0]; //ИЗМЕНЕНИЕ всё равно на 2 тысячи... при большом формате
            bottomAxes.IsPanEnabled = false; //Иначе глючит прокрутка!

            double delta = bottomAxes.ActualMaximum - bottomAxes.ActualMinimum;

            double newMin = hScrollBar.Value / _signalSupplier.SampleRate;
            double newMax = newMin + delta;
            if (newMin + delta > bottomAxes.AbsoluteMaximum)
            {
                newMin = bottomAxes.AbsoluteMaximum - delta;
                newMax = bottomAxes.AbsoluteMaximum;
            }
            



            //bottomAxes.Minimum = newMin;
            //bottomAxes.Maximum = newMax;

            bottomAxes.Zoom(newMin, newMax);
            bottomAxes.IsPanEnabled = true; //Иначе глючит прокрутка!
            _plotView.InvalidatePlot(true);
        }

        private void UpdateScrollBarBorder(int minimum, int maximum)
        {
            if (_hScrollBar == null) return;
            _hScrollBar.Maximum = maximum;
            _hScrollBar.Minimum = minimum;
        }

        private void ScrollValueUpdate(object? sender, EventArgs e)
        {
            TrackBar? trackBar = sender as TrackBar;
            if (trackBar == null) return;
            ScrollStep = (double)trackBar.Value / STEP_SCROLL_STEP;
            OnScrollStepChange?.Invoke(this, e);
            if (_plotView != null)
                _plotView.Focus();
        }

        private void KeyboardScroll(object? sender, OxyKeyEventArgs e)
        {
            if (e.Key == OxyKey.Right || e.Key == OxyKey.Left)
            {
                var bottomAxes = _model.Axes[0]; //ИЗМЕНЕНИЕ всё равно на 2 тысячи... при большом формате

                bottomAxes.IsPanEnabled = false; //Иначе глючит прокрутка!

                double newMin = bottomAxes.ActualMinimum + ScrollStep * (e.Key == OxyKey.Right ? 1 : -1);
                double newMax = bottomAxes.ActualMaximum + ScrollStep * (e.Key == OxyKey.Right ? 1 : -1);

                //bottomAxes.Minimum = newMin;
                //bottomAxes.Maximum = newMax;

                bottomAxes.Zoom(newMin, newMax);
                /*                bottomAxes.MajorStep = ScrollStep;
                                bottomAxes.MinimumMajorStep = ScrollStep;
                                bottomAxes.MinimumMinorStep = ScrollStep;*/

                //bottomAxes.Reset();
                bottomAxes.IsPanEnabled = true; //Иначе глючит прокрутка!
                _plotView.InvalidatePlot(true);
                int s = 0;
            }
        }
        /// <summary>
        /// Обновление ширины автоматически расставляемого курсора
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SegmentWidthChange(object? sender, EventArgs e)
        {
            TrackBar? trackBar = sender as TrackBar;
            if (trackBar == null) return;
            SegmentWidth = (double)trackBar.Value / STEP_SEGMENT_WIDTH;
            OnWidthSegmentChange?.Invoke(sender, new EventArgs());
            if (_plotView != null)
                _plotView.Focus();
        }
        /// <summary>
        /// Обновление полей, отслеживающих положение курсора мыши
        /// </summary>
        /// <param name="sender">Курсор</param>
        /// <param name="e">Координаты курсора</param>
        private void MouseMove(object? sender, MouseEventArgs e)
        {
            _xCoord = e.X;
            _yCoord = e.X;
        }
        /// <summary>
        /// Преобразование аннотации на графике сигнала в сегмент в терминах сэмплов
        /// </summary>
        /// <param name="annotation">Аннотация на графике</param>
        /// <returns>Сегмент в терминах сэмплов сигнала</returns>
        /// <exception cref="ArgumentException">Отсутствует поставщик сигнала</exception>
        private Segment PlotCoordToSegment(RectangleAnnotation annotation)
        {
            if (_signalSupplier == null)
                throw new ArgumentException("PlotAnnotationToSegment: Не установлен поставщик данных для отрисовки сигнала!");
            return new Segment(Channel, _currentSegmentType,
                (ulong)(annotation.MinimumX * _signalSupplier.SampleRate),
                (ulong)(annotation.MaximumX * _signalSupplier.SampleRate),
                SegmentationAgent.Human, null);
        }

        private Segment PlotCoordToSegment(double left, double right, SegmentType segmentType)
        {
            if (_signalSupplier == null)
                throw new ArgumentException("PlotAnnotationToSegment: Не установлен поставщик данных для отрисовки сигнала!");
            return new Segment(Channel, segmentType,
                (ulong)(left * _signalSupplier.SampleRate),
                (ulong)(right * _signalSupplier.SampleRate),
                SegmentationAgent.Human, null);
        }

        private double SampleToPlotCoord(ulong sample)
        {
            if (_signalSupplier == null)
                throw new ArgumentNullException("SampleToPlotCoord: поставщик данных не установлен");
            return (double)sample / _signalSupplier.SampleRate;
        }
        /// <summary>
        /// Преобразование времени в номер отсчёта сигнала
        /// </summary>
        /// <param name="plotCoord">Время на графике</param>
        /// <returns>Номер сэмпла сигнала, соответствующий времени</returns>
        /// <exception cref="ArgumentNullException">Отсутствует поставщик данных</exception>
        private ulong PlotCoordToSample(double plotCoord)
        {
            if (_signalSupplier == null)
                throw new ArgumentNullException("PlotCoordToSample: поставщик данных не установлен");
            return (ulong)(plotCoord * _signalSupplier.SampleRate);
        }

        /// <summary>
        /// Добавление разметки по указателю мыши при нажатию кнопки пробел
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="ArgumentException"></exception>
        private void SpaceMark(object? sender, KeyEventArgs e)
        {
            if (_signalSupplier == null)
            {
                MessageBox.Show("Отсутствует сигнал для разметки данных. Сначала загрузите электрограмму.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (_signalSupplier.SegmentsHandler == null)
            {
                throw new ArgumentException("SpaceMark: хранилище разметки не создано!");
            }

            if (e.KeyCode == Keys.Space)
            {
                DataPoint _actionsPoint = GetRealPoint(_xCoord, _yCoord);
                RectangleAnnotation annotation = BuildAnnotation(_actionsPoint);
                if (_signalSupplier.SegmentsHandler.AppendSegment(PlotCoordToSegment(annotation)))
                {
                    _model.Annotations.Add(annotation);
                    _plotView.InvalidatePlot(true);
                }
                else
                {
                    MessageBox.Show("Комплексы налезают друг на друга.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private RectangleAnnotation BuildAnnotation(DataPoint centerPoint)
        {
            var bottomAxes = _model.Axes[0];
            double halfSegmentWidth = SegmentWidth / 2;

            if ((centerPoint.X - halfSegmentWidth) <= bottomAxes.AbsoluteMinimum)
                throw new ArgumentNullException("BuildAnnotation: минимальная метка за границей графика");
            if ((centerPoint.X + halfSegmentWidth) >= bottomAxes.AbsoluteMaximum)
                throw new ArgumentNullException("BuildAnnotation: максимальная метка за границей графика");

            return new RectangleAnnotation
            {
                MinimumX = centerPoint.X - halfSegmentWidth,
                MaximumX = centerPoint.X + halfSegmentWidth,
                MinimumY = _signalsMin,
                MaximumY = _signalsMax,
                Fill = OxyColor.FromAColor(TransparentLevel, _segmentsColor[_currentSegmentType]),
                Stroke = _segmentsColor[_currentSegmentType],
                StrokeThickness = 2
            };
        }
        private RectangleAnnotation BuildAnnotation(double startPoint, double endPoint)
        {
            var bottomAxes = _model.Axes[0];
            if (startPoint < bottomAxes.AbsoluteMinimum)
            {
                //throw new ArgumentNullException("BuildAnnotation: минимальная метка за границей графика");
                MessageBox.Show("BuildAnnotation: минимальная метка за границей графика", "Проблема в установке метки!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
                
            if (endPoint > bottomAxes.AbsoluteMaximum)
            {
                //throw new ArgumentNullException("BuildAnnotation: максимальная метка за границей графика");
                MessageBox.Show("BuildAnnotation: максимальная метка за границей графика", "Проблема в установке метки!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
                

            return new RectangleAnnotation
            {
                MinimumX = startPoint,
                MaximumX = endPoint,
                MinimumY = _signalsMin,
                MaximumY = _signalsMax,
                Fill = OxyColor.FromAColor(TransparentLevel, _segmentsColor[_currentSegmentType]),
                Stroke = _segmentsColor[_currentSegmentType],
                StrokeThickness = 2
            };
        }
        private RectangleAnnotation BuildAnnotation(ulong startSample, ulong endSample, SegmentType segmentType)
        {
            if (_signalSupplier == null)
                throw new ArgumentNullException("BuildAnnotation: отсутствует поставщик данных");

            var bottomAxes = _model.Axes[0];

            double minimumX = (double)startSample / _signalSupplier.SampleRate;
            if (minimumX < bottomAxes.AbsoluteMinimum)
                throw new ArgumentNullException("BuildAnnotation: минимальная метка за границей графика");
            double maximumX = (double)endSample / _signalSupplier.SampleRate;
            if (maximumX > bottomAxes.AbsoluteMaximum)
                throw new ArgumentNullException("BuildAnnotation: максимальная метка за границей графика");

            return new RectangleAnnotation
            {
                MinimumX = minimumX,
                MaximumX = maximumX,
                MinimumY = _signalsMin,
                MaximumY = _signalsMax,
                Fill = OxyColor.FromAColor(TransparentLevel, _segmentsColor[segmentType]),
                Stroke = _segmentsColor[segmentType],
                StrokeThickness = 2
            };
        }
        private RectangleAnnotation BuildAnnotation(double startPoint, double endPoint, SegmentType segmentType)
        {
            return new RectangleAnnotation
            {
                MinimumX = (ulong)startPoint,
                MaximumX = (ulong)endPoint,
                MinimumY = _signalsMin,
                MaximumY = _signalsMax,
                Fill = OxyColor.FromAColor(TransparentLevel, _segmentsColor[segmentType]),
                Stroke = _segmentsColor[segmentType],
                StrokeThickness = 2
            };
        }
        private void HideTracker(object? sender, TrackerEventArgs e)
        {
            _plotView.HideTracker();
        }
        private void VisualizeMarkupArea(object? sender, MouseEventArgs e)
        {
            if (_currentAnnotation == null) return;
            if (e.Button == MouseButtons.Left)
            {
                DataPoint _actionsPoint = GetRealPoint(e.X, e.Y);
                _currentAnnotation.MaximumX = _actionsPoint.X;
                _model.InvalidatePlot(true);
            }
        }
        private void SetStartMarkup(object? sender, MouseEventArgs e)
        {
            if (_signalSupplier == null)
            {
                MessageBox.Show("Отсутствует сигнал для разметки данных. Сначала загрузите электрограмму.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (_signalSupplier.SegmentsHandler == null)
            {
                throw new ArgumentException("SpaceMark: хранилище разметки не создано!");
            }
            if (e.Button == MouseButtons.Left && _currentAnnotation == null)
            {
                DataPoint _actionsPoint = GetRealPoint(e.X, e.Y);
                _currentAnnotation = BuildAnnotation(_actionsPoint.X, _actionsPoint.X);
                _model.Annotations.Add(_currentAnnotation);
                _plotView.InvalidatePlot(true);
            }
            if (e.Button == MouseButtons.Right)
            {
                DataPoint _actionsPoint = GetRealPoint(e.X, e.Y);
                var ann = _model.Annotations;

                foreach (RectangleAnnotation annot in ann)
                {
                    double max = Math.Max(annot.MinimumX, annot.MaximumX);
                    double min = Math.Min(annot.MinimumX, annot.MaximumX);
                    if (_actionsPoint.X >= min && _actionsPoint.X <= max)
                    {

                        if (_signalSupplier.SegmentsHandler.DeleteSegment(Channel, 
                            (ulong)(min * _signalSupplier.SampleRate)))
                        {
                            ann.Remove(annot);
                        }
                        else
                        {
                            MessageBox.Show("Ошибка удаления. Сообщите разработчику", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        break;
                    }
                }
            }
            _plotView.InvalidatePlot(true);
        }
        private DataPoint GetRealPoint(double x, double y)
        {
            OxyPlot.ElementCollection<OxyPlot.Axes.Axis> axisList = _model.Axes;
            Axis? xAxis = axisList.FirstOrDefault(ax => ax.Position == AxisPosition.Bottom);
            Axis? yAxis = axisList.FirstOrDefault(ax => ax.Position == AxisPosition.Left);
            return OxyPlot.Axes.Axis.InverseTransform(new ScreenPoint(x, y), xAxis, yAxis);
        }

        private void SetEndMarkup(object? sender, MouseEventArgs e)
        {
            if (_signalSupplier == null)
            {
                MessageBox.Show("SetEndMarkup: Отсутствует сигнал для разметки данных. Сначала загрузите электрограмму.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _currentAnnotation = null;
                return;
            }
            if (_signalSupplier.SegmentsHandler == null)
            {
                throw new ArgumentException("SpaceMark: Хранилище разметки не создано!");
            }
            if (e.Button == MouseButtons.Left && _currentAnnotation != null)
            {
                DataPoint _actionsPoint = GetRealPoint(e.X, e.Y);
                if (Math.Abs(_currentAnnotation.MinimumX - _currentAnnotation.MaximumX) 
                    > MIN_SEGMENT_WIDTH)
                {
                    double left = Math.Min(_currentAnnotation.MinimumX, _currentAnnotation.MaximumX);
                    double right = Math.Max(_currentAnnotation.MinimumX, _currentAnnotation.MaximumX);

                    if (!_signalSupplier.SegmentsHandler.AppendSegment(PlotCoordToSegment(left, right, _currentSegmentType)))
                    {
                        MessageBox.Show("Комплексы налезают друг на друга.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        _model.Annotations.Remove(_currentAnnotation);
                    }
                }
                else
                {
                    MessageBox.Show("Слишком короткое выделение комплекса.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    _model.Annotations.Remove(_currentAnnotation);
                }

                _currentAnnotation = null;
                _plotView.InvalidatePlot(true);
            }
        }

        /// <summary>
        /// Установка поставщика данных
        /// </summary>
        /// <param name="supplier">Поставщик сигнала и разметки</param>
        public void SetDataSupplier(IDataLoader supplier)
        {
            if (supplier == null)
                throw new ArgumentNullException("SetDataSupplier: поставщик данных не установлен");
            //Сохраняем поставщика
            _signalSupplier = supplier;
            // Обновляем информацию о частоте
            _sampleRateLabel.Text = _signalSupplier.SampleRate.ToString();
            //Очищаем список каналов предыдущего сигнала
            _channels.Items.Clear();
            //Заполняем новый список каналов
            for (int i = 0; i < _signalSupplier.Data.Count; i++)
                _channels.Items.Add(i);
            // Сбрасываем отображаемый индекс, тем самым запустив событие обновление графика
            _channels.SelectedIndex = 0;
            //Включаем график на отслуживание событий
            _plotView.Enabled = true;
        }

        public void SetDataSuplier(IDataSuplier dataSuplier)
        {
            _dataSuplier = dataSuplier;
        }

        /// <summary>
        /// Обновление графика и номера канала при смене канала
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="Exception"></exception>
        private void СhannelsChange(object? sender, EventArgs e)
        {
            if (_signalSupplier == null)
            {
                throw new Exception("СhannelsChange: обновление графика без поставщика данных невозможно!");
            }
            ComboBox? channel = sender as ComboBox;
            if (channel == null) return;
            // Обновление сигнала на графике 
            PrintSignal((byte)channel.SelectedIndex);
            //Установка номера текущего канала
            Channel = (byte)channel.SelectedIndex;
        }

        /// <summary>
        /// Печать чигнала на графике и подготовка осей
        /// </summary>
        /// <param name="channel">Номер интересующего канала</param>
        /// <exception cref="ArgumentNullException">Отсутствует поставщик данных</exception>
        private void PrintSignal(byte channel)
        {
            if (_signalSupplier == null)
                throw new ArgumentNullException("PrintSignal: обновление графика без поставщика данных невозможно!");
            if (_signalSupplier.SegmentsHandler == null)
                throw new ArgumentNullException("PrintSignal: обновление сегментации без поставщика данных невозможно!");
            
            UpdateScrollBarBorder(0, _signalSupplier.Data[channel].Count);

            List<double> ys = _signalSupplier.Data[channel].ToList();
            var bottomAxes = _model.Axes[0];
            var leftAxes = _model.Axes[1];
            _signalsMax = ys.Max();
            _signalsMin = ys.Min();
            if (_signalsMax == _signalsMin)
            {
                _signalsMax += 1;
                _signalsMin -= 1;
            }
            bottomAxes.Maximum = SampleToPlotCoord(10000);
            bottomAxes.AbsoluteMinimum = 0;
            bottomAxes.AbsoluteMaximum = SampleToPlotCoord((ulong)(ys.Count-1));
            bottomAxes.Minimum = 0;
            bottomAxes.MinimumRange = SampleToPlotCoord(200);
            


            leftAxes.AbsoluteMaximum = _signalsMax;
            leftAxes.AbsoluteMinimum = _signalsMin;
            _signalSeries.Points.Clear();
            for (int i = 0; i < ys.Count; i++)
                _signalSeries.Points.Add(new OxyPlot.DataPoint(SampleToPlotCoord((ulong)i), ys[i]));
            var segmentation = _signalSupplier.SegmentsHandler.GetSegments(channel);
            _model.Annotations.Clear();
            foreach (var segment in segmentation)
            {
                try
                {
                    _model.Annotations.Add(BuildAnnotation(segment.StartMark, segment.EndMark, segment.Type));
                }
                catch (Exception ex)
                {
                    string msg = ex.Message;
                }
            }
                


            _plotView.InvalidatePlot(true);
            _plotView.Focus();
        }

         

        public void DrawPoints(List<ulong> points, byte channel)
        {
            _scatterSeries.Points.Clear();

            List<double> ys = _signalSupplier.Data[channel].ToList();
            for (int i = 0; i < points.Count; i++)
                _scatterSeries.Points.Add(new ScatterPoint(SampleToPlotCoord(points[i]), ys[(int)points[i]]));

            _plotView.InvalidatePlot(true);
            _plotView.Focus();
        }

        public void SetSegmentationType(SegmentType segmentType)
        {
            _currentSegmentType = segmentType;
            _plotView.Focus();
        }
        public void Dispose()
        {
            _plotView.MouseDown -= SetStartMarkup;
            _plotView.MouseUp -= SetEndMarkup;
            //_plotView.MouseMove -= VisualizeMarkupArea;
            _model.TrackerChanged -= HideTracker;
        }

        public void DrawTestSignal()
        {
            for (int i = 0; i < 10000; i++)
            {
                _signalSeries.Points.Add(new OxyPlot.DataPoint(i, Math.Sin(Math.PI / i)));
            }
            _model.Annotations.Clear();
            _plotView.InvalidatePlot(true);
            _plotView.Enabled = true;
        }

        public void ClearVisibleSegmentation()
        {
            if (_signalSupplier == null) return;

            var bottomAxes = _model.Axes[0];
            double left = bottomAxes.ActualMinimum;
            double right = bottomAxes.ActualMaximum;
            var ann = _model.Annotations;

            for (int i = ann.Count - 1; i >= 0; i--) {
                RectangleAnnotation? annot = ann[i] as RectangleAnnotation;
                if (annot == null) continue;
                double max = Math.Max(annot.MinimumX, annot.MaximumX);
                double min = Math.Min(annot.MinimumX, annot.MaximumX);
                if (right >= max && left <= min)
                {
                    if (_signalSupplier.SegmentsHandler.DeleteSegment(Channel, PlotCoordToSample(min)))
                        ann.Remove(annot);
                    else
                        MessageBox.Show("Ошибка удаления. Сообщите разработчику", "Ошибка", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            _plotView.InvalidatePlot(true);
            _plotView.Focus();
        }

        /// <summary>
        /// Пометить сигнал как шумный. ВНИМАНИЕ! Все остальные сегменты будут удалены!
        /// </summary>
        public void SetNoiseChannel()
        {
            if (_signalSupplier == null)
                throw new ArgumentNullException("SetNoiseChannel: поставщик сигнала не установлен");
            if (_signalSupplier.SegmentsHandler == null)
            {
                throw new ArgumentNullException("SetNoiseChannel: обработчик сегментации не установлен");
            }
            var bottomAxes = _model.Axes[0];
            double left = bottomAxes.AbsoluteMinimum;
            double right = bottomAxes.AbsoluteMaximum;
            if (_signalSupplier.SegmentsHandler.SegmentsNumber(Channel) > 0)
            {
                DialogResult res = MessageBox.Show("Все установленные сегменты будут стёрты!", "ВНИМАНИЕ!", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                if (res == DialogResult.Cancel)
                {
                    return;
                }
                _signalSupplier.SegmentsHandler.DeletSegments(Channel);
                _model.Annotations.Clear();
            }
            if (_signalSupplier.SegmentsHandler.AppendSegment(PlotCoordToSegment(left, right, SegmentType.Noise)))
            {
                _model.Annotations.Add(BuildAnnotation(left, right, SegmentType.Noise));
            }
            else
            {
                MessageBox.Show("SetNoiseChannel: Ошибка удаления. Сообщите разработчику", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            _plotView.InvalidatePlot(true);
            _plotView.Focus();
        }
        /// <summary>
        /// Обновление графика
        /// </summary>
        public void RefreshPlot()
        {
            //Обновляем график, выводя выбранный ранее канал
            PrintSignal(Channel);
            _plotView.Focus();
        }
    }
}
