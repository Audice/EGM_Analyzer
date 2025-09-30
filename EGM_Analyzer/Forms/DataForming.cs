using EGM_Analyzer.DataLoader;
using EGM_Analyzer.DataPrepare;
using EGM_Analyzer.Segmentation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CsvHelper;
using CsvHelper.Configuration.Attributes;
using System.Globalization;
using EGM_Analyzer.DatasetBuilder.DataBase;
using NumSharp;
using OxyPlot.Series;
using OxyPlot;
using OxyPlot.Annotations;

namespace EGM_Analyzer.Forms
{

    public partial class DataForming : Form
    {
        TabViewHandler tabViewHandler;
        IDataLoader _dataLoader;
        List<Rectangle> _originsSizes;
        private Rectangle _originFrameSize;
        List<Control> _controls;

        LoadingProcessForm _loadingForm;


        void AppendControls(Control control)
        {
            _controls.Add(control);
            _originsSizes.Add(new Rectangle(control.Location.X, control.Location.Y, control.Width, control.Height));
        }


        public DataForming()
        {
            InitializeComponent();
            _originFrameSize = new Rectangle(this.Location.X, this.Location.Y, this.Width, this.Height);
            _originsSizes = new List<Rectangle>();
            _controls = new List<Control>();

            AppendControls(DataPartPlot);
            AppendControls(groupBox1);
            AppendControls(label1);
            AppendControls(CurrentChannel);

            AppendControls(button1);
            AppendControls(button3);
            AppendControls(button4);
            AppendControls(button2);
            AppendControls(MarkupRecords);
            AppendControls(groupBox2);

            AppendControls(PartDurationLabel);
            AppendControls(label2);
            AppendControls(DurationTrackBar);

            AppendControls(label3);
            AppendControls(OverlapDuration);
            AppendControls(OverlapTrackBar);

            AppendControls(CheckedTargetFramerate);
            AppendControls(TargetFramerateValue);
            AppendControls(CheckNormalizeSignal);
            AppendControls(CheckRandomFile);
            AppendControls(VisualizeSignalPart);

            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;

            tabViewHandler = new TabViewHandler(MarkupRecords, OpenRecord);



        }

        private void button3_Click(object sender, EventArgs e)
        {
            tabViewHandler.AppendRecord();
        }

