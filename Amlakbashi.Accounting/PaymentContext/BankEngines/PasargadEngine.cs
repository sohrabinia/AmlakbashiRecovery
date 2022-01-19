using Amlakbashi.Accounting.PaymentContext.BankEngines.Interfaces;
using Amlakbashi.Core.Common.Enums;
using Amlakbashi.Core.DTOs.PaymentDTOs;
using Amlakbashi.Core.DTOs.PaymentDTOs.BankEPayDTOs;
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

namespace Amlakbashi.Accounting.PaymentContext.BankEngines
{
    internal class PasargadEngine : IPasargadEngine
    {
        const string merchantCode = "4398762";
        const string terminalCode = "1573467";
        const string key = "<RSAKeyValue><Modulus>jgQHQPmnm8fvGZf3MNGQ+BhTzLG5cZaBFU75Ew1BqfKBVme+K6ZxByqfH0UIkzAANoHeo4R5C7r5E2jb3ZgRWA64rKQhNRhn8OPhFd5s3avYvP4musD1TH2oVBE1tX1gVpFTRGjbJgjeHz1biBz0IQ/23ff8tI0ndRV50UaSGjM=</Modulus><Exponent>AQAB</Exponent><P>xoPujff/OxZzfK5gCRANggQDXApBCFvACOk+Eo+mxKkpo9jBDAJSYeYJhoSAjtUgPuFDF70xtHaXZde+EQ3gqw==</P><Q>tyO+dAF1tJPcCKmM+ovg6ePJfhGswHz00YyRkF+TE3r/ws3fpUo5VD+U6C3YXbGTgGZvSgQVmA6H3bMS3Vx8mQ==</Q><DP>JfHr9GkV+UZmVsvCAZl264Y22i3/lkhrYYir28JnnymykuYIqHH9K0dcRMEpDaRBYKOQPoZkbNlKQSZG512etw==</DP><DQ>hFYq4G7RnEwf+o5yVfXP75LvXc7t0yY4TlfSM84sXC5MNHtJuYn6BTvwoRnHuGSCHo1mq8hpxjfxy60D27tiOQ==</DQ><InverseQ>m5SRVGzsvzMRigpL3OZMATs5j50yKE8X855YjwESuN8HApNQk0Q1lp3GRbiY5N6O4K+vuDsTFsypWDPpkUVWJQ==</InverseQ><D>jYzCVicAwqrzTMVFYule34oP7JSwS+FBZCXE6RJrgqLt+1uIFyXcvtHirF44f8x2Sd4ENWOS6vg/zvLTQvmRPoF1Ofc8O7Hf4PnDKG+BlRZcZl7mTQp4JURK0+i+Y/t8EWDZlo0cnAEbWXei9iAE0dJ75qvlK64in/uRhvxXD2E=</D></RSAKeyValue>";

        public CheckPaymentDTO GetPaymentResult(string tref, out string result)
        {
            HttpWebRequest request =
            (HttpWebRequest)WebRequest.Create("https://pep.shaparak.ir/CheckTransactionResult.aspx");
            string text = "invoiceUID=" + tref;
            byte[] textArray = Encoding.UTF8.GetBytes(text);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = textArray.Length;
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(RemoteCertificateValidation);

            request.GetRequestStream().Write(textArray, 0, textArray.Length);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream());
            result = reader.ReadToEnd();
            XDocument doc = XDocument.Parse(result); //or XDocument.Load(path)
            string jsonText = JsonConvert.SerializeXNode(doc);
            dynamic resp = ((dynamic)JsonConvert.DeserializeObject<ExpandoObject>(jsonText)).resultObj;

            CheckPaymentDTO dto = new CheckPaymentDTO()
            {
                Result = Convert.ToBoolean(resp.result),
                PaymentId = resp.invoiceNumber,
                CreatePaymentDate = resp.invoiceDate,
                TransactionReferenceId = resp.transactionReferenceID,
                Amount = resp.amount
            };
            if (dto.Result)
            {
                dto.TransactionDate = resp.transactionDate;
                dto.ReferenceNumber = resp.referenceNumber;
                dto.TraceNumber = resp.traceNumber;
            }
            return dto;
        }

        public CheckPaymentDTO GetPaymentResult(long paymentId, DateTime paymentDate)
        {
            HttpWebRequest request =
                (HttpWebRequest)WebRequest.Create("https://pep.shaparak.ir/CheckTransactionResult.aspx");
            string text = "invoiceNumber=" + paymentId + "&invoiceDate=" + paymentDate +
                "&merchantCode=" + merchantCode + "&terminalCode=" + terminalCode;
            byte[] textArray = Encoding.UTF8.GetBytes(text);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = textArray.Length;
            ServicePointManager.ServerCertificateValidationCallback = new
                RemoteCertificateValidationCallback(RemoteCertificateValidation);

            request.GetRequestStream().Write(textArray, 0, textArray.Length);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream());
            var result = reader.ReadToEnd();
            XDocument doc = XDocument.Parse(result); //or XDocument.Load(path)
            string jsonText = JsonConvert.SerializeXNode(doc);
            dynamic resp = ((dynamic)JsonConvert.DeserializeObject<ExpandoObject>(jsonText)).resultObj;

            CheckPaymentDTO dto = new CheckPaymentDTO()
            {
                Result = Convert.ToBoolean(resp.result),
                PaymentId = resp.invoiceNumber,
                CreatePaymentDate = resp.invoiceDate,
                TransactionReferenceId = resp.transactionReferenceID,
                Amount = resp.amount
            };
            if (dto.Result)
            {
                dto.TransactionDate = resp.transactionDate;
                dto.ReferenceNumber = resp.referenceNumber;
                dto.TraceNumber = resp.traceNumber;
            }
            return dto;
        }

        public bool VerifyPayment(string paymentResult, int paymentId, long totalPayingPrice)
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
            byte[] signMain = rsa.SignData(Encoding.UTF8.GetBytes(data), new SHA1CryptoServiceProvider());
            var sign = Convert.ToBase64String(signMain);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://pep.shaparak.ir/VerifyPayment.aspx");

            string text = "InvoiceNumber=" + invoiceNumber + "&InvoiceDate=" + invoiceDate 
                + "&MerchantCode=" + merchantCode + "&TerminalCode=" + terminalCode + "&Amount=" + amount 
                + "&TimeStamp=" + timeStamp + "&Sign=" + sign;
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

            return dyn.actionResult.result == "True";
        }

        public EpayDTO GeneratePaymentData(int paymentId,
            long paymentTotalAmount, string redirectAddress,  out DateTime invoiceDate)
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
            var sign = Convert.ToBase64String(signMain);

            EpayDTO epay = new EpayDTO()
            {
                Url = "https://pep.shaparak.ir/gateway.aspx",
                Bank = BankEnum.Pasargad,
                Date = invoiceDate
            };
            epay.BankData = new Dictionary<string, object>();
            epay.BankData.Add("invoiceNumber", invoiceNumber);
            epay.BankData.Add("invoiceDate", invoiceDate);
            epay.BankData.Add("amount", paymentTotalAmount);
            epay.BankData.Add("terminalCode", terminalCode);
            epay.BankData.Add("merchantCode", merchantCode);
            epay.BankData.Add("redirectAddress", redirectAddress);
            epay.BankData.Add("timeStamp", timeStamp);
            epay.BankData.Add("action", action);
            epay.BankData.Add("sign", sign);
            return epay;
        }

        private static bool RemoteCertificateValidation(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
                return true;
            return false;
        }
    }
}
