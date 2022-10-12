using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Core.Common.Utilities
{
    public static class StringUtility
    {
        private static char[] numberChars = new char[]
        {
            '0','1','2','3','4','5','6','7','8','9',
            '۰','۱','۲','۳','۴','۵','۶','۷','۸','۹',
            '٠','١','٢','٣','٤','٥','٦','٧','٨','٩'
        };
        private static string[] persianNumbers = new string[10] { "۰", "۱", "۲", "۳", "۴", "۵", "۶", "۷", "۸", "۹" };
        private static string[] arabicNumbers = new string[10] { "٠", "١", "٢", "٣", "٤", "٥", "٦", "٧", "٨", "٩" };
        public static bool ContainsNumber(string str)
        {
            return str.Any(x => numberChars.Contains(x));
        }
        public static string PersianNumberToEnglish(string persian_number)
        {
            if (string.IsNullOrEmpty(persian_number))
            {
                return "";
            }
            persian_number = persian_number.Trim();
            for (int i = 0; i < persianNumbers.Length; i++)
            {
                persian_number = persian_number.Replace(persianNumbers[i], i.ToString());
                persian_number = persian_number.Replace(arabicNumbers[i], i.ToString());
            }
            return persian_number;
        }

        public static string EnglishNumberToPersian(string english_number)
        {
            if (string.IsNullOrEmpty(english_number))
            {
                return "";
            }
            string[] persian = new string[10] { "۰", "۱", "۲", "۳", "۴", "۵", "۶", "۷", "۸", "۹" };
            english_number = english_number.Trim();
            for (int i = 0; i < persian.Length; i++)
            {
                english_number = english_number.Replace(i.ToString(), persian[i]);
            }
            return english_number;
        }

        public static string GenerateVerifyCode(int numberOfDigit = 4)
        {
            int minValue = 1, maxValue = 10;
            for (int i = 1; i < numberOfDigit; i++)
            {
                minValue *= 10;
                maxValue *= 10;
            }
            return new Random().Next(minValue, maxValue).ToString();
        }

        public static string VerifyEmailSubject()
        {
            return "تایید ایمیل";
        }

        public static string VerifyEmailContent(string code)
        {
            return $"<div style='direction:rtl;text-align:right;font-size:16px;'><div>کد تایید شما در املاک باشی: <b>{code}</b></div></div>";
        }
    }
}