        private async void button4_Click(object sender, EventArgs e)
        {
            if (CheckedTargetFramerate.Checked && TargetFramerateValue.SelectedIndex == -1)
                MessageBox.Show("Не установлена целевая частота", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);


            DatasetBuilder.DatasetBuilder datasetBuilder = new DatasetBuilder.DatasetBuilder(
                tabViewHandler.GetPreparingRecords(),
                (uint)(DurationTrackBar.Value),
                (ushort)(OverlapTrackBar.Value),
                CheckNormalizeSignal.Checked);
            //(short)(CheckNormalizeSignal.Checked ? TargetFramerateValue.SelectedItem : -1));

            _loadingForm = new LoadingProcessForm(datasetBuilder);

            _loadingForm.Show();

            await datasetBuilder.PrepareDataset();

            _loadingForm.Hide();
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            string selectedSourceFolder = folderBrowserDialog.SelectedPath;
            if (folderBrowserDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            string selectedSaveFolder = folderBrowserDialog.SelectedPath;

            DatabaseBuilder databaseBuilder = new DatabaseBuilder(selectedSourceFolder, selectedSaveFolder,
                CheckNormalizeSignal.Checked, (uint)(DurationTrackBar.Value), (ushort)(OverlapTrackBar.Value));
            databaseBuilder.PrepareSignalMarkupsList();



            await databaseBuilder.DataToBase();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            //Путь до папки, в которой лежат сигналы + разметка
            string selectedSourceFolder = folderBrowserDialog.SelectedPath;
            var preparingRecords = DatabaseBuilder.GetPreparingRecords(selectedSourceFolder);
            tabViewHandler.AppendRecords(preparingRecords);
        }

        private void DataForming_Resize(object sender, EventArgs e)
        {
            if (_originsSizes == null) return;
            for (int i = 0; i < _originsSizes.Count; i++)
            {
                _3th_part.ResizeControl(_originsSizes[i], _controls[i], this, _originFrameSize);
            }
        }

        private LineSeries _signalSeries;

        private static Dictionary<SegmentType, OxyPlot.OxyColor> _segmentsColor = new Dictionary<SegmentType, OxyColor>()
        {
            {SegmentType.AT,  OxyPlot.OxyColors.Red},
            {SegmentType.RT,  OxyPlot.OxyColors.Green },
            {SegmentType.Artifact,  OxyPlot.OxyColors.Blue },
            {SegmentType.Noise,  OxyPlot.OxyColors.Gray }
        };
        private const byte TransparentLevel = 80;


        private void CheckRandomFile_Click(object sender, EventArgs e)
        {
            Random random = new Random((int)DateTime.Now.Ticks);

            if (_datasetFolder == null) return;
            if (_datasetFolder.Length < 4) return;

            // Получить список каталогов файла
            var directories = Directory.GetDirectories(_datasetFolder);

            string curSignal = directories[random.NextInt64(0, directories.Length)];

            var channelsDir = Directory.GetDirectories(curSignal);

            string curChannel = channelsDir[random.NextInt64(0, channelsDir.Length)];

            var signals = Directory.GetFiles(curChannel);

            string filename = signals[random.NextInt64(0, signals.Length)];
            filename = filename.Replace("_mask", "");

            FileInfo signal = new FileInfo(filename);
            string maskName = signal.Name.Replace(".npy", "") + "_mask.npy";
            FileInfo mask = new FileInfo(Path.Combine(curChannel, maskName));


            double[] subSignal = np.Load<double[]>(signal.FullName);
            byte[] subMask = np.Load<byte[]>(mask.FullName);

            if (DataPartPlot.Model == null)
            {
                DataPartPlot.Model = new OxyPlot.PlotModel();
            }

            if (_signalSeries == null)
            {
                _signalSeries = new OxyPlot.Series.LineSeries()
                {
                    Color = OxyPlot.OxyColors.Blue,
                    StrokeThickness = 2,
                };
                DataPartPlot.Model.Series.Add(_signalSeries);
            }

            _signalSeries.Points.Clear();
            DataPartPlot.Model.Annotations.Clear();

            double signalMax = subSignal.Max();
            double signalMin = subSignal.Min();

            for (int i = 0; i < subSignal.Length; i++)
                _signalSeries.Points.Add(new OxyPlot.DataPoint(i, subSignal[i]));

            int startIndex = -1;
            for (int i = 0; i < subMask.Length; i++)
            {
                if (subMask[i] != 0 && startIndex == -1)
                {
                    startIndex = i;
                }
                if (subMask[i] == 0 && startIndex != -1)
                {
                    SegmentType currentSegmentType = (SegmentType)(subMask[i - 1] - 1);
                    RectangleAnnotation rectangleAnnotation = new RectangleAnnotation
                    {
                        MinimumX = startIndex,
                        MaximumX = i - 1,
                        MinimumY = signalMin,
                        MaximumY = signalMax,
                        Fill = OxyColor.FromAColor(TransparentLevel,
                                                    _segmentsColor[currentSegmentType]),
                        Stroke = _segmentsColor[currentSegmentType],
                        StrokeThickness = 2
                    };
                    DataPartPlot.Model.Annotations.Add(rectangleAnnotation);
                    startIndex = -1;
                }
            }

            if (startIndex != -1)
            {
                SegmentType currentSegmentType = (SegmentType)(subMask[subMask.Length - 1] - 1);
                RectangleAnnotation rectangleAnnotation = new RectangleAnnotation
                {
                    MinimumX = startIndex,
                    MaximumX = subMask.Length - 1,
                    MinimumY = signalMin,
                    MaximumY = signalMax,
                    Fill = OxyColor.FromAColor(TransparentLevel,
                                                _segmentsColor[currentSegmentType]),
                    Stroke = _segmentsColor[currentSegmentType],
                    StrokeThickness = 2
                };
                DataPartPlot.Model.Annotations.Add(rectangleAnnotation);
                startIndex = -1;
            }

            VisualizeSignalPart.Text = signal.FullName.Replace(_datasetFolder, "");

            DataPartPlot.InvalidatePlot(true);
        }

        string? _datasetFolder = null;
        private void установитьПапкуDatasetаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            //Путь до папки, в которой лежат сигналы + разметка
            _datasetFolder = folderBrowserDialog.SelectedPath;
        }

        private void закрытьПрограммуToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void кВыборуРежимаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void DurationTrackBar_ValueChanged(object sender, EventArgs e)
        {
            PartDurationLabel.Text = DurationTrackBar.Value.ToString();
        }

        private void OverlapTrackBar_ValueChanged(object sender, EventArgs e)
        {
            OverlapDuration.Text = OverlapTrackBar.Value.ToString();
        }
    }
}
