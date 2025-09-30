using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseData.Electrogram
{
    internal static class ParseFunctions
    {
        private static readonly byte[] END_PATTERN = { 13, 10 };

        public static DateTime ParseDate(byte[] rawData, int version)
        {
            string dateTime = System.Text.Encoding.Default.GetString(rawData);
            if (version >= 17)
                return DateTime.ParseExact(dateTime, "MM/d/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            return DateTime.ParseExact(dateTime, "M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture);
        }
        public static long ParseLong(byte[] rawData)
        {
            long outValue = 0;
            return long.TryParse(System.Text.Encoding.Default.GetString(rawData), out outValue) ? outValue : -1;
        }
        public static Guid ParseGuid(byte[] rawData)
        {
            Guid curGuid;
            return Guid.TryParse(System.Text.Encoding.Default.GetString(rawData), out curGuid) ? curGuid : Guid.Empty;
        }

        public static byte[] ParseHeader(byte[] rawData, byte[] headerBytes)
        {
            var matchIndexes =
                from index in Enumerable.Range(0, rawData.Length - headerBytes.Length + 1)
                where rawData.Skip(index).Take(headerBytes.Length).SequenceEqual(headerBytes)
                select (int?)index;

            int? matchIndex = matchIndexes.FirstOrDefault();

            if (matchIndex != null)
            {
                var rnIndexes =
                    from index in Enumerable.Range(matchIndex.Value, rawData.Length - END_PATTERN.Length + 1)
                    where rawData.Skip(index).Take(END_PATTERN.Length).SequenceEqual(END_PATTERN)
                    select (int?)index;

                int? endIndex = rnIndexes.FirstOrDefault();
                if (endIndex != null)
                {
                    return GetValidValue(rawData, matchIndex.Value + headerBytes.Length + 1, endIndex.Value);
                }
            }
            return null;
        }

        private static byte[] GetValidValue(byte[] bytes, int firstIndex, int lastIndex)
        {
            byte[] part = new byte[lastIndex - firstIndex];
            Array.Copy(bytes, firstIndex, part, 0, part.Length);
            int validIndex = -1;
            for (int i = part.Length - 1; i >= 0; i--)
            {
                if (part[i] != ' ')
                {
                    validIndex = i;
                    break;
                }
            }
            byte[] validArray = new byte[validIndex + 1];
            Array.Copy(part, 0, validArray, 0, validArray.Length);

            return validArray;
        }
    }
}
