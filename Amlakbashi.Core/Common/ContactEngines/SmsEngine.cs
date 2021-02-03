using Kavenegar.Core.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Core.Common.ContactEngines
{
    public static class SmsEngine
    {
        const string api = "3434654F7A4C2F32574846646959666A52524D5A74737233336E5A6D55515975";
        const string line = "10008412";

        public static void SendMessage(string to, string message)
        {
            try
            {
                var sg = new Kavenegar.KavenegarApi(api);
                sg.Send(line, to, message);
            }
            catch
            {

            }
        }

        public static void SendVerification(string to, string code)
        {
            try
            {
                var sg = new Kavenegar.KavenegarApi(api);
                sg.VerifyLookup(to, code, "verification");
            }
            catch
            {

            }
        }

        public static void SendSms(string to, string template)
        {
            try
            {
                var sg = new Kavenegar.KavenegarApi(api);
                sg.VerifyLookup(to, "", template);
            }
            catch
            {

            }
        }

        public static void SendMessage(string[] to, string message)
        {
            try
            {
                var sg = new Kavenegar.KavenegarApi(api);
                var sender = to.Select(q => line);
                var text = to.Select(q => message);
                sg.SendArray(sender.ToList(), to.ToList(), text.ToList());
            }
            catch
            {

            }
        }

        public static void VerifyLookup(string mobile, string name, VerifyLookupType type = VerifyLookupType.Sms)
        {
            VerifyLookup(mobile, ".", name, type);
        }

        public static void VerifyLookup(string mobile, string token, string name, VerifyLookupType type = VerifyLookupType.Sms)
        {
            try
            {
                var sg = new Kavenegar.KavenegarApi(api);
                sg.VerifyLookup(mobile, token, name, type);
            }
            catch
            {
                //LogHelper.LogError(ex);
            }
        }

        public static void VerifyLookup(string mobile, string token, string token2, string name)
        {
            VerifyLookup(mobile, token, token2, "", name);
        }

        public static void VerifyLookup(string mobile, string token, string token2, string token3, string name)
        {
            try
            {
                var sg = new Kavenegar.KavenegarApi(api);
                sg.VerifyLookup(mobile, token, token2, token3, name);
            }
            catch
            {

            }
        }

        public static void VerifyLookup(string mobile, string token, string token2, string token3, string token4, string name)
        {
            VerifyLookup(mobile, token, token2, token3, token4, "", name);
        }

        public static void VerifyLookup(string mobile, string token, string token2, string token3, string token4, string token5, string name)
        {
            try
            {
                var sg = new Kavenegar.KavenegarApi(api);
                sg.VerifyLookup(mobile, token, token2, token3, token4, token5, name, VerifyLookupType.Sms);
            }
            catch
            {

            }
        }
    }
}
