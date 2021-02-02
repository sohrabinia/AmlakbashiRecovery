using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Core.Common.Utilities
{
    public static class BankUtility
    {
        public static bool ValidateBankCardNumber(string card_number)
        {
            if (card_number.Length != 16)
            {
                return false;
            }
            var check_number_array = new int[16];
            for (int i = 0; i < 16; i++)
            {
                var str = card_number.Substring(i, 1);
                int n;
                if (int.TryParse(str, out n))
                {
                    var multiplier = (i + 1) % 2 == 0 ? 1 : 2;
                    var check_number = n * multiplier;
                    if (check_number > 9)
                        check_number -= 9;
                    check_number_array[i] = check_number;
                }
                else
                {
                    return false;
                }
            }
            var sum = check_number_array.Sum();
            var is_valid = sum % 10 == 0;
            return is_valid;
        }
    }
}
