namespace EGM_Analyzer.Forms
{
    partial class EGM_Markup
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
            components = new System.ComponentModel.Container();
            groupBox1 = new GroupBox();
            Artifact = new RadioButton();
            Noise = new RadioButton();
            T_Complex = new RadioButton();
            R_Complex = new RadioButton();
            menuStrip1 = new MenuStrip();
            файлToolStripMenuItem = new ToolStripMenuItem();
            загрузитьСигналToolStripMenuItem = new ToolStripMenuItem();
            загрузитьСигналToolStripMenuItem1 = new ToolStripMenuItem();
            подгрузитьРазметкуToolStripMenuItem = new ToolStripMenuItem();
            разметкуXLSXToolStripMenuItem = new ToolStripMenuItem();
            добавитьРазметкуToolStripMenuItem = new ToolStripMenuItem();
            загрузитьРазметкуЧастиКаналовToolStripMenuItem = new ToolStripMenuItem();
            сохранитьСигналВNPYToolStripMenuItem = new ToolStripMenuItem();
            сохранитьРазметкуToolStripMenuItem = new ToolStripMenuItem();
            тестовыйСигналToolStripMenuItem = new ToolStripMenuItem();
            оПрограммеToolStripMenuItem = new ToolStripMenuItem();
            информацияToolStripMenuItem = new ToolStripMenuItem();
            выходToolStripMenuItem = new ToolStripMenuItem();
            кВыборуРежимаToolStripMenuItem = new ToolStripMenuItem();
            закрытьПрограммуToolStripMenuItem = new ToolStripMenuItem();
            groupBox2 = new GroupBox();
            MarkupsPaths = new Label();
            SignalPath = new Label();
            SampleRateLabel = new Label();
            FullSignalToNoise = new Button();
            button1 = new Button();
            ScrollStepLabel = new Label();
            ScrollStepTitle = new Label();
            ScrollStepValue = new TrackBar();
            ATSegmentWidth = new Label();
            segmentationEventHandlerBindingSource1 = new BindingSource(components);
            ATSegmentWidthTitle = new Label();
            ATSegmentWidthValue = new TrackBar();
            SamplerateTitle = new Label();
            ChannelsList = new ComboBox();
            ChannelTitle = new Label();
            SignalMarkup = new OxyPlot.WindowsForms.PlotView();
            OpenSignalDialog = new OpenFileDialog();
            OpenMarkupDialog = new OpenFileDialog();
            SaveMarkupDialog = new SaveFileDialog();
            toolTip1 = new ToolTip(components);
            hScrollBar1 = new HScrollBar();
            groupBox1.SuspendLayout();
            menuStrip1.SuspendLayout();
            groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)ScrollStepValue).BeginInit();
            ((System.ComponentModel.ISupportInitialize)segmentationEventHandlerBindingSource1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)ATSegmentWidthValue).BeginInit();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(Artifact);
            groupBox1.Controls.Add(Noise);
            groupBox1.Controls.Add(T_Complex);
            groupBox1.Controls.Add(R_Complex);
            groupBox1.Font = new Font("Segoe UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 204);
            groupBox1.Location = new Point(761, 39);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(210, 129);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            groupBox1.Text = "Selecting a marking complex";
            // 
            // Artifact
            // 
            Artifact.AutoSize = true;
            Artifact.Location = new Point(6, 72);
            Artifact.Name = "Artifact";
            Artifact.Size = new Size(99, 29);
            Artifact.TabIndex = 3;
            Artifact.TabStop = true;
            Artifact.Text = "Artifacts";
            Artifact.UseVisualStyleBackColor = true;
            Artifact.CheckedChanged += Artifact_CheckedChanged;
            // 
            // Noise
            // 
            Noise.AutoSize = true;
            Noise.Location = new Point(6, 97);
            Noise.Name = "Noise";
            Noise.Size = new Size(78, 29);
            Noise.TabIndex = 2;
            Noise.TabStop = true;
            Noise.Text = "Noise";
            Noise.UseVisualStyleBackColor = true;
            Noise.CheckedChanged += Noise_CheckedChanged;
            // 
            // T_Complex
            // 
            T_Complex.AutoSize = true;
            T_Complex.Location = new Point(6, 47);
            T_Complex.Name = "T_Complex";
            T_Complex.Size = new Size(50, 29);
            T_Complex.TabIndex = 1;
            T_Complex.TabStop = true;
            T_Complex.Text = "RT";
            T_Complex.UseVisualStyleBackColor = true;
            T_Complex.CheckedChanged += T_Complex_CheckedChanged;
            // 
            // R_Complex
            // 
            R_Complex.AutoSize = true;
            R_Complex.ForeColor = Color.Black;
            R_Complex.Location = new Point(6, 22);
            R_Complex.Name = "R_Complex";
            R_Complex.Size = new Size(51, 29);
            R_Complex.TabIndex = 0;
            R_Complex.TabStop = true;
            R_Complex.Text = "AT";
            R_Complex.UseVisualStyleBackColor = true;
            R_Complex.Click += R_Complex_Click;
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { файлToolStripMenuItem, оПрограммеToolStripMenuItem, выходToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(983, 24);
            menuStrip1.TabIndex = 1;
            menuStrip1.Text = "menuStrip1";
            // 
            // файлToolStripMenuItem
            // 
            файлToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { загрузитьСигналToolStripMenuItem, сохранитьСигналВNPYToolStripMenuItem, сохранитьРазметкуToolStripMenuItem, тестовыйСигналToolStripMenuItem });
            файлToolStripMenuItem.Name = "файлToolStripMenuItem";
            файлToolStripMenuItem.Size = new Size(37, 20);
            файлToolStripMenuItem.Text = "File";
            // 
            // загрузитьСигналToolStripMenuItem
            // 
            загрузитьСигналToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { загрузитьСигналToolStripMenuItem1, подгрузитьРазметкуToolStripMenuItem, разметкуXLSXToolStripMenuItem, добавитьРазметкуToolStripMenuItem, загрузитьРазметкуЧастиКаналовToolStripMenuItem });
            загрузитьСигналToolStripMenuItem.Name = "загрузитьСигналToolStripMenuItem";
            загрузитьСигналToolStripMenuItem.Size = new Size(186, 22);
            загрузитьСигналToolStripMenuItem.Text = "Загрузить";
            // 
            // загрузитьСигналToolStripMenuItem1
            // 
            загрузитьСигналToolStripMenuItem1.Name = "загрузитьСигналToolStripMenuItem1";
            загрузитьСигналToolStripMenuItem1.Size = new Size(263, 22);
            загрузитьСигналToolStripMenuItem1.Text = "Загрузить сигнал";
            загрузитьСигналToolStripMenuItem1.Click += загрузитьСигналToolStripMenuItem1_Click_1;
            // 
            // подгрузитьРазметкуToolStripMenuItem
            // 
            подгрузитьРазметкуToolStripMenuItem.Name = "подгрузитьРазметкуToolStripMenuItem";
            подгрузитьРазметкуToolStripMenuItem.Size = new Size(263, 22);
            подгрузитьРазметкуToolStripMenuItem.Text = "Загрузить разметку";
            подгрузитьРазметкуToolStripMenuItem.Click += подгрузитьРазметкуToolStripMenuItem_Click_1;
            // 
            // разметкуXLSXToolStripMenuItem
            // 
            разметкуXLSXToolStripMenuItem.Name = "разметкуXLSXToolStripMenuItem";
            разметкуXLSXToolStripMenuItem.Size = new Size(263, 22);
            разметкуXLSXToolStripMenuItem.Text = "Загрузить разметку XLSX";
            разметкуXLSXToolStripMenuItem.Click += разметкуXLSXToolStripMenuItem_Click;
            // 
            // добавитьРазметкуToolStripMenuItem
            // 
            добавитьРазметкуToolStripMenuItem.Name = "добавитьРазметкуToolStripMenuItem";
            добавитьРазметкуToolStripMenuItem.Size = new Size(263, 22);
            добавитьРазметкуToolStripMenuItem.Text = "Добавить разметку";
            добавитьРазметкуToolStripMenuItem.Click += добавитьРазметкуToolStripMenuItem_Click;
            // 
            // загрузитьРазметкуЧастиКаналовToolStripMenuItem
            // 
            загрузитьРазметкуЧастиКаналовToolStripMenuItem.Name = "загрузитьРазметкуЧастиКаналовToolStripMenuItem";
            загрузитьРазметкуЧастиКаналовToolStripMenuItem.Size = new Size(263, 22);
            загрузитьРазметкуЧастиКаналовToolStripMenuItem.Text = "Загрузить разметку части каналов";
            загрузитьРазметкуЧастиКаналовToolStripMenuItem.Click += загрузитьРазметкуЧастиКаналовToolStripMenuItem_Click;
            // 
            // сохранитьСигналВNPYToolStripMenuItem
            // 
            сохранитьСигналВNPYToolStripMenuItem.Name = "сохранитьСигналВNPYToolStripMenuItem";
            сохранитьСигналВNPYToolStripMenuItem.Size = new Size(186, 22);
            сохранитьСигналВNPYToolStripMenuItem.Text = "Сохранить сигнал";
            сохранитьСигналВNPYToolStripMenuItem.Click += сохранитьСигналВNPYToolStripMenuItem_Click;
            // 
            // сохранитьРазметкуToolStripMenuItem
            // 
            сохранитьРазметкуToolStripMenuItem.Name = "сохранитьРазметкуToolStripMenuItem";
            сохранитьРазметкуToolStripMenuItem.Size = new Size(186, 22);
            сохранитьРазметкуToolStripMenuItem.Text = "Сохранить разметку";
            сохранитьРазметкуToolStripMenuItem.Click += сохранитьРазметкуToolStripMenuItem_Click;
            // 
            // тестовыйСигналToolStripMenuItem
            // 
            тестовыйСигналToolStripMenuItem.Name = "тестовыйСигналToolStripMenuItem";
            тестовыйСигналToolStripMenuItem.Size = new Size(186, 22);
            тестовыйСигналToolStripMenuItem.Text = "Тестовый сигнал";
            тестовыйСигналToolStripMenuItem.Click += тестовыйСигналToolStripMenuItem_Click;
            // 
            // оПрограммеToolStripMenuItem
            // 
            оПрограммеToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { информацияToolStripMenuItem });
            оПрограммеToolStripMenuItem.Name = "оПрограммеToolStripMenuItem";
            оПрограммеToolStripMenuItem.Size = new Size(52, 20);
            оПрограммеToolStripMenuItem.Text = "About";
            // 
            // информацияToolStripMenuItem
            // 
            информацияToolStripMenuItem.Name = "информацияToolStripMenuItem";
            информацияToolStripMenuItem.Size = new Size(180, 22);
            информацияToolStripMenuItem.Text = "Информация";
            информацияToolStripMenuItem.Click += информацияToolStripMenuItem_Click;
            // 
            // выходToolStripMenuItem
            // 
            выходToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { кВыборуРежимаToolStripMenuItem, закрытьПрограммуToolStripMenuItem });
            выходToolStripMenuItem.Name = "выходToolStripMenuItem";
            выходToolStripMenuItem.Size = new Size(38, 20);
            выходToolStripMenuItem.Text = "Exit";
            выходToolStripMenuItem.Click += выходToolStripMenuItem_Click;
            // 
            // кВыборуРежимаToolStripMenuItem
            // 
            кВыборуРежимаToolStripMenuItem.Name = "кВыборуРежимаToolStripMenuItem";
            кВыборуРежимаToolStripMenuItem.Size = new Size(186, 22);
            кВыборуРежимаToolStripMenuItem.Text = "К выбору режима";
            кВыборуРежимаToolStripMenuItem.Click += кВыборуРежимаToolStripMenuItem_Click;
            // 
            // закрытьПрограммуToolStripMenuItem
            // 
            закрытьПрограммуToolStripMenuItem.Name = "закрытьПрограммуToolStripMenuItem";
            закрытьПрограммуToolStripMenuItem.Size = new Size(186, 22);
            закрытьПрограммуToolStripMenuItem.Text = "Закрыть программу";
            закрытьПрограммуToolStripMenuItem.Click += закрытьПрограммуToolStripMenuItem_Click;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(MarkupsPaths);
            groupBox2.Controls.Add(SignalPath);
            groupBox2.Controls.Add(SampleRateLabel);
            groupBox2.Controls.Add(FullSignalToNoise);
            groupBox2.Controls.Add(button1);
            groupBox2.Controls.Add(ScrollStepLabel);
            groupBox2.Controls.Add(ScrollStepTitle);
            groupBox2.Controls.Add(ScrollStepValue);
            groupBox2.Controls.Add(ATSegmentWidth);
            groupBox2.Controls.Add(ATSegmentWidthTitle);
            groupBox2.Controls.Add(ATSegmentWidthValue);
            groupBox2.Controls.Add(SamplerateTitle);
            groupBox2.Controls.Add(ChannelsList);
            groupBox2.Controls.Add(ChannelTitle);
            groupBox2.Font = new Font("Segoe UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 204);
            groupBox2.Location = new Point(761, 174);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(210, 375);
            groupBox2.TabIndex = 2;
            groupBox2.TabStop = false;
            groupBox2.Text = "Information about markup";
            // 
            // MarkupsPaths
            // 
            MarkupsPaths.Location = new Point(6, 49);
            MarkupsPaths.Name = "MarkupsPaths";
            MarkupsPaths.Size = new Size(198, 88);
            MarkupsPaths.TabIndex = 14;
            MarkupsPaths.Text = "Markup";
            MarkupsPaths.MouseHover += MarkupsPaths_MouseHover;
            // 
            // SignalPath
            // 
            SignalPath.Location = new Point(6, 19);
            SignalPath.Name = "SignalPath";
            SignalPath.Size = new Size(198, 30);
            SignalPath.TabIndex = 13;
            SignalPath.Text = "Signal";
            SignalPath.MouseHover += SignalPath_MouseHover;
            // 
            // SampleRateLabel
            // 
            SampleRateLabel.AutoSize = true;
            SampleRateLabel.Location = new Point(157, 166);
            SampleRateLabel.Name = "SampleRateLabel";
            SampleRateLabel.Size = new Size(36, 25);
            SampleRateLabel.TabIndex = 12;
            SampleRateLabel.Text = "---";
            // 
            // FullSignalToNoise
            // 
            FullSignalToNoise.Location = new Point(6, 347);
            FullSignalToNoise.Name = "FullSignalToNoise";
            FullSignalToNoise.Size = new Size(198, 22);
            FullSignalToNoise.TabIndex = 11;
            FullSignalToNoise.Text = "Mark signal as noise";
            FullSignalToNoise.UseVisualStyleBackColor = true;
            FullSignalToNoise.Click += FullSignalToNoise_Click;
            // 
            // button1
            // 
            button1.Location = new Point(6, 316);
            button1.Name = "button1";
            button1.Size = new Size(198, 25);
            button1.TabIndex = 10;
            button1.Text = "Clear visible markup";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // ScrollStepLabel
            // 
            ScrollStepLabel.AutoSize = true;
            ScrollStepLabel.Location = new Point(166, 247);
            ScrollStepLabel.Name = "ScrollStepLabel";
            ScrollStepLabel.Size = new Size(22, 25);
            ScrollStepLabel.TabIndex = 9;
            ScrollStepLabel.Text = "0";
            // 
            // ScrollStepTitle
            // 
            ScrollStepTitle.AutoSize = true;
            ScrollStepTitle.Location = new Point(6, 247);
            ScrollStepTitle.Name = "ScrollStepTitle";
            ScrollStepTitle.Size = new Size(126, 25);
            ScrollStepTitle.TabIndex = 8;
            ScrollStepTitle.Text = "Scrolling step";
            // 
            // ScrollStepValue
            // 
            ScrollStepValue.Location = new Point(6, 265);
            ScrollStepValue.Name = "ScrollStepValue";
            ScrollStepValue.Size = new Size(198, 45);
            ScrollStepValue.TabIndex = 7;
            ScrollStepValue.Scroll += ScrollStepValue_Scroll;
            // 
            // ATSegmentWidth
            // 
            ATSegmentWidth.AutoSize = true;
            ATSegmentWidth.DataBindings.Add(new Binding("Text", segmentationEventHandlerBindingSource1, "SegmentWidth", true));
            ATSegmentWidth.Location = new Point(166, 181);
            ATSegmentWidth.Name = "ATSegmentWidth";
            ATSegmentWidth.Size = new Size(22, 25);
            ATSegmentWidth.TabIndex = 6;
            ATSegmentWidth.Text = "0";
            // 
            // segmentationEventHandlerBindingSource1
            // 
            segmentationEventHandlerBindingSource1.DataSource = typeof(Segmentation.SegmentationEventHandler);
            // 
            // ATSegmentWidthTitle
            // 
            ATSegmentWidthTitle.AutoSize = true;
            ATSegmentWidthTitle.Location = new Point(6, 181);
            ATSegmentWidthTitle.Name = "ATSegmentWidthTitle";
            ATSegmentWidthTitle.Size = new Size(138, 25);
            ATSegmentWidthTitle.TabIndex = 5;
            ATSegmentWidthTitle.Text = "Segment width";
            // 
            // ATSegmentWidthValue
            // 
            ATSegmentWidthValue.Location = new Point(6, 199);
            ATSegmentWidthValue.Maximum = 200;
            ATSegmentWidthValue.Minimum = 10;
            ATSegmentWidthValue.Name = "ATSegmentWidthValue";
            ATSegmentWidthValue.Size = new Size(198, 45);
            ATSegmentWidthValue.TabIndex = 4;
            ATSegmentWidthValue.Value = 50;
            // 
            // SamplerateTitle
            // 
            SamplerateTitle.AutoSize = true;
            SamplerateTitle.Location = new Point(6, 166);
            SamplerateTitle.Name = "SamplerateTitle";
            SamplerateTitle.Size = new Size(153, 25);
            SamplerateTitle.TabIndex = 2;
            SamplerateTitle.Text = "Signal frequency";
            // 
            // ChannelsList
            // 
            ChannelsList.FormattingEnabled = true;
            ChannelsList.Location = new Point(132, 140);
            ChannelsList.Name = "ChannelsList";
            ChannelsList.Size = new Size(72, 33);
            ChannelsList.TabIndex = 1;
            // 
            // ChannelTitle
            // 
            ChannelTitle.AutoSize = true;
            ChannelTitle.Location = new Point(6, 143);
            ChannelTitle.Name = "ChannelTitle";
            ChannelTitle.Size = new Size(148, 25);
            ChannelTitle.TabIndex = 0;
            ChannelTitle.Text = "Current channel";
            // 
            // SignalMarkup
            // 
            SignalMarkup.BackColor = Color.White;
            SignalMarkup.Location = new Point(12, 39);
            SignalMarkup.Name = "SignalMarkup";
            SignalMarkup.PanCursor = Cursors.Hand;
            SignalMarkup.Size = new Size(729, 510);
            SignalMarkup.TabIndex = 3;
            SignalMarkup.Text = "plotView1";
            SignalMarkup.ZoomHorizontalCursor = Cursors.SizeWE;
            SignalMarkup.ZoomRectangleCursor = Cursors.SizeNWSE;
            SignalMarkup.ZoomVerticalCursor = Cursors.SizeNS;
            // 
            // OpenSignalDialog
            // 
            OpenSignalDialog.FileName = "openFileDialog1";
            // 
            // OpenMarkupDialog
            // 
            OpenMarkupDialog.FileName = "openFileDialog1";
            // 
            // hScrollBar1
            // 
            hScrollBar1.Location = new Point(12, 552);
            hScrollBar1.Name = "hScrollBar1";
            hScrollBar1.Size = new Size(729, 17);
            hScrollBar1.TabIndex = 4;
            // 
            // EGM_Markup
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(983, 576);
            Controls.Add(hScrollBar1);
            Controls.Add(SignalMarkup);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            Name = "EGM_Markup";
            Text = "EGM_Markup";
            Resize += EGM_Markup_Resize;
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)ScrollStepValue).EndInit();
            ((System.ComponentModel.ISupportInitialize)segmentationEventHandlerBindingSource1).EndInit();
            ((System.ComponentModel.ISupportInitialize)ATSegmentWidthValue).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private GroupBox groupBox1;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem файлToolStripMenuItem;
        private ToolStripMenuItem оПрограммеToolStripMenuItem;
        private ToolStripMenuItem информацияToolStripMenuItem;
        private RadioButton Artifact;
        private RadioButton Noise;
        private RadioButton T_Complex;
        private RadioButton R_Complex;
        private GroupBox groupBox2;
        private ToolStripMenuItem загрузитьСигналToolStripMenuItem;
        private ToolStripMenuItem сохранитьРазметкуToolStripMenuItem;
        private OxyPlot.WindowsForms.PlotView SignalMarkup;
        private ToolStripMenuItem выходToolStripMenuItem;
        private ToolStripMenuItem тестовыйСигналToolStripMenuItem;
        private ToolStripMenuItem кВыборуРежимаToolStripMenuItem;
        private ToolStripMenuItem закрытьПрограммуToolStripMenuItem;
        private OpenFileDialog OpenSignalDialog;
        private OpenFileDialog OpenMarkupDialog;
        private Label SamplerateTitle;
        private ComboBox ChannelsList;
        private Label ChannelTitle;
        private SaveFileDialog SaveMarkupDialog;
        private Label ATSegmentWidth;
        private Label ATSegmentWidthTitle;
        private TrackBar ATSegmentWidthValue;
        private BindingSource segmentationEventHandlerBindingSource1;
        private TrackBar ScrollStepValue;
        private Label ScrollStepLabel;
        private Label ScrollStepTitle;
        private Button button1;
        private Button FullSignalToNoise;
        private ToolStripMenuItem загрузитьСигналToolStripMenuItem1;
        private ToolStripMenuItem подгрузитьРазметкуToolStripMenuItem;
        private ToolStripMenuItem сохранитьСигналВNPYToolStripMenuItem;
        private ToolStripMenuItem добавитьРазметкуToolStripMenuItem;
        private Label SampleRateLabel;
        private ToolStripMenuItem разметкуXLSXToolStripMenuItem;
        private Label MarkupsPaths;
        private Label SignalPath;
        private ToolTip toolTip1;
        private ToolStripMenuItem загрузитьРазметкуЧастиКаналовToolStripMenuItem;
        private HScrollBar hScrollBar1;
    }
}