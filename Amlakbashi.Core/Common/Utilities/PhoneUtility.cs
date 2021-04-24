using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Amlakbashi.Core.Common.Utilities
{
    public static class PhoneUtility
    {
        public static string CorrectPhoneNumberIfPossible(string number)
        {
            if (string.IsNullOrEmpty(number))
                return null;
            string[] persian = new string[10] { "۰", "۱", "۲", "۳", "۴", "۵", "۶", "۷", "۸", "۹" };
            string[] arabic = new string[10] { "٠", "١", "٢", "٣", "٤", "٥", "٦", "٧", "٨", "٩" };
            number = number.Trim();
            for (int i = 0; i < persian.Length; i++)
            {
                number = number.Replace(persian[i], i.ToString());
                number = number.Replace(arabic[i], i.ToString());
            }
            if (number.Substring(0, 2) == "00" && number.Length > 11)
            {
                number = number.Remove(0, 2);
                number = number.Insert(0, "+");
            }
            if (PhoneUtility.ValidateLocalNumber(number))
            {
                number = PhoneUtility.LocalNumberToInternational(number, 98);
            }
            if (number.Substring(3, 1) != " " && number.Length > 11)
            {
                number = number.Insert(3, " ");
            }
            if (number.Substring(0, 1) != "+")
            {
                number = "+" + number;
            }
            if (number.Contains("  "))
            {
                number = number.Replace("  ", " ");
            }
            if (number.Contains(" ") == false)
            {
                number = number.Substring(0, 3) + " " + number.Substring(3);
            }
            return number;
        }

        public static bool ValidateInternationalNumber(string international_number)
        {
            if (string.IsNullOrEmpty(international_number))
                return false;
            Regex r = new Regex(@"\+\d+\s[0-9]{10}",
                RegexOptions.IgnoreCase | RegexOptions.Singleline);
            return r.IsMatch(international_number);
        }
        public static bool ValidateLocalNumber(string local_number)
        {
            if (string.IsNullOrEmpty(local_number) || local_number.Length != 11)
                return false;
            Regex r = new Regex("0[0-9]{10}",
                RegexOptions.IgnoreCase | RegexOptions.Singleline);
            return r.IsMatch(local_number);
        }
        public static bool ValidateIranMobileNumber(string mobile_number)
        {
            if (string.IsNullOrEmpty(mobile_number))
                return false;
            Regex r = new Regex("09[0-9]{9}",
                RegexOptions.IgnoreCase | RegexOptions.Singleline);
            return r.IsMatch(mobile_number);
        }
        public static bool ValidateCallableNumber(string callable_number)
        {
            return !string.IsNullOrEmpty(callable_number) && ((ValidateLocalNumber(callable_number) && callable_number.Length == 11) ||
                (callable_number.Substring(0, 2) == "00" && callable_number.Length > 11));
        }
        public static string InternationalNumberToLocal(string international_number)
        {
            if (string.IsNullOrEmpty(international_number))
                return null;
            return "0" + international_number.Split(' ')[1];
        }
        public static string LocalNumberToInternational(string local_number, int country_code)
        {
            if (string.IsNullOrEmpty(local_number))
                return null;
            return "+" + country_code + " " + local_number.Remove(0, 1);
        }
        public static bool IsNumberForIran(string number)
        {
            return !string.IsNullOrEmpty(number) && number.Substring(0, 3) == "+98";
        }
        public static string InternationalNumberToCallable(string number)
        {
            if (string.IsNullOrEmpty(number))
                return null;
            return number.Replace(" ", "").Replace("+", "00");
        }

        public static string NormalizePhoneNumber(string phone_number)
        {
            return string.IsNullOrEmpty(phone_number) ? null :
                (IsNumberForIran(phone_number) ?
                InternationalNumberToLocal(phone_number) :
                InternationalNumberToCallable(phone_number));
        }
    }
}
