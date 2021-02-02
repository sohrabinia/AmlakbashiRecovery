using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Core.Common.Utilities
{
    public static class MathUtility
    {
        public static int GenerateFibonacci(int first, int second, int n)
        {
            if (n < 1)
                return 0;
            var serie = new List<int>() { first, second };
            for (int i = 1; i <= n; i++)
            {
                serie.Add(serie[i - 1] + serie[i]);
            }
            return serie[n - 1];
        }

        public static string IntToHex(int input)
        {
            var hash = "";
            var alphabet = "0123456789ABCDEF";
            var alphabetLength = alphabet.Length;
            do
            {
                hash = alphabet[input % alphabetLength] + hash;
                input = input / alphabetLength;
            } while (input > 0);
            return hash;
        }

        public static int HexToInt(string hex)
        {
            int intValue = int.Parse(hex, System.Globalization.NumberStyles.HexNumber);
            return intValue;
        }
    }
}
