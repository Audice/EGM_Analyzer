using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EGM_Analyzer.DataPrepare
{
    public class PreparingRecord
    {
        /// <summary>
        /// Список путей к файлам, в которых располагается разметка сигналов. 
        /// Выбор структуры связан с тем, что разметок может быть несколько.
        /// </summary>
        public List<string> MarkupsPaths
        {
            get; private set;
        } = new List<string>();
        /// <summary>
        /// Список имён файлов с разметкой
        /// </summary>
        public List<string> MarkupsNames
        {
            get; private set;
        } = new List<string>();
        /// <summary>
        /// Путь до сигнала
        /// </summary>
        public string SignalPath
        {
            get; private set;
        } = "";



        public PreparingRecord(string signalPath, List<string> markupPath)
        {
            SignalPath = signalPath;
            AppendMarkups(markupPath);
        }

        /// <summary>
        /// Проверка наличия выбранных путей разметки в уже загруженных
        /// </summary>
        /// <param name="markupPaths"></param>
        /// <returns>False - если совпадений с существующими путями не найдены.</returns>
        public bool IsMarkupsMatches(List<string> markupPaths)
        {
            foreach (string markupPath in markupPaths)
            {
                if (MarkupsPaths.FindIndex(x => x == markupPath) >= 0)
                {
                    return true;
                }
            }
            return false;
        }

        public string SignalName()
        {
            FileInfo signalFileInfo = new FileInfo(SignalPath);
            return signalFileInfo.Name.Replace(signalFileInfo.Extension, "");
        }

        public void AppendMarkups(List<string> markups)
        {
            foreach (var markup in markups)
            {
                AppendMarkup(markup);
            }
        }

        public void AppendMarkup(string filePath)
        {
            FileInfo fileInfo = new FileInfo(filePath);
            string fileName = fileInfo.Name.Replace(".json", "");
            if (!MarkupsNames.Contains(fileName))
            {
                MarkupsPaths.Add(filePath);
                MarkupsNames.Add(fileName);
            }
            else
            {
                throw new Exception("Попытка добавить существующий файл!");
            }
        }

        public void RemoveMarkup(string markupName)
        {
            int indexMarkupName = MarkupsNames.FindIndex(x => x == markupName);
            if ( indexMarkupName >= 0)
            {
                MarkupsNames.RemoveAt(indexMarkupName);
                MarkupsPaths.RemoveAt(indexMarkupName);
            }
        }
    }
}
