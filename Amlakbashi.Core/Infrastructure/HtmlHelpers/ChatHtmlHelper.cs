using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Amlakbashi.Core.Infrastructure.HtmlHelpers
{
    public static class ChatHtmlHelper
    {
        public static List<string> GenerateMultilineChat(string text)
        {
            string generatedText;
            String[] separator = { "\n" };
            var sentences = text != null ? text.Split(separator, StringSplitOptions.None) : new string[0];
            var generatedSentences = new List<string>();
            foreach (var sentence in sentences)
            {
                var words = sentence.Split(' ');
                generatedText = "";
                for (int i = 0; i < words.Length; i++)
                {
                    var word = words[i];
                    var isUrl = Regex.IsMatch(word, @"^http(s)?://.*"); ;
                    if (isUrl)
                    {
                        generatedText += "<a target='_blank' style='color:#242424;text-shadow: 1px 1px 0.5px white;' href='" + word + "'>" + word + "</a>";
                    }
                    else
                    {
                        generatedText += ((i == 0 ? word + " " : " " + word + " "));
                    }
                }
                generatedSentences.Add(generatedText);
            }
            return generatedSentences;
        }
    }
}
