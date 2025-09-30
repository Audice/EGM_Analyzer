namespace EGM_Analyzer.Forms
{
    partial class DataForming
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
            DataPartPlot = new OxyPlot.WindowsForms.PlotView();
            menuStrip1 = new MenuStrip();
            файлToolStripMenuItem = new ToolStripMenuItem();
            загрузитьToolStripMenuItem = new ToolStripMenuItem();
            загрузитьСигналToolStripMenuItem = new ToolStripMenuItem();
            загрузитьРазметкуToolStripMenuItem = new ToolStripMenuItem();
            установитьПапкуDatasetаToolStripMenuItem = new ToolStripMenuItem();
            кВыборуРежимаToolStripMenuItem = new ToolStripMenuItem();
            закрытьПрограммуToolStripMenuItem = new ToolStripMenuItem();
            groupBox1 = new GroupBox();
            button4 = new Button();
            button2 = new Button();
            button1 = new Button();
            button3 = new Button();
            MarkupRecords = new TabControl();
            groupBox2 = new GroupBox();
            CheckedTargetFramerate = new CheckBox();
            CheckNormalizeSignal = new CheckBox();
            TargetFramerateValue = new ComboBox();
            OverlapDuration = new Label();
            label3 = new Label();
            OverlapTrackBar = new TrackBar();
            PartDurationLabel = new Label();
            label2 = new Label();
            DurationTrackBar = new TrackBar();
            CurrentChannel = new ComboBox();
            label1 = new Label();
            OpenRecord = new OpenFileDialog();
            CheckRandomFile = new Button();
            VisualizeSignalPart = new Label();
            menuStrip1.SuspendLayout();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)OverlapTrackBar).BeginInit();
            ((System.ComponentModel.ISupportInitialize)DurationTrackBar).BeginInit();
            SuspendLayout();
            // 
            // DataPartPlot
            // 
            DataPartPlot.BackColor = Color.White;
            DataPartPlot.Location = new Point(12, 86);
            DataPartPlot.Name = "DataPartPlot";
            DataPartPlot.PanCursor = Cursors.Hand;
            DataPartPlot.Size = new Size(820, 558);
            DataPartPlot.TabIndex = 5;
            DataPartPlot.Text = "plotView1";
            DataPartPlot.ZoomHorizontalCursor = Cursors.SizeWE;
            DataPartPlot.ZoomRectangleCursor = Cursors.SizeNWSE;
            DataPartPlot.ZoomVerticalCursor = Cursors.SizeNS;
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { файлToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(1171, 24);
            menuStrip1.TabIndex = 6;
            menuStrip1.Text = "menuStrip1";
            // 
            // файлToolStripMenuItem
            // 
            файлToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { загрузитьToolStripMenuItem, установитьПапкуDatasetаToolStripMenuItem, кВыборуРежимаToolStripMenuItem, закрытьПрограммуToolStripMenuItem });
            файлToolStripMenuItem.Name = "файлToolStripMenuItem";
            файлToolStripMenuItem.Size = new Size(48, 20);
            файлToolStripMenuItem.Text = "Файл";
            // 
            // загрузитьToolStripMenuItem
            // 
            загрузитьToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { загрузитьСигналToolStripMenuItem, загрузитьРазметкуToolStripMenuItem });
            загрузитьToolStripMenuItem.Name = "загрузитьToolStripMenuItem";
            загрузитьToolStripMenuItem.Size = new Size(222, 22);
            загрузитьToolStripMenuItem.Text = "Загрузить";
            // 
            // загрузитьСигналToolStripMenuItem
            // 
            загрузитьСигналToolStripMenuItem.Name = "загрузитьСигналToolStripMenuItem";
            загрузитьСигналToolStripMenuItem.Size = new Size(181, 22);
            загрузитьСигналToolStripMenuItem.Text = "Загрузить сигнал";
            // 
            // загрузитьРазметкуToolStripMenuItem
            // 
            загрузитьРазметкуToolStripMenuItem.Name = "загрузитьРазметкуToolStripMenuItem";
            загрузитьРазметкуToolStripMenuItem.Size = new Size(181, 22);
            загрузитьРазметкуToolStripMenuItem.Text = "Загрузить разметку";
            // 
            // установитьПапкуDatasetаToolStripMenuItem
            // 
            установитьПапкуDatasetаToolStripMenuItem.Name = "установитьПапкуDatasetаToolStripMenuItem";
            установитьПапкуDatasetаToolStripMenuItem.Size = new Size(222, 22);
            установитьПапкуDatasetаToolStripMenuItem.Text = "Установить папку Dataset'а";
            установитьПапкуDatasetаToolStripMenuItem.Click += установитьПапкуDatasetаToolStripMenuItem_Click;
            // 
            // кВыборуРежимаToolStripMenuItem
            // 
            кВыборуРежимаToolStripMenuItem.Name = "кВыборуРежимаToolStripMenuItem";
            кВыборуРежимаToolStripMenuItem.Size = new Size(222, 22);
            кВыборуРежимаToolStripMenuItem.Text = "К выбору режима";
            кВыборуРежимаToolStripMenuItem.Click += кВыборуРежимаToolStripMenuItem_Click;
            // 
            // закрытьПрограммуToolStripMenuItem
            // 
            закрытьПрограммуToolStripMenuItem.Name = "закрытьПрограммуToolStripMenuItem";
            закрытьПрограммуToolStripMenuItem.Size = new Size(222, 22);
            закрытьПрограммуToolStripMenuItem.Text = "Закрыть программу";
            закрытьПрограммуToolStripMenuItem.Click += закрытьПрограммуToolStripMenuItem_Click;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(button4);
            groupBox1.Controls.Add(button2);
            groupBox1.Controls.Add(button1);
            groupBox1.Controls.Add(button3);
            groupBox1.Controls.Add(MarkupRecords);
            groupBox1.Controls.Add(groupBox2);
            groupBox1.Controls.Add(CurrentChannel);
            groupBox1.Controls.Add(label1);
            groupBox1.Location = new Point(848, 39);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(311, 680);
            groupBox1.TabIndex = 7;
            groupBox1.TabStop = false;
            groupBox1.Text = "Параметры формирования набора данных";
            // 
            // button4
            // 
            button4.Location = new Point(6, 611);
            button4.Name = "button4";
            button4.Size = new Size(146, 54);
            button4.TabIndex = 7;
            button4.Text = "Сформировать датасет для обучения";
            button4.UseVisualStyleBackColor = true;
            button4.Click += button4_Click;
            // 
            // button2
            // 
            button2.Location = new Point(159, 611);
            button2.Name = "button2";
            button2.Size = new Size(146, 54);
            button2.TabIndex = 6;
            button2.Text = "Сформировать базу данных";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // button1
            // 
            button1.Location = new Point(158, 71);
            button1.Name = "button1";
            button1.Size = new Size(146, 38);
            button1.TabIndex = 5;
            button1.Text = "Указать папку с данными";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // button3
            // 
            button3.Location = new Point(6, 70);
            button3.Name = "button3";
            button3.Size = new Size(146, 38);
            button3.TabIndex = 4;
            button3.Text = "Добавить сигнал с разметкой";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // MarkupRecords
            // 
            MarkupRecords.Location = new Point(6, 115);
            MarkupRecords.Name = "MarkupRecords";
            MarkupRecords.SelectedIndex = 0;
            MarkupRecords.Size = new Size(299, 264);
            MarkupRecords.TabIndex = 3;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(CheckedTargetFramerate);
            groupBox2.Controls.Add(CheckNormalizeSignal);
            groupBox2.Controls.Add(TargetFramerateValue);
            groupBox2.Controls.Add(OverlapDuration);
            groupBox2.Controls.Add(label3);
            groupBox2.Controls.Add(OverlapTrackBar);
            groupBox2.Controls.Add(PartDurationLabel);
            groupBox2.Controls.Add(label2);
            groupBox2.Controls.Add(DurationTrackBar);
            groupBox2.Location = new Point(6, 385);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(298, 220);
            groupBox2.TabIndex = 2;
            groupBox2.TabStop = false;
            groupBox2.Text = "Разбить сигнал на участки";
            // 
            // CheckedTargetFramerate
            // 
            CheckedTargetFramerate.AutoSize = true;
            CheckedTargetFramerate.Location = new Point(10, 152);
            CheckedTargetFramerate.Name = "CheckedTargetFramerate";
            CheckedTargetFramerate.Size = new Size(176, 19);
            CheckedTargetFramerate.TabIndex = 9;
            CheckedTargetFramerate.Text = "Привести к единой частоте";
            CheckedTargetFramerate.UseVisualStyleBackColor = true;
            // 
            // CheckNormalizeSignal
            // 
            CheckNormalizeSignal.AutoSize = true;
            CheckNormalizeSignal.Location = new Point(10, 188);
            CheckNormalizeSignal.Name = "CheckNormalizeSignal";
            CheckNormalizeSignal.Size = new Size(186, 19);
            CheckNormalizeSignal.TabIndex = 8;
            CheckNormalizeSignal.Text = "Нормализовать данные [0; 1]";
            CheckNormalizeSignal.UseVisualStyleBackColor = true;
            // 
            // TargetFramerateValue
            // 
            TargetFramerateValue.FormattingEnabled = true;
            TargetFramerateValue.Items.AddRange(new object[] { "1000", "5000", "10000", "20000" });
            TargetFramerateValue.Location = new Point(207, 149);
            TargetFramerateValue.Name = "TargetFramerateValue";
            TargetFramerateValue.Size = new Size(85, 23);
            TargetFramerateValue.TabIndex = 6;
            // 
            // OverlapDuration
            // 
            OverlapDuration.AutoSize = true;
            OverlapDuration.Location = new Point(251, 85);
            OverlapDuration.Name = "OverlapDuration";
            OverlapDuration.Size = new Size(12, 15);
            OverlapDuration.TabIndex = 5;
            OverlapDuration.Text = "-";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(10, 85);
            label3.Name = "label3";
            label3.Size = new Size(78, 15);
            label3.TabIndex = 4;
            label3.Text = "Нахлёст (мс)";
            // 
            // OverlapTrackBar
            // 
            OverlapTrackBar.Location = new Point(10, 103);
            OverlapTrackBar.Maximum = 150;
            OverlapTrackBar.Minimum = 10;
            OverlapTrackBar.Name = "OverlapTrackBar";
            OverlapTrackBar.Size = new Size(282, 45);
            OverlapTrackBar.TabIndex = 3;
            OverlapTrackBar.Value = 20;
            OverlapTrackBar.ValueChanged += OverlapTrackBar_ValueChanged;
            // 
            // PartDurationLabel
            // 
            PartDurationLabel.AutoSize = true;
            PartDurationLabel.Location = new Point(251, 28);
            PartDurationLabel.Name = "PartDurationLabel";
            PartDurationLabel.Size = new Size(12, 15);
            PartDurationLabel.TabIndex = 2;
            PartDurationLabel.Text = "-";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(10, 28);
            label2.Name = "label2";
            label2.Size = new Size(155, 15);
            label2.TabIndex = 1;
            label2.Text = "Длительность участка (мс)";
            // 
            // DurationTrackBar
            // 
            DurationTrackBar.Location = new Point(9, 46);
            DurationTrackBar.Maximum = 4000;
            DurationTrackBar.Minimum = 500;
            DurationTrackBar.Name = "DurationTrackBar";
            DurationTrackBar.Size = new Size(283, 45);
            DurationTrackBar.TabIndex = 0;
            DurationTrackBar.Value = 2000;
            DurationTrackBar.ValueChanged += DurationTrackBar_ValueChanged;
            // 
            // CurrentChannel
            // 
            CurrentChannel.FormattingEnabled = true;
            CurrentChannel.Location = new Point(174, 29);
            CurrentChannel.Name = "CurrentChannel";
            CurrentChannel.Size = new Size(121, 23);
            CurrentChannel.TabIndex = 1;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(16, 32);
            label1.Name = "label1";
            label1.Size = new Size(91, 15);
            label1.TabIndex = 0;
            label1.Text = "Текущий канал";
            // 
            // OpenRecord
            // 
            OpenRecord.FileName = "openFileDialog1";
            // 
            // CheckRandomFile
            // 
            CheckRandomFile.Location = new Point(12, 650);
            CheckRandomFile.Name = "CheckRandomFile";
            CheckRandomFile.Size = new Size(820, 69);
            CheckRandomFile.TabIndex = 8;
            CheckRandomFile.Text = "Проверить рандомный файл из сформированного датасета";
            CheckRandomFile.UseVisualStyleBackColor = true;
            CheckRandomFile.Click += CheckRandomFile_Click;
            // 
            // VisualizeSignalPart
            // 
            VisualizeSignalPart.Font = new Font("Segoe UI", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 204);
            VisualizeSignalPart.Location = new Point(12, 39);
            VisualizeSignalPart.Name = "VisualizeSignalPart";
            VisualizeSignalPart.Size = new Size(820, 44);
            VisualizeSignalPart.TabIndex = 9;
            VisualizeSignalPart.Text = "-";
            // 
            // DataForming
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1171, 731);
            Controls.Add(VisualizeSignalPart);
            Controls.Add(CheckRandomFile);
            Controls.Add(groupBox1);
            Controls.Add(DataPartPlot);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            Name = "DataForming";
            Text = "DataForming";
            Resize += DataForming_Resize;
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)OverlapTrackBar).EndInit();
            ((System.ComponentModel.ISupportInitialize)DurationTrackBar).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private OxyPlot.WindowsForms.PlotView DataPartPlot;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem файлToolStripMenuItem;
        private ToolStripMenuItem загрузитьToolStripMenuItem;
        private ToolStripMenuItem загрузитьСигналToolStripMenuItem;
        private ToolStripMenuItem загрузитьРазметкуToolStripMenuItem;
        private GroupBox groupBox1;
        private ComboBox CurrentChannel;
        private Label label1;
        private GroupBox groupBox2;
        private TabControl MarkupRecords;
        private Button button3;
        private OpenFileDialog OpenRecord;
        private Button button1;
        private Button button2;
        private Button button4;
        private TrackBar DurationTrackBar;
        private Label PartDurationLabel;
        private Label label2;
        private Label label3;
        private TrackBar OverlapTrackBar;
        private Label OverlapDuration;
        private ComboBox TargetFramerateValue;
        private CheckBox CheckNormalizeSignal;
        private CheckBox CheckedTargetFramerate;
        private Button CheckRandomFile;
        private Label VisualizeSignalPart;
        private ToolStripMenuItem установитьПапкуDatasetаToolStripMenuItem;
        private ToolStripMenuItem кВыборуРежимаToolStripMenuItem;
        private ToolStripMenuItem закрытьПрограммуToolStripMenuItem;
    }
}