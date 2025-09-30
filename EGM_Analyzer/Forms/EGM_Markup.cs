using EGM_Analyzer.DataLoader;
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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace EGM_Analyzer.Forms
{
    public partial class EGM_Markup : Form
    {
        IDataLoader _dataLoader;
        List<Rectangle> _originsSizes;
        private Rectangle _originFrameSize;
        List<Control> _controls;
        SegmentationEventHandler _segmentationEventHandler;

        void AppendControls(Control control)
        {
            _controls.Add(control);
            _originsSizes.Add(new Rectangle(control.Location.X, control.Location.Y, control.Width, control.Height));
        }

        LoadingProcessForm _loadingForm;

        public EGM_Markup()
        {
            InitializeComponent();

            _originFrameSize = new Rectangle(this.Location.X, this.Location.Y, this.Width, this.Height);
            _originsSizes = new List<Rectangle>();
            _controls = new List<Control>();

            AppendControls(SignalMarkup);
            AppendControls(groupBox1);
            AppendControls(groupBox2);
            AppendControls(ChannelTitle);
            AppendControls(ChannelTitle);
            AppendControls(ChannelsList);
            AppendControls(SamplerateTitle);
            AppendControls(SampleRateLabel);
            AppendControls(ATSegmentWidthTitle);
            AppendControls(ATSegmentWidth);
            AppendControls(ATSegmentWidthValue);
            AppendControls(ScrollStepTitle);
            AppendControls(ScrollStepLabel);
            AppendControls(ScrollStepValue);
            AppendControls(button1);
            AppendControls(FullSignalToNoise);
            AppendControls(MarkupsPaths);
            AppendControls(SignalPath);
            AppendControls(hScrollBar1);

            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;

            _segmentationEventHandler = new SegmentationEventHandler(SignalMarkup, ChannelsList, SampleRateLabel, ATSegmentWidthValue, ScrollStepValue);
            _segmentationEventHandler.InitScrollBar(this.hScrollBar1);

            _segmentationEventHandler.OnWidthSegmentChange += TitleWidthUpdate;
            _segmentationEventHandler.OnScrollStepChange += ScrollStepValueUpdate;
            ATSegmentWidth.Text = _segmentationEventHandler.SegmentWidth.ToString();
            ScrollStepLabel.Text = _segmentationEventHandler.ScrollStep.ToString();

            _loadingForm = new LoadingProcessForm();
        }

        private void ScrollStepValueUpdate(object? sender, EventArgs e)
        {
            ScrollStepLabel.Text = _segmentationEventHandler.ScrollStep.ToString();
        }

        private void TitleWidthUpdate(object? sender, EventArgs e)
        {
            this.ATSegmentWidth.Text = _segmentationEventHandler.SegmentWidth.ToString();
        }

        private void EGM_Markup_Resize(object sender, EventArgs e)
        {
            if (_originsSizes == null) return;
            for (int i = 0; i < _originsSizes.Count; i++)
            {
                _3th_part.ResizeControl(_originsSizes[i], _controls[i], this, _originFrameSize);
            }
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void R_Complex_Click(object sender, EventArgs e)
        {
            _segmentationEventHandler.SetSegmentationType(SegmentType.AT);
        }

        private void T_Complex_CheckedChanged(object sender, EventArgs e)
        {
            _segmentationEventHandler.SetSegmentationType(SegmentType.RT);
        }

        private void Artifact_CheckedChanged(object sender, EventArgs e)
        {
            _segmentationEventHandler.SetSegmentationType(SegmentType.Artifact);
        }

        private void Noise_CheckedChanged(object sender, EventArgs e)
        {
            _segmentationEventHandler.SetSegmentationType(SegmentType.Noise);
        }

        private void тестовыйСигналToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _segmentationEventHandler.DrawTestSignal();
        }

        private void закрытьПрограммуToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void кВыборуРежимаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void сохранитьРазметкуToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_dataLoader == null)
            {
                MessageBox.Show("Сигнал не открыт, разметка не создана. Нечего сохранять.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            SaveMarkupDialog.Filter = "JSON files(*.json)|*.json";
            if (SaveMarkupDialog.ShowDialog() == DialogResult.Cancel)
                return;
            // получаем выбранный файл
            string filepath = SaveMarkupDialog.FileName;

            _dataLoader.SegmentsHandler.Save(filepath);
        }

        private async void подгрузитьРазметкуToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenMarkupDialog.Filter = "JSON files(*.json)|*.json";
            if (OpenMarkupDialog.ShowDialog() == DialogResult.Cancel)
            {
                _loadingForm.Hide();
                return;
            }
            string markupFilename = OpenMarkupDialog.FileName;
            try
            {
                await _dataLoader.AppendSegments(markupFilename);
                _segmentationEventHandler.RefreshPlot();
            }
            catch (Exception ex)
            {
                _loadingForm.Hide();
                MessageBox.Show("Ошибка открытия файла резметки!", ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _segmentationEventHandler.ClearVisibleSegmentation();
        }

        private void FullSignalToNoise_Click(object sender, EventArgs e)
        {
            _segmentationEventHandler.SetNoiseChannel();
        }

        InputTextForm? InputTextForm = null;
        private void загрузитьСигналToolStripMenuItem1_Click_1(object sender, EventArgs e)
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
            this.SignalPath.Text = fileInfo.FullName;

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
            await _dataLoader.LoadSignal();
            GC.Collect();

            _segmentationEventHandler.SetDataSupplier(_dataLoader);
            _loadingForm.Hide();
        }

        private async void подгрузитьРазметкуToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            if (_dataLoader == null)
            {
                MessageBox.Show("Ошибка!",
                    "Нельзя открыть разметку, если отсутствует файл сигнала!",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            _loadingForm.Show();
            OpenSignalDialog.Filter = "JSON files(*.json)|*.json";
            if (OpenSignalDialog.ShowDialog() == DialogResult.Cancel)
            {
                _loadingForm.Hide();
                return;
            }

            if (!this.MarkupsPaths.Text.Contains(OpenSignalDialog.FileName))
            {
                this.MarkupsPaths.Text += Environment.NewLine + OpenSignalDialog.FileName;
            }

            // загружаем файл разметки
            try
            {
                await _dataLoader.LoadMarkups(OpenSignalDialog.FileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка открытия разметки!",
                    ex.Message,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                _loadingForm.Hide();
                return;
            }
            GC.Collect();
            // Обновляем разметку в случае успеха загрузки
            _segmentationEventHandler.RefreshPlot();
            _loadingForm.Hide();
        }

        private async void добавитьРазметкуToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_dataLoader == null)
            {
                MessageBox.Show("Ошибка!",
                    "Нельзя открыть разметку, если отсутствует файл сигнала!",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            _loadingForm.Show();
            OpenSignalDialog.Filter = "JSON files(*.json)|*.json";
            if (OpenSignalDialog.ShowDialog() == DialogResult.Cancel)
            {
                _loadingForm.Hide();
                return;
            }

            if (!this.MarkupsPaths.Text.Contains(OpenSignalDialog.FileName))
            {
                this.MarkupsPaths.Text += Environment.NewLine + OpenSignalDialog.FileName;
            }

            // загружаем файл разметки
            try
            {
                await _dataLoader.AppendSegments(OpenSignalDialog.FileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка открытия разметки!",
                    ex.Message,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                _loadingForm.Hide();
                return;
            }
            GC.Collect();
            // Обновляем разметку в случае успеха загрузки
            _segmentationEventHandler.RefreshPlot();
            _loadingForm.Hide();
        }

        private async void сохранитьСигналВNPYToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_dataLoader == null)
            {
                MessageBox.Show("Ошибка!",
                    "Файл с сигналом не найден!",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            _loadingForm.Show();
            SaveMarkupDialog.Filter = "Numpy files(*.npy)|*.npy|BIN files(*.bin)|*.bin";
            if (SaveMarkupDialog.ShowDialog() == DialogResult.Cancel)
                return;
            string filepath = SaveMarkupDialog.FileName;
            await _dataLoader.ResaveSignal(filepath);
            GC.Collect();
            _loadingForm.Hide();
        }

        private void информацияToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Information information = new Information();
            information.Show();
        }

        private async void разметкуXLSXToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_dataLoader == null)
            {
                MessageBox.Show("Ошибка!",
                    "Нельзя открыть разметку, если отсутствует файл сигнала!",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            _loadingForm.Show();
            OpenSignalDialog.Filter = "XLSX files(*.xlsx)|*.xlsx";
            if (OpenSignalDialog.ShowDialog() == DialogResult.Cancel)
            {
                _loadingForm.Hide();
                return;
            }
            if (!this.MarkupsPaths.Text.Contains(OpenSignalDialog.FileName))
            {
                this.MarkupsPaths.Text += Environment.NewLine + OpenSignalDialog.FileName;
            }
            // загружаем файл разметки
            try
            {
                await _dataLoader.LoadXLSXMarkups(OpenSignalDialog.FileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка открытия разметки!",
                    ex.Message,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                _loadingForm.Hide();
                return;
            }
            GC.Collect();
            // Обновляем разметку в случае успеха загрузки
            _segmentationEventHandler.RefreshPlot();
            _loadingForm.Hide();
        }

        private void ScrollStepValue_Scroll(object sender, EventArgs e)
        {

        }

        private void SignalPath_MouseHover(object sender, EventArgs e)
        {
            string signalResult = SignalPath.Text.Length < 10 ? "Сигнал не установлен" : SignalPath.Text;
            toolTip1.Show(signalResult, SignalPath);
        }

        private void MarkupsPaths_MouseHover(object sender, EventArgs e)
        {
            string signalResult = MarkupsPaths.Text.Length < 15 ? "Разметка не установлена" : MarkupsPaths.Text;
            toolTip1.Show(signalResult, MarkupsPaths);
        }

        InputChannelsForm? InputChannelsForm = null;

        private void загрузитьРазметкуЧастиКаналовToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InputChannelsForm = new InputChannelsForm("Введите стартовый и финишный канал разметки");
            InputChannelsForm.Show();
            InputChannelsForm.FormClosing += InputChannelsFormClose;
        }

        private async void InputChannelsFormClose(object? sender, FormClosingEventArgs e)
        {
            InputChannelsForm inputTextForm = sender as InputChannelsForm;
            if (inputTextForm.StartChannel != null && inputTextForm.FinishChannel != null)
            {
                if (_dataLoader == null)
                {
                    MessageBox.Show("Ошибка!",
                        "Нельзя открыть разметку, если отсутствует файл сигнала!",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                _loadingForm.Show();
                OpenSignalDialog.Filter = "JSON files(*.json)|*.json";
                if (OpenSignalDialog.ShowDialog() == DialogResult.Cancel)
                {
                    _loadingForm.Hide();
                    return;
                }

                if (!this.MarkupsPaths.Text.Contains(OpenSignalDialog.FileName))
                {
                    this.MarkupsPaths.Text += Environment.NewLine + OpenSignalDialog.FileName;
                }

                // загружаем файл разметки
                try
                {
                    await _dataLoader.LoadMarkups(OpenSignalDialog.FileName, inputTextForm.StartChannel.Value,
                        inputTextForm.FinishChannel.Value);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка открытия разметки!",
                        ex.Message,
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    _loadingForm.Hide();
                    return;
                }
                GC.Collect();
                // Обновляем разметку в случае успеха загрузки
                _segmentationEventHandler.RefreshPlot();
                _loadingForm.Hide();
            }
            else
            {
                MessageBox.Show("Вы не установили стартовый и/или финишный канал!", "Ошибка!",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            inputTextForm.FormClosing -= InpurFormClose;
        }
    }
}
