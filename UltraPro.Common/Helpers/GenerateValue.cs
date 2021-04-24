using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace UltraPro.Common.Helpers
{
    public static class GenerateValue
    {
        public static string GetRandomAlphanumericString(int length)
        {
            const string alphanumericCharacters =
                "ABCDEFGHIJKLMNOPQRSTUVWXYZ" +
                "abcdefghijklmnopqrstuvwxyz" +
                "0123456789";

            return GetRandomString(length, alphanumericCharacters);
        }

        public static string GetUniqueRandomAlphanumericString(uint number, int length)
        {
            const string alphanumericCharacters = "ZUisAjked5Bu0ClmDfabcN67OEtFGh8HrI9pJ14KLoMvygzqPwxQRS23nTVWXY";
            return GetUniqueRandomString(number, length, alphanumericCharacters.ToCharArray());
        }

        public static string GetRandomString(int length, IEnumerable<char> characterSet)
        {
            if (length < 0)
                throw new ArgumentException("Length must not be negative.", "Length");
            if (length > int.MaxValue / 8) // 250 million chars ought to be enough for anybody
                throw new ArgumentException("Length is too big.", "Length");
            if (characterSet == null)
                throw new ArgumentNullException("CharacterSet");

            var characterArray = characterSet.Distinct().ToArray();
            if (characterArray.Length == 0)
                throw new ArgumentException("CharacterSet must not be empty.", "CharacterSet");

            var bytes = new byte[length * 8];
            var result = new char[length];

            using (var cryptoProvider = new RNGCryptoServiceProvider())
            {
                cryptoProvider.GetBytes(bytes);
            }

            for (int i = 0; i < length; i++)
            {
                ulong value = BitConverter.ToUInt64(bytes, i * 8);
                result[i] = characterArray[value % (uint)characterArray.Length];
            }

            return new string(result);
        }

        public static string GetUniqueRandomString(uint number, int length, IList<char> characterSet)
        {
            StringBuilder stringBuilder = new StringBuilder();

            for (int i = 0; i < length; ++i)
            {
                stringBuilder.Append(characterSet[(int)number & ((1 << (length-1)) - 1)]);
                number = number >> (length - 1);
            }

            return stringBuilder.ToString();
        }
    }
}
