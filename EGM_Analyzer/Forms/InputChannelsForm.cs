using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EGM_Analyzer.Forms
{
    public partial class InputChannelsForm : Form
    {
        public byte? StartChannel { get; private set; }
        public byte? FinishChannel { get; private set; }
        public InputChannelsForm(string message)
        {
            InitializeComponent();
            label1.Text = message;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            byte startChannel;
            if (byte.TryParse(this.textBox1.Text, out startChannel))
            {
                if (startChannel > 0)
                    StartChannel = (byte)(startChannel - 1);
            }
            byte finishChannel;
            if (byte.TryParse(this.textBox2.Text, out finishChannel))
            {
                FinishChannel = (byte)(finishChannel);
            }

            if (StartChannel >= FinishChannel)
            {
                StartChannel = null;
                FinishChannel = null;
            }
            this.Close();
        }
    }
}
