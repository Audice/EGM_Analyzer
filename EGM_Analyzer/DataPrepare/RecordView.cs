using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EGM_Analyzer.DataPrepare
{
    public class RecordView
    {
        TabViewHandler _tabViewHandler;
        public PreparingRecord PreparingRecord
        {
            get; private set;
        }
        private TabPage _page;

        private Label _signalTitle = new Label() { Text = "Сигнал"};
        private Label _markupTitle = new Label() { Text = "Разметка" };
        private Label _signalPath = new Label() { Text = "", AutoSize = false };
        private ListBox _markupsPaths = new ListBox();

        private Button _removeSelectedMarkup = new Button() { Text = "Удалить разметку" };
        private Button _appendMarkup = new Button() { Text = "Добавить разметку" };
        private Button _removeRecord = new Button() { Text = "Удалить запись" };

        public RecordView(int tabWidth, int tabHeight, PreparingRecord preparingRecord, TabViewHandler tabViewHandler)
        {
            PreparingRecord = preparingRecord;
            _page = new TabPage() { Size = new Size(tabWidth, tabHeight), Text = preparingRecord.SignalName()};
            _removeSelectedMarkup.Click += removeSelectedMarkup;
            _appendMarkup.Click += appendMarkup;
            _removeRecord.Click += removeRecord;
            _tabViewHandler = tabViewHandler;

            ControlsTabBuild(tabWidth, tabHeight);

            _page.Controls.Add(_signalTitle);
            _page.Controls.Add(_signalPath);
            _page.Controls.Add(_markupTitle);
            _page.Controls.Add(_markupsPaths);
            _page.Controls.Add(_appendMarkup);
            _page.Controls.Add(_removeSelectedMarkup);
            _page.Controls.Add(_removeRecord);

        }

        void ControlsTabBuild(int tabWidth, int tabHeight)
        {
            //Настройка расположения GUI элементов
            int borderPaddingWidth = tabWidth / 40;
            int borderPaddingHeight = tabHeight / 40;
            int titleWidth = (tabWidth - 3 * borderPaddingWidth) / 10;
            int titleHeight = (tabHeight - 4 * borderPaddingHeight) / 18;

            _signalTitle.Location = new Point(borderPaddingWidth, borderPaddingHeight);
            _signalTitle.Width = 2 * titleWidth;

            _signalPath.Location = new Point(borderPaddingWidth + _signalTitle.Width, borderPaddingHeight);
            _signalPath.Width = 8 * titleWidth - borderPaddingWidth;
            _signalPath.Height = 3 * titleHeight;
            _signalPath.Text = PreparingRecord.SignalPath;


            _markupTitle.Location = new Point(borderPaddingWidth, _signalPath.Height + 2 * borderPaddingHeight);
            _markupTitle.Width = 2 * titleWidth;

            _markupsPaths.Location = new Point(2 * borderPaddingWidth + _markupTitle.Width, _signalPath.Height + 2 * borderPaddingHeight);
            _markupsPaths.Size = new Size(8 * titleWidth, 11 * titleHeight);


            foreach (var markupName in PreparingRecord.MarkupsNames)
                _markupsPaths.Items.Add(markupName);


            //Кнопки
            int buttonWidth = (tabWidth - 5 * borderPaddingWidth) / 3;

            _appendMarkup.Location = new Point(borderPaddingWidth, 14 * titleHeight + 2 * borderPaddingHeight);
            _appendMarkup.Size = new Size(buttonWidth, 3 * titleHeight);

            _removeSelectedMarkup.Location = new Point(2 * borderPaddingWidth + buttonWidth, 14 * titleHeight + 2 * borderPaddingHeight);
            _removeSelectedMarkup.Size = new Size(buttonWidth, 3 * titleHeight);

            _removeRecord.Location = new Point(3 * borderPaddingWidth + 2 * buttonWidth, 14 * titleHeight + 2 * borderPaddingHeight);
            _removeRecord.Size = new Size(buttonWidth, 3 * titleHeight);
        }

        private void removeRecord(object? sender, EventArgs e)
        {
            _removeSelectedMarkup.Click -= removeSelectedMarkup;
            _appendMarkup.Click -= appendMarkup;
            _removeRecord.Click -= removeRecord;
            _tabViewHandler.RemoveTab(this);
        }

        private void appendMarkup(object? sender, EventArgs e)
        {
            try
            {
                PreparingRecord.AppendMarkups(_tabViewHandler.GetMarkupFiles());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Попытка добавить существующий файл!", "Ошибка добавления разметки",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void removeSelectedMarkup(object? sender, EventArgs e)
        {
            if (_markupsPaths.SelectedIndex != -1)
            {
                string? selectedItem = (string)_markupsPaths.SelectedItem;
                if (selectedItem != null) {
                    PreparingRecord.RemoveMarkup(selectedItem);
                    _markupsPaths.Items.RemoveAt(_markupsPaths.SelectedIndex);
                }
            }
        }

        public TabPage GetTabPage()
        {
            return _page;
        }

    }
}
