using System;
using System.Collections.Generic;
using System.Text;

namespace UltraPro.Common.Helpers
{
    public static class Helper
    {
        public static bool IsOneOf<T>(this T value, params T[] items)
        {
            for (int i = 0; i < items.Length; ++i)
            {
                if (items[i].Equals(value))
                {
                    return true;
                }
            }

            return false;
        }

        public static string ReplaceNullable(this string stringToSearch, string find, string replaceWith)
        {
            if (stringToSearch == null) return null;
            if (string.IsNullOrEmpty(find) || replaceWith == null) return stringToSearch;

            return stringToSearch.Replace(find, replaceWith);
        }

        public static double PerformanceParcentage(this double score, double scoreScale)
        {
            return Math.Round(((score / scoreScale) * 100), 2);
        }

        public static int[] GetMonthsByFrequency(this DateTime date, int frequency)
        {
            List<int> monthIds = new List<int>();

            var strtDate = date.AddMonths(-1);
            for (int i = 0; i < 12 / frequency; i++)
            {
                strtDate = strtDate.AddMonths(frequency);
                monthIds.Add(strtDate.Month);
            }

            return monthIds.ToArray();
        }

        //number to currency bdt===================================================================
        public static string NumberToCurrencyTextBdt(double number)
        {
            // Round the value just in case the decimal value is longer than two digits
            number = Math.Round(number, 2);

            string wordNumber = string.Empty;

            // Divide the number into the whole and fractional part strings
            string[] arrNumber = number.ToString().Split('.');

            // Get the whole number text
            long wholePart = long.Parse(arrNumber[0]);
            string strWholePart = NumberToWords(wholePart);


            wordNumber = (wholePart == 0 ? "No" : strWholePart) + (wholePart == 1 ? " Taka" : " Taka");


            if (arrNumber.Length > 1)
            {
                wordNumber += " and ";
                // If the length of the fractional element is only 1, add a 0 so that the text returned isn't,
                // 'One', 'Two', etc but 'Ten', 'Twenty', etc.
                long fractionPart = long.Parse((arrNumber[1].Length == 1 ? arrNumber[1] + "0" : arrNumber[1]));
                string strFarctionPart = NumberToWords(fractionPart);

                wordNumber += (fractionPart == 0 ? " No" : strFarctionPart) + (fractionPart == 1 ? " Paisa" : " Paisa");
            }
            else
                wordNumber += "";

            return wordNumber /*+ " Only"*/;
        }
        // Bangladeshi Taka Number to word===================================================================

        //this array specifies the names of the powers of 10
        static Tuple<int, string>[] powers =
{
    new Tuple<int, string>(0, ""),
    new Tuple<int, string>(3, "Thousand"),
    new Tuple<int, string>(5, "Lac"),
    new Tuple<int, string>(7, "Crore")
};

        //this array specifies the digits' names
        static string[] digits = { "", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine" };
        static string[] extendedDigits = { "Ten", "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen" };
        static string[] tensWords = { "", "", "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety" };

        private static string NumberToWords(long number)
        {
            var sb = new StringBuilder();

            //begin with the left most digit (greatest power of 10)
            for (int i = powers.Length - 1; i >= 0; --i)
            {
                //translate the current part only (for a known power of 10)
                //usually part is a 3-digit number
                int part = (int)(number / (long)Math.Pow(10, powers[i].Item1));
                //if the part is 0, we don't have to add anything
                if (part > 0)
                {
                    //extract the hundreds
                    int hundreds = part / 100;
                    if (hundreds > 9)
                        throw new ArgumentException(number + " is too large and cannot be expressed.");
                    if (hundreds > 0)
                    {
                        //if there are hundreds, copy them to the output
                        sb.Append(digits[hundreds]);
                        sb.Append(" Hundred ");
                    }
                    //convert the next two digits
                    sb.Append(TwoDigitNumberToWord(part % 100));
                    sb.Append(" ");
                    //and append the name of the power of 10
                    sb.Append(powers[i].Item2);
                    sb.Append(" ");
                    //subtract the currently managed part
                    number -= part * (long)Math.Pow(10, powers[i].Item1);
                }
            }
            return sb.ToString();
        }

        private static string TwoDigitNumberToWord(int number)
        {
            //one digit case
            if (number < 10)
                return digits[number];
            //special case 10 <= n <= 19
            if (number < 20)
                return extendedDigits[number - 10];
            int tens = number / 10;
            int ones = number % 10;
            //concatenate the word from the two digits' words
            return tensWords[tens] + " " + digits[ones];
        }

        #region usd number to word
        public static string NumberToCurrencyTextUsd(double number)
        {
            //find the currency
            // with fixed 2 digit value
            number = Convert.ToDouble(number.ToString("F2"));

            // Round the value just in case the decimal value is longer than two digits
            number = Math.Round(number, 2);

            string wordNumber = string.Empty;

            // Divide the number into the whole and fractional part strings
            string[] arrNumber = number.ToString().Split('.');

            // Get the whole number text
            long wholePart = long.Parse(arrNumber[0]);
            string strWholePart = NumberToTextUsd(wholePart);

            // For amounts of zero dollars show 'No Dollars...' instead of 'Zero Dollars...'
            wordNumber = (wholePart == 0 ? "No" : strWholePart) + (wholePart == 1 ? " Dollar" : " Dollars");

            // If the array has more than one element then there is a fractional part otherwise there isn't
            // just add 'No Cents' to the end
            if (arrNumber.Length > 1)
            {
                wordNumber += " and ";
                // If the length of the fractional element is only 1, add a 0 so that the text returned isn't,
                // 'One', 'Two', etc but 'Ten', 'Twenty', etc.
                long fractionPart = long.Parse((arrNumber[1].Length == 1 ? arrNumber[1] + "0" : arrNumber[1]));
                string strFarctionPart = NumberToTextUsd(fractionPart);

                wordNumber += (fractionPart == 0 ? " No" : strFarctionPart) + (fractionPart == 1 ? " Cent" : " Cents");
            }
            else
                wordNumber += "";

            return wordNumber + " Only";
        }
        public static string NumberToTextUsd(long number)
        {
            StringBuilder wordNumber = new StringBuilder();

            string[] powers = new string[] { "Thousand ", "Million ", "Billion " };
            string[] tens = new string[] { "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety" };
            string[] ones = new string[]
            {
               "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten",
               "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen"
            };

            if (number == 0)
            {
                return "Zero";
            }
            if (number < 0)
            {
                wordNumber.Append("Negative ");
                number = -number;
            }

            long[] groupedNumber = new long[] { 0, 0, 0, 0 };
            int groupIndex = 0;

            while (number > 0)
            {
                groupedNumber[groupIndex++] = number % 1000;
                number /= 1000;
            }

            for (int i = 3; i >= 0; i--)
            {
                long group = groupedNumber[i];

                if (group >= 100)
                {
                    wordNumber.Append(ones[group / 100 - 1] + " Hundred ");
                    group %= 100;

                    if (group == 0 && i > 0)
                        wordNumber.Append(powers[i - 1]);
                }

                if (group >= 20)
                {
                    if ((group % 10) != 0)
                        wordNumber.Append(tens[group / 10 - 2] + " " + ones[group % 10 - 1] + " ");
                    else
                        wordNumber.Append(tens[group / 10 - 2] + " ");
                }
                else if (group > 0)
                    wordNumber.Append(ones[group - 1] + " ");

                if (group != 0 && i > 0)
                    wordNumber.Append(powers[i - 1]);
            }

            return wordNumber.ToString().Trim();
        }
        #endregion
    }
}
