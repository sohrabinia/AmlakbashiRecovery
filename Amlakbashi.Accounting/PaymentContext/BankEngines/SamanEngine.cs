using Amlakbashi.Accounting.PaymentContext.BankEngines.Interfaces;
using Amlakbashi.Core.Common.Enums;
using Amlakbashi.Core.Common.Extensions;
using Amlakbashi.Core.DTOs.PaymentDTOs.BankEPayDTOs;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Amlakbashi.Accounting.PaymentContext.BankEngines
{
    internal class SamanEngine : ISamanEngine
    {
        const string terminalId = "12744768";
        const string bankTokenUrl = "https://sep.shaparak.ir/MobilePG/MobilePayment";
        const string bankPayUrl = "https://sep.shaparak.ir/OnlinePG/OnlinePG";

        public async Task<EpayDTO> GetPaymentToken(SamanRequestTokenDTO requestToken)
        {
            requestToken.TerminalId = terminalId;
            var restClient = new RestClient(bankTokenUrl);
            var request = new RestRequest()
            {
                Method = Method.Post,
                RequestFormat = DataFormat.Json
            };
            request.AddBody(requestToken);
            var response = await restClient.ExecuteAsync(request);
            var responseToken = JsonConvert.DeserializeObject<SamanResponseTokenDTO>(response.Content);

            EpayDTO epay = new EpayDTO()
            {
                Url = bankPayUrl,
                Bank = BankEnum.Saman,
                Date = DateTime.Now
            };
            if (responseToken.status != 1)
            {
                epay.HasError = true;
                epay.ErrorMessage = responseToken.errorDesc;
                return epay;
            }
            epay.BankData = new Dictionary<string, object>();
            epay.BankData.Add("token", responseToken.token);
            return epay;
        }

        public async Task<string> VerifyEpay(string RefNum)
        {
            var soapXmlString = @"<?xml version=""1.0"" encoding=""utf-8""?>
                <soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:soapenc=""http://schemas.xmlsoap.org/soap/encoding/"" xmlns:tns=""urn:Foo"" xmlns:types=""urn:Foo/encodedTypes"" xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
                  <soap:Body soap:encodingStyle=""http://schemas.xmlsoap.org/soap/encoding/"">
                    <tns:verifyTransaction>";

            soapXmlString += $"<String_1 xsi:type=\"xsd:string\">{RefNum}</String_1>";
            soapXmlString += $"<String_2 xsi:type=\"xsd:string\">{terminalId}</String_2>";
            soapXmlString += @"</tns:verifyTransaction></soap:Body></soap:Envelope>";

            HttpWebRequest request = CreateSoapRequest();
            XmlDocument soapEnvelopeXml = new XmlDocument();
            soapEnvelopeXml.LoadXml(soapXmlString);
            using (Stream stream = await request.GetRequestStreamAsync())
            {
                soapEnvelopeXml.Save(stream);
            }

            using (WebResponse response = await request.GetResponseAsync())
            {
                using (StreamReader rd = new StreamReader(response.GetResponseStream()))
                {
                    XmlDocument soapResponse = new XmlDocument();
                    soapResponse.LoadXml(rd.ReadToEnd());

                    XmlNamespaceManager xmlnsManager = new XmlNamespaceManager(soapResponse.NameTable);
                    xmlnsManager.AddNamespace("soap", "http://schemas.xmlsoap.org/soap/envelope/");
                    xmlnsManager.AddNamespace("xsi", "http://www.w3.org/2001/XMLSchema-instance");
                    xmlnsManager.AddNamespace("xsd", "http://www.w3.org/2001/XMLSchema");
                    xmlnsManager.AddNamespace("soapenc", "http://schemas.xmlsoap.org/soap/encoding/");
                    xmlnsManager.AddNamespace("tns", "urn:Foo");
                    xmlnsManager.AddNamespace("types", "urn:Foo/encodedTypes");
                    XmlNode node = soapResponse.SelectSingleNode("/soap:Envelope/soap:Body/tns:verifyTransactionResponse/result", xmlnsManager);
                    return node.InnerText;
                }
            }
        }

        private HttpWebRequest CreateSoapRequest()
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create("https://verify.sep.ir/Payments/ReferencePayment.asmx");
            webRequest.ContentType = "text/xml; charset=utf-8";
            webRequest.Accept = "text/xml";
            webRequest.Method = "POST";
            webRequest.Headers.Add("SOAPAction", "verifyTransaction");
            return webRequest;
        }
    }
}
