namespace EGM_Analyzer.Forms
{
    partial class AnalysisForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            menuStrip1 = new MenuStrip();
            файлToolStripMenuItem = new ToolStripMenuItem();
            загрузкаМоделиToolStripMenuItem = new ToolStripMenuItem();
            загрузкаСигналаToolStripMenuItem = new ToolStripMenuItem();
            SignalPlot = new OxyPlot.WindowsForms.PlotView();
            RRIntervalsValue = new OxyPlot.WindowsForms.PlotView();
            groupBox1 = new GroupBox();
            ScrollStepValue = new TrackBar();
            label2 = new Label();
            label1 = new Label();
            SampleRateLabel = new Label();
            ChannelsList = new ComboBox();
            OpenSignalDialog = new OpenFileDialog();
            HeatMapHeartPoint = new OxyPlot.WindowsForms.PlotView();
            hScrollBar1 = new HScrollBar();
            ModelLoadDialog = new OpenFileDialog();
            menuStrip1.SuspendLayout();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)ScrollStepValue).BeginInit();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { файлToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(969, 24);
            menuStrip1.TabIndex = 0;
            menuStrip1.Text = "menuStrip1";
            // 
            // файлToolStripMenuItem
            // 
            файлToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { загрузкаМоделиToolStripMenuItem, загрузкаСигналаToolStripMenuItem });
            файлToolStripMenuItem.Name = "файлToolStripMenuItem";
            файлToolStripMenuItem.Size = new Size(37, 20);
            файлToolStripMenuItem.Text = "File";
            // 
            // загрузкаМоделиToolStripMenuItem
            // 
            загрузкаМоделиToolStripMenuItem.Name = "загрузкаМоделиToolStripMenuItem";
            загрузкаМоделиToolStripMenuItem.Size = new Size(154, 22);
            загрузкаМоделиToolStripMenuItem.Text = "Loading model";
            загрузкаМоделиToolStripMenuItem.Click += загрузкаМоделиToolStripMenuItem_Click;
            // 
            // загрузкаСигналаToolStripMenuItem
            // 
            загрузкаСигналаToolStripMenuItem.Name = "загрузкаСигналаToolStripMenuItem";
            загрузкаСигналаToolStripMenuItem.Size = new Size(154, 22);
            загрузкаСигналаToolStripMenuItem.Text = "Loading signal";
            загрузкаСигналаToolStripMenuItem.Click += загрузкаСигналаToolStripMenuItem_Click;
            // 
            // SignalPlot
            // 
            SignalPlot.BackColor = Color.White;
            SignalPlot.Location = new Point(12, 27);
            SignalPlot.Name = "SignalPlot";
            SignalPlot.PanCursor = Cursors.Hand;
            SignalPlot.Size = new Size(713, 378);
            SignalPlot.TabIndex = 1;
            SignalPlot.Text = "plotView1";
            SignalPlot.ZoomHorizontalCursor = Cursors.SizeWE;
            SignalPlot.ZoomRectangleCursor = Cursors.SizeNWSE;
            SignalPlot.ZoomVerticalCursor = Cursors.SizeNS;
            // 
            // RRIntervalsValue
            // 
            RRIntervalsValue.BackColor = Color.White;
            RRIntervalsValue.Location = new Point(12, 428);
            RRIntervalsValue.Name = "RRIntervalsValue";
            RRIntervalsValue.PanCursor = Cursors.Hand;
            RRIntervalsValue.Size = new Size(713, 69);
            RRIntervalsValue.TabIndex = 2;
            RRIntervalsValue.Text = "plotView2";
            RRIntervalsValue.ZoomHorizontalCursor = Cursors.SizeWE;
            RRIntervalsValue.ZoomRectangleCursor = Cursors.SizeNWSE;
            RRIntervalsValue.ZoomVerticalCursor = Cursors.SizeNS;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(ScrollStepValue);
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(label1);
            groupBox1.Controls.Add(SampleRateLabel);
            groupBox1.Controls.Add(ChannelsList);
            groupBox1.Font = new Font("Segoe UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 204);
            groupBox1.Location = new Point(731, 27);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(226, 184);
            groupBox1.TabIndex = 3;
            groupBox1.TabStop = false;
            groupBox1.Text = "Display settings";
            // 
            // ScrollStepValue
            // 
            ScrollStepValue.Location = new Point(6, 133);
            ScrollStepValue.Name = "ScrollStepValue";
            ScrollStepValue.Size = new Size(214, 45);
            ScrollStepValue.TabIndex = 4;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(15, 60);
            label2.Name = "label2";
            label2.Size = new Size(82, 25);
            label2.TabIndex = 3;
            label2.Text = "Channel";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(15, 28);
            label1.Name = "label1";
            label1.Size = new Size(99, 25);
            label1.TabIndex = 2;
            label1.Text = "Frequency";
            // 
            // SampleRateLabel
            // 
            SampleRateLabel.AutoSize = true;
            SampleRateLabel.Location = new Point(130, 28);
            SampleRateLabel.Name = "SampleRateLabel";
            SampleRateLabel.Size = new Size(63, 25);
            SampleRateLabel.TabIndex = 1;
            SampleRateLabel.Text = "label1";
            // 
            // ChannelsList
            // 
            ChannelsList.FormattingEnabled = true;
            ChannelsList.Location = new Point(130, 57);
            ChannelsList.Name = "ChannelsList";
            ChannelsList.Size = new Size(90, 33);
            ChannelsList.TabIndex = 0;
            // 
            // OpenSignalDialog
            // 
            OpenSignalDialog.FileName = "openFileDialog1";
            // 
            // HeatMapHeartPoint
            // 
            HeatMapHeartPoint.Location = new Point(731, 246);
            HeatMapHeartPoint.Name = "HeatMapHeartPoint";
            HeatMapHeartPoint.PanCursor = Cursors.Hand;
            HeatMapHeartPoint.Size = new Size(226, 251);
            HeatMapHeartPoint.TabIndex = 4;
            HeatMapHeartPoint.Text = "plotView1";
            HeatMapHeartPoint.ZoomHorizontalCursor = Cursors.SizeWE;
            HeatMapHeartPoint.ZoomRectangleCursor = Cursors.SizeNWSE;
            HeatMapHeartPoint.ZoomVerticalCursor = Cursors.SizeNS;
            // 
            // hScrollBar1
            // 
            hScrollBar1.Location = new Point(12, 410);
            hScrollBar1.Name = "hScrollBar1";
            hScrollBar1.Size = new Size(713, 17);
            hScrollBar1.TabIndex = 5;
            // 
            // ModelLoadDialog
            // 
            ModelLoadDialog.FileName = "openFileDialog1";
            // 
            // AnalysisForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(969, 509);
            Controls.Add(hScrollBar1);
            Controls.Add(HeatMapHeartPoint);
            Controls.Add(groupBox1);
            Controls.Add(RRIntervalsValue);
            Controls.Add(SignalPlot);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            Name = "AnalysisForm";
            Text = "AnalysisForm";
            Resize += AnalysisForm_Resize;
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)ScrollStepValue).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip menuStrip1;
        private ToolStripMenuItem файлToolStripMenuItem;
        private ToolStripMenuItem загрузкаМоделиToolStripMenuItem;
        private ToolStripMenuItem загрузкаСигналаToolStripMenuItem;
        private OxyPlot.WindowsForms.PlotView SignalPlot;
        private OxyPlot.WindowsForms.PlotView RRIntervalsValue;
        private GroupBox groupBox1;
        private OpenFileDialog OpenSignalDialog;
        private ComboBox ChannelsList;
        private Label label2;
        private Label label1;
        private Label SampleRateLabel;
        private OxyPlot.WindowsForms.PlotView HeatMapHeartPoint;
        private TrackBar ScrollStepValue;
        private HScrollBar hScrollBar1;
        private OpenFileDialog ModelLoadDialog;
    }
}