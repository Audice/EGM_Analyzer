using EGM_Analyzer.Forms;
using System.Text.Json;
using System;
using EGM_Analyzer.DataLoader.OldMurkup;

namespace EGM_Analyzer
{
    public partial class Form1 : Form
    {
        List<Rectangle> _originsSizes;
        private Rectangle _originFrameSize;
        List<Control> _controls;

        void AppendControls(Control control)
        {
            _controls.Add(control);
            _originsSizes.Add(new Rectangle(control.Location.X, control.Location.Y, control.Width, control.Height));
        }
        public Form1()
        {
            InitializeComponent();

            _originFrameSize = new Rectangle(this.Location.X, this.Location.Y, this.Width, this.Height);
            _originsSizes = new List<Rectangle>();
            _controls = new List<Control>();

            AppendControls(pictureBox1);
            AppendControls(pictureBox2);
            AppendControls(pictureBox3);
            AppendControls(pictureBox4);
            AppendControls(pictureBox5);
            AppendControls(pictureBox6);
            AppendControls(pictureBox7);
            AppendControls(label1);
            AppendControls(label2);
            AppendControls(label3);
            AppendControls(label4);
            AppendControls(label5);
            AppendControls(label6);

            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;

        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (_originsSizes == null) return;
            for (int i = 0; i < _originsSizes.Count; i++)
            {
                _3th_part.ResizeControl(_originsSizes[i], _controls[i], this, _originFrameSize);
            }
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            MarkupFormOpen();
        }

        private void MarkupFormOpen()
        {
            this.Hide();
            var markupForm = new EGM_Markup();
            markupForm.Closed += (s, args) =>
            {
                this.Show();
            };
            markupForm.Show();
        }

        private void AnalysisFormingOpen(object sender, EventArgs e)
        {
            this.Hide();
            var dataForming = new AnalysisForm();
            dataForming.Closed += (s, args) =>
            {
                this.Show();
            };
            dataForming.Show();
        }

        private void DataFormingOpen(object sender, EventArgs e)
        {
            this.Hide();
            var dataForming = new DataForming();
            dataForming.Closed += (s, args) =>
            {
                this.Show();
            };
            dataForming.Show();
        }

        private void ResearchFormOpen()
        {
            this.Hide();
            var researchForm = new ResearchForm();
            researchForm.Closed += (s, args) =>
            {
                this.Show();
            };
            researchForm.Show();
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            MarkupFormOpen();
        }

        private void label1_Click(object sender, EventArgs e)
        {
            MarkupFormOpen();
        }

        private void label2_Click(object sender, EventArgs e)
        {
            MarkupFormOpen();
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

    }
}
