using Amlakbashi.Core.Common.Utilities;
using Amlakbashi.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Amlakbashi.Core.Entities.Chat;

namespace Amlakbashi.Core.Infrastructure.LocalizationHelpers
{
    public class ChatLocalization
    {
        public static string GetStatusString(int status)
        {
            switch ((ChatStatusEnum)status)
            {
                case ChatStatusEnum.Sent:
                    return "ارسال شده";
                case ChatStatusEnum.HasForbiddenCharacters:
                    return "دارای کلمات ممنوعه";
                case ChatStatusEnum.Deleted:
                    return "پاک شده";
                default:
                    return "";
            }
        }

        public static bool TextHasForbiddenCharacters(string text, out string outputText,
            IQueryable<Advertise> allAdvertises)
        {
            var words = text.Split(' ');
            bool has_forbidden_characters = false;
            for (int i = 0; i < words.Length; i++)
            {
                var word_has_forbidden_characters = false;
                var word = words[i];
                long advertise_id;
                if (long.TryParse(word, out advertise_id))
                {
                    var advertise = allAdvertises.FirstOrDefault(x => x.Id == advertise_id);
                    if (advertise != null)
                    {
                        continue;
                    }
                }
                if (BankUtility.ValidateBankCardNumber(word))
                {
                    word_has_forbidden_characters = true;
                }
                else if (word.Contains("6037") ||
                    word.Contains("5892") ||
                    word.Contains("6276") ||
                    word.Contains("6279") ||
                    word.Contains("6280") ||
                    word.Contains("6277") ||
                    word.Contains("5029") ||
                    word.Contains("6274") ||
                    word.Contains("6221") ||
                    word.Contains("5022") ||
                    word.Contains("6219") ||
                    word.Contains("6393") ||
                    word.Contains("6396") ||
                    word.Contains("6362") ||
                    word.Contains("5028") ||
                    word.Contains("6104") ||
                    word.Contains("6273") ||
                    word.Contains("5894"))
                {
                    word_has_forbidden_characters = true;
                }
                else if (word.Contains("پلاک") || word.Contains("بلاك") ||
                    word.Contains("کوچه") || word.Contains("كوجه"))
                {
                    word_has_forbidden_characters = true;
                }
                else if (word.StartsWith("0") || word.StartsWith("۰") || word.StartsWith("٠"))
                {
                    word_has_forbidden_characters = true;
                }
                else if (word.StartsWith("9") || word.StartsWith("۹") || word.StartsWith("٩"))
                {
                    word_has_forbidden_characters = true;
                }
                else if (word.Contains("09") || word.Contains("۰۹") || word.Contains("٠٩"))
                {
                    word_has_forbidden_characters = true;
                }
                else if (word.Contains("صفر") || word.Contains("صفر") ||
                     word.Contains(" ص ") || word.Contains(" ف ") ||
                     word.Contains(" ر ") || word.Contains(" ن ") ||
                      word.Contains(" ه ") || word.Contains(" د ") ||
                     word.Contains("صفرنه") || word.Contains("صفرنه") ||
                     word.Contains("نهصد") || word.Contains("نهصد") ||
                     word.Contains("نهصدو") || word.Contains("نهصدو"))
                {
                    word_has_forbidden_characters = true;
                }
                else if (word.Contains("دیوار") || word.Contains("ديوار") ||
                   word.Contains("شیپور") || word.Contains("شيبور") ||
                   word.Contains("جاکو") || word.Contains("جاكو") ||
                   word.Contains("جا کو") || word.Contains("جا كو"))
                {
                    word_has_forbidden_characters = true;
                }
                else if (Regex.IsMatch(word, @"[a-zA-Z]"))
                {
                    word_has_forbidden_characters = true;
                }
                if (word_has_forbidden_characters)
                {
                    //var sb = new StringBuilder(words[i].Length);
                    //for (int j = 0; j < words[i].Length; j++)
                    //{
                    //    sb.Append('*');
                    //}
                    //words[i] = sb.ToString();
                    has_forbidden_characters = true;
                }
            }

            if (has_forbidden_characters)
            {
                var sb = new StringBuilder(5);
                for (int j = 0; j < 5; j++)
                {
                    sb.Append('*');
                }
                outputText = sb.ToString();
            }
            else
            {
                outputText = text;
            }

            return has_forbidden_characters;
        }

        private static List<string> ForbiddenWords = new List<string>()
        {
            "6037",
            "5892",
            "6276",
            "6279",
            "6280",
            "6277",
            "5029",
            "6274",
            "6221",
            "5022",
            "6219",
            "6393",
            "6396",
            "6362",
            "5028",
            "6104",
            "6273",
            "5894",
            "پلاک",
            "بلاك",
            "کوچه",
            "كوجه",
            "09",
            "۰۹",
            "صفر",
            "ص",
            "ف",
            "ر",
            "ن",
            "ه",
            "د",
            "صفرنه",
            "نهصد",
            "نهصدو",
            "دیوار",
            "شیپور",
            "شيبور",
            "جاکو",
            "",
            "",
            "",
        };
        private static List<string> ForbiddenStartedCharacters = new List<string>()
        {
            "0",
            "۰",
            "9",
            "۹",
        };
        public static bool HasForbiddenWord(string text)
        {
            var words = text.Split(' ');
            foreach (var word in words)
            {
                if (BankUtility.ValidateBankCardNumber(word) ||
                    ForbiddenWords.Any(x => word.Contains(x)) ||
                    ForbiddenStartedCharacters.Any(x => word.StartsWith(x)) ||
                    Regex.IsMatch(word, @"[a-zA-Z]"))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
