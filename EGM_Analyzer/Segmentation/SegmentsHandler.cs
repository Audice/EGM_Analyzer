using EGM_Analyzer.DataLoader.OldMurkup;
using EGM_Analyzer.Save;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EGM_Analyzer.Segmentation
{

    public class SegmentsHandler
    {
        public SegmentationSaver SegmentationSaver
        {
            get; private set;
        }

        public ushort AutosavePeriod
        {
            get; set;
        } = 5;

        private ushort _unsavedChanges = 0;

        private List<SortedList<ulong, Segment>> _channelsSortedSegments;

        public byte ChannelsCount
        {
            get; private set;
        }
        
        public SegmentsHandler(byte channelsCount, FileInfo signalFile, ushort sampleRate, string? usedModel = null)
        {
            
            SegmentationSaver = new SegmentationSaver(signalFile, sampleRate, usedModel);

            _channelsSortedSegments = new List<SortedList<ulong, Segment>>();
            ChannelsCount = channelsCount;
            for (int i = 0; i < channelsCount; i++)
            {
                _channelsSortedSegments.Add(new SortedList<ulong, Segment>());
            }
        }

        /// <summary>
        /// Получить количество сегментов в канале 
        /// </summary>
        /// <param name="channelNumber">Номер канала</param>
        /// <returns>Количество сегментов в канале </returns>
        /// <exception cref="ArgumentException">Номер канала превышает возможное количество каналов</exception>
        public int SegmentsNumber(byte channelNumber)
        {
            if (channelNumber >= _channelsSortedSegments.Count)
                throw new ArgumentException("SegmentsNumber: несовпадение числа каналов в сигнале и текущего сегмента");
            return _channelsSortedSegments[channelNumber].Count;
        }

        /// <summary>
        /// Удалить все сегменты в канале
        /// </summary>
        /// <param name="channelNumber">Индекс канала</param>
        /// <exception cref="ArgumentException">Номер канала превышает возможное количество каналов</exception>
        public void DeletSegments(byte channelNumber)
        {
            if (channelNumber >= _channelsSortedSegments.Count)
                throw new ArgumentException("DeletSegments: несовпадение числа каналов в сигнале и текущего сегмента");
            _channelsSortedSegments[channelNumber].Clear();
            //AutoSave();
        }

        /// <summary>
        /// Удаление отмеченного сегмента
        /// </summary>
        /// <param name="channel">Канал, сегмент которого удаляется</param>
        /// <param name="startMark">Начало удаляемого сегмента</param>
        /// <exception cref="ArgumentException">Отсутствия сегмента или некорректного канала</exception>
        public bool DeleteSegment(byte channel, ulong startMark)
        {
            if (channel >= _channelsSortedSegments.Count)
                throw new ArgumentException("DeleteSegment: несовпадение числа каналов в сигнале и текущего сегмента");
            if (!_channelsSortedSegments[channel].ContainsKey(startMark))
            {
                //Из-за округления ключ сместился вправо на 1 или влево на 1
                bool isKeyConteins = false;
                ulong leftKey = startMark - 1;
                if (_channelsSortedSegments[channel].ContainsKey(leftKey))
                {
                    isKeyConteins = true;
                    startMark = leftKey;
                }
                ulong rightKey = startMark + 1;
                if (_channelsSortedSegments[channel].ContainsKey(rightKey))
                {
                    isKeyConteins = true;
                    startMark = rightKey;
                }

                if (!isKeyConteins)
                    throw new ArgumentException("DeleteSegment: неопределённое поведение - удаляемый сегмент отсутствует.");
            }
            bool result = false;
            result = _channelsSortedSegments[channel].Remove(startMark);
            //AutoSave();
            return result;
        }

        public List<Segment> GetSegments(byte channel)
        {
            if (_channelsSortedSegments.Count <= channel)
                throw new ArgumentException("Некорректный номер канала");

            return _channelsSortedSegments[channel].Values.ToList();
        }

        /// <summary>
        /// Метод добавления сегмента в общий список сегментов по всем каналам. Контроль наложение сегментов присутствует
        /// </summary>
        /// <param name="segment">Интересуемый сегмент</param>
        /// <returns>True если добавление успешно. Иначе False.</returns>
        /// <exception cref="ArgumentException">Запрошен некорректный канал</exception>
        public bool AppendSegment(Segment segment, bool autosave_enable = true)
        {
            if (segment.Channel >= _channelsSortedSegments.Count)
                throw new ArgumentException("AppendSegment: несовпадение числа каналов в сигнале и текущего сегмента");
            
            try
            {
                if (_channelsSortedSegments[segment.Channel].ContainsKey(segment.StartMark))
                    return false;
                _channelsSortedSegments[segment.Channel].Add(segment.StartMark, segment);
            }
            catch (Exception ex)
            {
                //Отказано в добавлении, так как такой элемент уже есть
                return false;
            }
            //Получить индекс элемента в списке
            int segmentIndex = _channelsSortedSegments[segment.Channel].IndexOfKey(segment.StartMark);
            
            bool leftSegment = (segmentIndex - 1 >= 0 
                && _channelsSortedSegments[segment.Channel].GetValueAtIndex(segmentIndex - 1).EndMark < segment.StartMark) || segmentIndex == 0;
            
            bool rightSegment = (segmentIndex + 1 < _channelsSortedSegments[segment.Channel].Count
                && _channelsSortedSegments[segment.Channel].GetValueAtIndex(segmentIndex + 1).StartMark > segment.EndMark) 
                || segmentIndex == _channelsSortedSegments[segment.Channel].Count - 1;
            
            if (leftSegment && rightSegment)
            {
                if (autosave_enable)
                    AutoSave();
                return true;
            }
            //Иначе есть наложение сегментов, что не допустимо.
            //Поэтому сообщаем о невозможность действия и удаляем сегмент из списка
            _channelsSortedSegments[segment.Channel].Remove(segment.StartMark);
            return false;
        }

        public void AppendSegments(List<Murkup[]>? segments)
        {
            if (segments == null) throw new ArgumentNullException("AppendSegment: Список сегментов не создан!");

            for (int channel = 0; channel < segments.Count; channel++)
            {
                for (int i = 0; i < segments[channel].Length; i++)
                {
                    AppendSegment(new Segment((byte)channel, SegmentType.AT,
                        (ulong)segments[channel][i].search_segment[0],
                        (ulong)segments[channel][i].search_segment[1],
                        SegmentationAgent.Human, segments[channel][i].position
                    ), false);
                }
            }
        }

        public void AppendSegments(List<List<Segment>>? segments, byte startChannel, byte endChannel)
        {
            if (segments == null) throw new ArgumentNullException("AppendSegment: Список сегментов не создан!");

            for (int channel = startChannel; channel < endChannel; channel++)
            {
                //Очищаем от старого при добавлении???
                _channelsSortedSegments[channel].Clear();
                for (int i = 0; i < segments[channel].Count; i++)
                {
                    AppendSegment(new Segment((byte)channel,
                        segments[channel][i].Type,
                        (ulong)segments[channel][i].StartMark,
                        (ulong)segments[channel][i].EndMark,
                        SegmentationAgent.Human, segments[channel][i].ComplexMark
                    ), false);
                }
            }
        }

        public void AppendSegments(List<List<Segment>>? segments)
        {
            if (segments == null) throw new ArgumentNullException("AppendSegment: Список сегментов не создан!");

            for (int channel = 0; channel < segments.Count; channel++)
            {
                for (int i = 0; i < segments[channel].Count; i++)
                {
                    AppendSegment(new Segment((byte)channel,
                        segments[channel][i].Type,
                        (ulong)segments[channel][i].StartMark,
                        (ulong)segments[channel][i].EndMark,
                        SegmentationAgent.Human, segments[channel][i].ComplexMark
                    ), false);
                }
            }
        }

        /// <summary>
        /// Функция автосохранения разметки
        /// </summary>
        private void AutoSave()
        {
            if (++_unsavedChanges < AutosavePeriod) return;
            _unsavedChanges = 0;
            List<List<Segment>> segments = new List<List<Segment>>();
            for (int i = 0; i < _channelsSortedSegments.Count; i++) 
            {
                segments.Add(new List<Segment>(_channelsSortedSegments[i].Values));
            }
            SegmentationSaver.AutomaticSave(segments);
        }

        public void Save(string filepath)
        {
            List<List<Segment>> segments = new List<List<Segment>>();
            for (int i = 0; i < _channelsSortedSegments.Count; i++)
            {
                segments.Add(new List<Segment>(_channelsSortedSegments[i].Values));
            }
            SegmentationSaver.Save(segments, filepath);
        }
    }
}
