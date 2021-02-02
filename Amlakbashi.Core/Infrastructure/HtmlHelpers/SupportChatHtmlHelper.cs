using Amlakbashi.Core.Infrastructure.LocalizationHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Amlakbashi.Core.Entities.SupportChat;

namespace Amlakbashi.Core.Infrastructure.HtmlHelpers
{
    public static class SupportChatHtmlHelper
    {
        public static string GenerateQuestionButton(long id, AutoQuestion question)
        {
            var onclick = /*id > 0 ? */string.Format("userAskQuestion({0}, {1})", id, (int)question); /*:*/
            //string.Format("sendSupportChatMessage({0}, '{1}', {2})", id, GetQuestionTitle(question), (int)question);
            var result = "<div onclick=\"" + onclick + "\"";
            result += " style=\"width:fit-content;font:12px Liransans;cursor:pointer;box-shadow:1px 2px 3px 0 rgba(0,0,0,.1);border: 1px solid #ccc;background-color: #fff; border-radius:5px;padding:5px;margin:5px;\">";
            result += (SupportChatLocalization.GetQuestionTitle(question) + "</div>");
            return result;
        }

        public static string GenerateOpenChatButton(string text)
        {
            var result = "\nOPENCHATBUTTON";
            //var onclick = "openChatInput()";
            //var result = " <div onclick=\"" + onclick + "\"";
            //result += " style=\"width:fit-content;font:12px Liransans;cursor:pointer;box-shadow:1px 2px 3px 0 rgba(0,0,0,.1);border: 1px solid #ccc;background-color: #fff; border-radius:5px;padding:5px;margin:5px;\">";
            //result += text + "</div>";
            return result;
        }

        public static string GenerateQuestionButtonList(long id, string openChatText)
        {
            var autoText = GenerateQuestionButton(id,
                AutoQuestion.
                questionHowToReserve);
            autoText += GenerateQuestionButton(id,
                AutoQuestion.
                questionHowToContactHost);
            autoText += GenerateQuestionButton(id,
                AutoQuestion.
                questionCheckInCheckout);
            autoText += GenerateQuestionButton(id,
                AutoQuestion.
                questionEvidence);
            autoText += GenerateQuestionButton(id,
                AutoQuestion.
                questionCancelReserveRules);
            autoText += GenerateQuestionButton(id,
                AutoQuestion.
                questionPaymentGuest);
            //autoText += GenerateQuestionButton(id,
            //    AutoQuestion.
            //    questionPaymentHost);
            if (!string.IsNullOrEmpty(openChatText))
            {
                autoText += GenerateOpenChatButton(openChatText);
            }
            return autoText;
        }
    }
}
