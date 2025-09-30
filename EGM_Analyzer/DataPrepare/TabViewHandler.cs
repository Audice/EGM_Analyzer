using ParseData.Electrogram;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EGM_Analyzer.DataPrepare
{
    public class TabViewHandler
    {
        List<RecordView> _recordsVeiw = new List<RecordView>();

        private TabControl _recordsTab;
        public OpenFileDialog OpenRecord { get; }

        private const string SignalFilter = "Numpy files(*.npy)|*.npy|Binary files(*.bin)|*.bin";
        private const string MarkupFilter = "JSON files(*.json)|*.json";

        public TabViewHandler(TabControl recordsTab, OpenFileDialog openRecord)
        {
            _recordsTab = recordsTab;
            OpenRecord = openRecord;
        }

        public void AppendRecord()
        {
            OpenRecord.Filter = SignalFilter;
            if (OpenRecord.ShowDialog() == DialogResult.Cancel)
                return;
            string signalPath = OpenRecord.FileName;

            List<string> markupsFiles = GetMarkupFiles();

            if (IsReadd(signalPath, markupsFiles))
            {
                MessageBox.Show("Попытка добавить существующий сигнал или разметку!", "Ошибка добавления записи",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            PreparingRecord? pr = null;
            try
            {
                pr = new PreparingRecord(signalPath, markupsFiles);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Попытка добавить существующий файл!", "Ошибка добавления разметки",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            RecordView recordView = new RecordView(_recordsTab.Width, _recordsTab.Height, pr, this);
            _recordsVeiw.Add(recordView);

            _recordsTab.TabPages.Add(recordView.GetTabPage());
        }

        public void AppendRecords(List<PreparingRecord> preparingRecords)
        {
            _recordsVeiw.Clear();
            _recordsTab.TabPages.Clear();

            foreach (var prerparingRecord in preparingRecords)
            {
                RecordView recordView = new RecordView(_recordsTab.Width, _recordsTab.Height, prerparingRecord, this);
                _recordsVeiw.Add(recordView);
                _recordsTab.TabPages.Add(recordView.GetTabPage());
            }
        }

        public List<PreparingRecord> GetPreparingRecords()
        {
            List<PreparingRecord> preparingRecords = new List<PreparingRecord>();
            foreach (var recordView in _recordsVeiw)
            {
                preparingRecords.Add(recordView.PreparingRecord);
            }
            return preparingRecords;
        }

        private bool IsReadd(string signalPath, List<string> markupsFiles)
        {
            foreach(var recordView in _recordsVeiw)
            {
                if (recordView.PreparingRecord.SignalPath == signalPath
                    || recordView.PreparingRecord.IsMarkupsMatches(markupsFiles))
                {
                    return true;
                }
            }
            return false;
        }

        public List<string> GetMarkupFiles()
        {
            OpenRecord.Filter = MarkupFilter;
            OpenRecord.Multiselect = true;
            List<string> markupsFiles = new List<string>();
            DialogResult dr = OpenRecord.ShowDialog();
            if (dr == System.Windows.Forms.DialogResult.OK)
            {
                foreach (string file in OpenRecord.FileNames)
                {
                    markupsFiles.Add(file);
                }
            }
            OpenRecord.Multiselect = false;
            return markupsFiles;
        }



        public void RemoveTab(RecordView recordView) // TabPage page)
        {
            _recordsTab.TabPages.Remove(recordView.GetTabPage());
            _recordsVeiw.Remove(recordView);
        }
    }
}
