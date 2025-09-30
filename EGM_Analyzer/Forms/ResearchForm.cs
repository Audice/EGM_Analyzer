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
    public partial class ResearchForm : Form
    {
        List<Rectangle> _originsSizes;
        private Rectangle _originFrameSize;
        List<Control> _controls;

        void AppendControls(Control control)
        {
            _controls.Add(control);
            _originsSizes.Add(new Rectangle(control.Location.X, control.Location.Y, control.Width, control.Height));
        }

        public ResearchForm()
        {
            InitializeComponent();
            _originFrameSize = new Rectangle(this.Location.X, this.Location.Y, this.Width, this.Height);
            _originsSizes = new List<Rectangle>();
            _controls = new List<Control>();

            //AppendControls(SignalMarkup);


            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
        }

        private void ResearchForm_Resize(object sender, EventArgs e)
        {
            if (_originsSizes == null) return;
            for (int i = 0; i < _originsSizes.Count; i++)
            {
                _3th_part.ResizeControl(_originsSizes[i], _controls[i], this, _originFrameSize);
            }
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
