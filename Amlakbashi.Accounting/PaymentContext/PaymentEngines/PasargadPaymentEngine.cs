using Amlakbashi.Accounting.PaymentContext.PaymentEngines.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml.Linq;

namespace Amlakbashi.Accounting.PaymentContext.PaymentEngines
{
    internal class PasargadPaymentEngine : IPasargadPaymentEngine
    {
        const string merchantCode = "4398762";
        const string terminalCode = "1573467";
        const string key = "<RSAKeyValue><Modulus>jgQHQPmnm8fvGZf3MNGQ+BhTzLG5cZaBFU75Ew1BqfKBVme+K6ZxByqfH0UIkzAANoHeo4R5C7r5E2jb3ZgRWA64rKQhNRhn8OPhFd5s3avYvP4musD1TH2oVBE1tX1gVpFTRGjbJgjeHz1biBz0IQ/23ff8tI0ndRV50UaSGjM=</Modulus><Exponent>AQAB</Exponent><P>xoPujff/OxZzfK5gCRANggQDXApBCFvACOk+Eo+mxKkpo9jBDAJSYeYJhoSAjtUgPuFDF70xtHaXZde+EQ3gqw==</P><Q>tyO+dAF1tJPcCKmM+ovg6ePJfhGswHz00YyRkF+TE3r/ws3fpUo5VD+U6C3YXbGTgGZvSgQVmA6H3bMS3Vx8mQ==</Q><DP>JfHr9GkV+UZmVsvCAZl264Y22i3/lkhrYYir28JnnymykuYIqHH9K0dcRMEpDaRBYKOQPoZkbNlKQSZG512etw==</DP><DQ>hFYq4G7RnEwf+o5yVfXP75LvXc7t0yY4TlfSM84sXC5MNHtJuYn6BTvwoRnHuGSCHo1mq8hpxjfxy60D27tiOQ==</DQ><InverseQ>m5SRVGzsvzMRigpL3OZMATs5j50yKE8X855YjwESuN8HApNQk0Q1lp3GRbiY5N6O4K+vuDsTFsypWDPpkUVWJQ==</InverseQ><D>jYzCVicAwqrzTMVFYule34oP7JSwS+FBZCXE6RJrgqLt+1uIFyXcvtHirF44f8x2Sd4ENWOS6vg/zvLTQvmRPoF1Ofc8O7Hf4PnDKG+BlRZcZl7mTQp4JURK0+i+Y/t8EWDZlo0cnAEbWXei9iAE0dJ75qvlK64in/uRhvxXD2E=</D></RSAKeyValue>";
        public bool ReadPaymentResult(string tref, out string result)
        {
            HttpWebRequest request =
            (HttpWebRequest)WebRequest.Create("https://pep.shaparak.ir/CheckTransactionResult.aspx");
            string text = "invoiceUID=" + tref;
            byte[] textArray = Encoding.UTF8.GetBytes(text);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = textArray.Length;
            ServicePointManager.ServerCertificateValidationCallback = new
            RemoteCertificateValidationCallback(RemoteCertificateValidation);

            request.GetRequestStream().Write(textArray, 0, textArray.Length);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream());
            result = reader.ReadToEnd();
            XDocument doc = XDocument.Parse(result); //or XDocument.Load(path)
            string jsonText = JsonConvert.SerializeXNode(doc);
            dynamic resp = ((dynamic)JsonConvert.DeserializeObject<ExpandoObject>(jsonText)).resultObj;

            return resp.result.ToLower() == "true";
        }

        public bool VerifyPayment(string paymentResult, int paymentId,
            long totalPayingPrice, out string referenceNumber,
            out long transactionReferenceID)
        {
            XDocument doc = XDocument.Parse(paymentResult); //or XDocument.Load(path)
            string jsonText = JsonConvert.SerializeXNode(doc);
            dynamic resp = ((dynamic)JsonConvert.DeserializeObject<ExpandoObject>(jsonText)).resultObj;
            var amount = totalPayingPrice;
            var timeStamp = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            var invoiceNumber = paymentId;
            var invoiceDate = (DateTime)DateTime.Parse(resp.invoiceDate);

            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(key);

            string data = "#" + merchantCode + "#" + terminalCode + "#" + invoiceNumber + "#" + invoiceDate + "#" + amount + "#" + timeStamp + "#";
            byte[] signMain = rsa.SignData(Encoding.UTF8.GetBytes(data), new
            SHA1CryptoServiceProvider());
            var sign = Convert.ToBase64String(signMain);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://pep.shaparak.ir/VerifyPayment.aspx");

            string text = "InvoiceNumber=" + invoiceNumber + "&InvoiceDate=" +
            invoiceDate + "&MerchantCode=" + merchantCode + "&TerminalCode=" +
            terminalCode + "&Amount=" + amount + "&TimeStamp=" + timeStamp +
            "&Sign=" + sign;
            byte[] textArray = Encoding.UTF8.GetBytes(text);

            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = textArray.Length;
            request.GetRequestStream().Write(textArray, 0, textArray.Length);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream());
            string result = reader.ReadToEnd();

            doc = XDocument.Parse(result); //or XDocument.Load(path)
            jsonText = JsonConvert.SerializeXNode(doc);
            dynamic dyn = JsonConvert.DeserializeObject<ExpandoObject>(jsonText);

            var validated = dyn.actionResult.result == "True";
            if (validated)
            {
                referenceNumber = (string)resp.referenceNumber;
                transactionReferenceID = long.Parse(resp.transactionReferenceID);
            }
            else
            {
                referenceNumber = null;
                transactionReferenceID = 0;
            }
            return validated;
        }

        public Dictionary<string,object> GeneratePaymentData(int paymentId,
            long paymentTotalAmount, string redirectAddress,
            out string sign, out DateTime invoiceDate)
        {
            var timeStamp = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            var invoiceNumber = paymentId;
            invoiceDate = DateTime.Now;

            var action = "1003";
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(key);

            string data = "#" + merchantCode + "#" + terminalCode + "#" + invoiceNumber + "#" + invoiceDate + "#" + paymentTotalAmount + "#" + redirectAddress + "#" + action + "#" + timeStamp + "#";
            byte[] signMain = rsa.SignData(Encoding.UTF8.GetBytes(data), new
            SHA1CryptoServiceProvider());
            sign = Convert.ToBase64String(signMain);
            //Session.Add("invoiceDate", invoiceDate);
            var result = new Dictionary<string, object>();

            result.Add("url", "https://pep.shaparak.ir/gateway.aspx");
            result.Add("invoiceNumber", invoiceNumber);
            result.Add("invoiceDate", invoiceDate);
            result.Add("amount", paymentTotalAmount);
            result.Add("terminalCode", terminalCode);
            result.Add("merchantCode", merchantCode);
            result.Add("redirectAddress", redirectAddress);
            result.Add("timeStamp", timeStamp);
            result.Add("action", action);
            result.Add("sign", sign);
            return result;
        }

        private static bool RemoteCertificateValidation(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
                return true;
            return false;
        }
    }
}
