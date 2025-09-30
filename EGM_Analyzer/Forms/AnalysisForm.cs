using EGM_Analyzer.Analyzers;
using EGM_Analyzer.Analyzers.Classes;
using EGM_Analyzer.Analyzers.SegmentationTools;
using EGM_Analyzer.DataLoader;
using EGM_Analyzer.Segmentation;
using ParseData.Electrogram;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;

namespace EGM_Analyzer.Forms
{
    public partial class AnalysisForm : Form
    {
        List<Rectangle> _originsSizes;
        private Rectangle _originFrameSize;
        List<Control> _controls;

        void AppendControls(Control control)
        {
            _controls.Add(control);
            _originsSizes.Add(new Rectangle(control.Location.X, control.Location.Y, control.Width, control.Height));
        }

        IDataLoader? _dataLoader;
        IAnalyzer? _analyzer;
        SegmentationEventHandler _segmentationEventHandler;
        NavigationPlot _navigationPlot;
        ActivationMapPlot _activationMapPlot;
        public AnalysisForm()
        {
            InitializeComponent();
            _originFrameSize = new Rectangle(this.Location.X, this.Location.Y, this.Width, this.Height);
            _originsSizes = new List<Rectangle>();
            _controls = new List<Control>();

            AppendControls(label1);
            AppendControls(label2);
            AppendControls(groupBox1);
            AppendControls(ChannelsList);
            AppendControls(SampleRateLabel);
            AppendControls(ScrollStepValue);
            AppendControls(hScrollBar1);
            AppendControls(SignalPlot);
            AppendControls(RRIntervalsValue);
            AppendControls(HeatMapHeartPoint);



            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;

            _loadingForm = new LoadingProcessForm();

            _segmentationEventHandler = new SegmentationEventHandler(SignalPlot, ChannelsList, SampleRateLabel, null, ScrollStepValue);
            _segmentationEventHandler.InitScrollBar(this.hScrollBar1);

            _navigationPlot = new NavigationPlot(RRIntervalsValue, ChannelsList);
            
        }

        private void загрузкаМоделиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadModel();
        }

        LoadingProcessForm _loadingForm;
        InputTextForm? InputTextForm = null;
        private void загрузкаСигналаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InputTextForm = new InputTextForm("Введите частоту сигнала");
            InputTextForm.Show();
            InputTextForm.FormClosing += InpurFormClose;
        }

        private void InpurFormClose(object? sender, FormClosingEventArgs e)
        {
            InputTextForm inputTextForm = sender as InputTextForm;
            ushort samplerate;
            if (inputTextForm != null && inputTextForm.Data != null
                && ushort.TryParse(inputTextForm.Data, out samplerate))
            {
                LoadSignal(samplerate);
            }
            else
            {
                MessageBox.Show("Вы не установили частоту сигнала", "Ошибка!",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            inputTextForm.FormClosing -= InpurFormClose;
        }

        private void LoadModel()
        {
            _loadingForm.Show();

            ModelLoadDialog.Filter = "ONNX files(*.onnx)|*.onnx";
            if (ModelLoadDialog.ShowDialog() == DialogResult.Cancel)
            {
                _loadingForm.Hide();
                return;
            }

            FileInfo fileInfo = new FileInfo(ModelLoadDialog.FileName);
            _analyzer = new AISegmentationAnalyzer(ModelLoadDialog.FileName,
                new List<IClass>() { new Background(), new Activate(), new PostActivate(), new Artefacts() });

            _loadingForm.Hide();

            if (_dataLoader != null)
                PredictSegmentation();

        }

        RRAnalyzer rrAnalaser;

        private async void PredictSegmentation()
        {
            _loadingForm.Show();
            if (_analyzer == null)
                throw new ArgumentNullException("Объект анализатора не проинициализирован!");
            if (_dataLoader == null)
                throw new ArgumentNullException("Не загружен сигнал!");

            await _analyzer.Analyze(_dataLoader);
            var segments = _analyzer.AnalysisResult.GetSegments();
            _dataLoader.SegmentsHandler.AppendSegments(segments);
            _segmentationEventHandler.RefreshPlot();
            rrAnalaser = new RRAnalyzer(_dataLoader);

            _segmentationEventHandler.DrawPoints(rrAnalaser.ActivationMoments[ChannelsList.SelectedIndex], 
                (byte)ChannelsList.SelectedIndex);

            _navigationPlot.SetDataSuplier(rrAnalaser);


            _loadingForm.Hide();
        }


        private async void LoadSignal(ushort samplerate)
        {
            _loadingForm.Show();

            OpenSignalDialog.Filter = "NPY files(*.npy)|*.npy|C2D files(*.c2d)|*.c2d";
            if (OpenSignalDialog.ShowDialog() == DialogResult.Cancel)
            {
                _loadingForm.Hide();
                return;
            }

            FileInfo fileInfo = new FileInfo(OpenSignalDialog.FileName);
            //this.SignalPath.Text = fileInfo.FullName;

            switch (fileInfo.Extension)
            {
                case ".npy":
                    _dataLoader = new DataLoader.DataLoader(OpenSignalDialog.FileName, samplerate);
                    break;
                case ".c2d":
                    _dataLoader = new C2D_Loader(OpenSignalDialog.FileName, samplerate);
                    break;
                default:
                    _loadingForm.Hide();
                    MessageBox.Show("Ошибка открытия файла сигнала!", "Не установлен тип файла",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
            }
            _activationMapPlot = new ActivationMapPlot(HeatMapHeartPoint, new Tuple<byte, byte>(8, 8), _dataLoader);

            await _dataLoader.LoadSignal();
            GC.Collect();

            _segmentationEventHandler.SetDataSupplier(_dataLoader);

            _loadingForm.Hide();

            if (_analyzer != null)
                PredictSegmentation();
        }

        private void AnalysisForm_Resize(object sender, EventArgs e)
        {
            if (_originsSizes == null) return;
            for (int i = 0; i < _originsSizes.Count; i++)
            {
                _3th_part.ResizeControl(_originsSizes[i], _controls[i], this, _originFrameSize);
            }
        }
    }
}
