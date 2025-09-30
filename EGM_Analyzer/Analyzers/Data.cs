using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace EGM_Analyzer.Analyzers
{
    public class Data<T>
    {
        /// <summary>
        /// Размерность данных для анализа
        /// </summary>
        public long[]? Shape
        {
            get; set;
        }

        public Array? Signal
        {
            get; set;
        }

        public long? StartSample
        {
            get; set;
        }

        public long? EndSample
        {
            get; set;
        }



        public T[] GetOneDimData()
        {
            if (Signal == null)
                throw new ArgumentNullException("Попытка получения сигнала, который не проинициализирован");
            return ArrayExtensions.Flatten<T>(Signal);
        }



    }

    public static class ArrayExtensions
    {
        public static T[] Flatten<T>(this Array array)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));

            T[] result = new T[array.Length];
            int index = 0;

            // Используем многомерный итератор
            var indices = new int[array.Rank];
            FlattenRecursive(array, result, ref index, indices, 0);

            return result;
        }

        private static void FlattenRecursive<T>(Array source, T[] destination, ref int index,
                                              int[] indices, int currentDimension)
        {
            if (currentDimension == source.Rank - 1)
            {
                // Последнее измерение - заполняем значения
                for (int i = 0; i < source.GetLength(currentDimension); i++)
                {
                    indices[currentDimension] = i;
                    destination[index++] = (T)source.GetValue(indices);
                }
            }
            else
            {
                // Рекурсивно обходим измерения
                for (int i = 0; i < source.GetLength(currentDimension); i++)
                {
                    indices[currentDimension] = i;
                    FlattenRecursive(source, destination, ref index, indices, currentDimension + 1);
                }
            }
        }
    }
}
