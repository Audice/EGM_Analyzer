using EGM_Analyzer.DataLoader;
using EGM_Analyzer.EventsArgs;
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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;

namespace EGM_Analyzer.Forms
{
    public partial class LoadingProcessForm : Form
    {
        List<Rectangle> _originsSizes;
        private Rectangle _originFrameSize;
        List<Control> _controls;

        IDataLoader? _dataLoader;

        void AppendControls(Control control)
        {
            _controls.Add(control);
            _originsSizes.Add(new Rectangle(control.Location.X, control.Location.Y, control.Width, control.Height));
        }

        public LoadingProcessForm()
        {
            InitializeComponent();

            _originFrameSize = new Rectangle(this.Location.X, this.Location.Y, this.Width, this.Height);
            _originsSizes = new List<Rectangle>();
            _controls = new List<Control>();

            //AppendControls(this.pictureBox1);
            //AppendControls(this.label1);


            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;

        }

        DatasetBuilder.DatasetBuilder _datasetBuilder;

        public LoadingProcessForm(DatasetBuilder.DatasetBuilder datasetBuilder)
        {
            InitializeComponent();

            _originFrameSize = new Rectangle(this.Location.X, this.Location.Y, this.Width, this.Height);
            _originsSizes = new List<Rectangle>();
            _controls = new List<Control>();

            //AppendControls(this.pictureBox1);
            //AppendControls(this.label1);


            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;

            _datasetBuilder = datasetBuilder;

            _datasetBuilder.RecordsProcessUpdate += UpdateSaveInfo;
        }

        private void UpdateSaveInfo(object? sender, RecordsProcessedEventArgs e)
        {
            string result = $"Сохранение датасета. \n Запись: {e.CurrentRecordName} \n Канал: {e.ChannelNumber}" +
                $"\n Номер записи: {e.CurrentRecordsNumber} из {e.NumRecords}" +
                $"\n Номер части: {e.CurrentPartNumber} из {e.NumParts}";

            
            label1.BeginInvoke(new Action(() =>
            {
                label1.Text = result;
            }));
        }

        private void LoadingProcessForm_Resize(object sender, EventArgs e)
        {
/*            if (_originsSizes == null) return;
            for (int i = 0; i < _originsSizes.Count; i++)
            {
                _3th_part.ResizeControl(_originsSizes[i], _controls[i], this, _originFrameSize);
            }*/
        }

        private void LoadingProcessForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (_datasetBuilder != null && _datasetBuilder.RecordsProcessUpdate != null)
                _datasetBuilder.RecordsProcessUpdate -= UpdateSaveInfo;
        }
    }
}
