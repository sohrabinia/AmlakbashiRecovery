using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Amlakbashi.Core.Common.BankingEngines.PodiumEngine.GeneralInfos;
using System.Security.Cryptography;
using System.Text;
using System;
using Amlakbashi.Core.Common.BankingEngines.PodiumEngine.RequestInfos;
using System.Xml.Linq;
using System.Linq;

namespace Amlakbashi.Core.Common.BankingEngines.PodiumEngine.Base
{
    public abstract class PodiumRequest<R, TRes, TReq> where R : PodiumResponse<TRes>
        where TRes : PodiumResultInfo where TReq : PodiumRequestInfo
    {
        private readonly HttpClient client = new HttpClient();
        private const string url = "https://api.pod.ir/srv/sc/nzh/doServiceCall";
        protected const string userName = "97374service";
        private const string token = "be49c42827a3424c9774a9467865b8ff";
        private const string tokenIssuer = "1";
        private const string key = "<RSAKeyValue><Modulus>yHABhi+peAQDDEX6bDCJJ84hiPhfuWtrpOXYKfP4BPylxgiOTIqstI7IwJ1+9d00AWiDdRFfq1GBqRcwHR3ZSYeBBI8hlQw8e8l41Qon6Mi7TkdbtONSKqmQIxztG1OpPl634JSvJuann8r38qs2CgYpAP4r2J1e42RXR3+btcLqjLBFaFoIhadddSWCspKb2opIvC8ogtZB9sIvdLQwJULD+QeMo69KhuxfZ6UYsfXs7oaBx2t1EtFY7KU45OPt/qWAVlw0jhqhipbhPf/KOoXMlIfiHSkZ0zKgxiOWYj9p9fQfgUOAWOCOEpO4xPqRPldiC/DX5SZcbuzdHm7g/w==</Modulus><Exponent>AQAB</Exponent><P>/dKMIxd8CZL/EFhPlUTyJiqm4zaNF6igPbqcdWo0YwUSwETMWbvOXMhCOmYG4wpfb32ebKmHqRVd+Pbe2Aym0uHforIHrc3MpQ/LWWIZ3rTs7KSJnCWPfCneVkuPe2FwXuKYsMBIDmZ34LGlVN8soqjh204T4NF6L193yoYWkXM=</P><Q>yig2g04Cr1FSLkOHZqndPbplZK6qQxIdxmF/sj19ypsLuqUXDtn0Z4dhsg4m6a6dVUdGWLjL7dpGsw77MPmAfGc5LpA0D0wUO9NnuzvFt/e0uofAwEcfTkNEEryaWZEAh92sUfvGvbV69/on/zndgD1WW2kAyzlHfzZ6OOQgX0U=</Q><DP>CYEWykGY3Wrprh0AbeB1CMTUFvH1+orP5T/Vl/fwGj0YZKau2DVWFtdQ04r6ewnqZOxZQExgzg4ExMvJsGFY5dCbZSYxb2qjXRLoVGJ+AX7Y2EuY84Xch5OxxZ1ys/YEFSl6+jNY/2EK4BkGzdTFgRm9y05zJBnz704t2CGJOs0=</DP><DQ>b7/zAnqpOUj26OQjgTj9OBZF3nY/YHwvQW68LEeZtdonjbGdIXSSinhoJQl1sU/5YR7ZYEonUQCMfIcXjGcgA6T8EyVb4ejodnnSqcnM7nlbypEomCG4yIaepszjdgeWB4UesONBZbVgmJwMaCYQr8c4aaeN7MlpIIV44uIYkxE=</DQ><InverseQ>zByW8AP4IGqXrEVMpjI1gzVwH0yd+XKVrkARaf8yZ0i3taqDpM8MXESvsb9/2qpa2suijFAYnpe3+H3bJBF61EWLORFt3JUfpYxKrO+LBuFz4NwtLaOdja4XD5ovKKSzi9ucad5tMkuH2Zf3PHRZsm5XSVyp96p2IMMjBXsV+W8=</InverseQ><D>XFqniSGq717B3MTdpz4R5GJpN0hiKvhhCADTwjjknb+fzoigsInxG9zyiTXIixbaYI+8ipNvxmtu1++UPsZDGUszWlts+NVDhTIG1/qu+uaKz323trhLYtbwUA2C/uVkhRV1mihIMB91Ov51kr3qz2GZQRjcMRoJ37KbOJmGsI+GE0Eu1E0i+s7Itg4lsEviG95sGqDhk4H/9iAsVfzIFnrqouxfJH9ING7QcHgWbGi5Zb1dJiOCOijanhkfPH0ij7p77UUsZUQ7DaXBYkKUHpvx1H8Me41PylJMrN9IjvDTqke+dOtc5PoObIWbW1IUNcxe1eIA3scnB2RGLXDc6Q==</D></RSAKeyValue>";

        protected abstract RequestData GetRequestData();

        public async Task<R> Send()
        {
            var requestData = GetRequestData();
            var requestDict = RequestDataToDict(requestData);
            var content = new FormUrlEncodedContent(requestDict);
            client.DefaultRequestHeaders.Add("_token_", token);
            client.DefaultRequestHeaders.Add("_token_issuer_", tokenIssuer);
            var httpResponse = await client.PostAsync(url, content);
            var stringResponse = await httpResponse.Content.ReadAsStringAsync();

            var responseData = JsonConvert.DeserializeObject<R>(stringResponse);
            if (responseData.hasError == false && string.IsNullOrEmpty(responseData.result.result) == false)
            {
                XDocument doc = XDocument.Parse(responseData.result.result);
                string jsonText = doc.Root.Descendants().Last().Value;
                responseData.BankResult = JsonConvert.DeserializeObject<TRes>(jsonText);
            }
            return responseData;
        }

        private Dictionary<string,string> RequestDataToDict<T>(T data) where T : class
        {
            var dict = new Dictionary<string, string>();
            var props = data.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
            foreach (var prop in props)
            {
                if (prop.Name == "BatchPayaItemInfos")
                {
                    var payaRequestList = prop.GetValue(data, null) as List<BatchPayaRequestItem>;
                    if (payaRequestList != null)
                    {
                        for (int i = 0; i < payaRequestList.Count; i++)
                        {
                            var payaDict = BatchPayaRequestItem.GenerateUrlEncodeDict("BatchPayaItemInfos", payaRequestList[i], i);
                            foreach (var payaItem in payaDict)
                            {
                                dict.Add(payaItem.Key, payaItem.Value);
                            }
                        }
                    }
                }
                else
                {
                    dict.Add(prop.Name, prop.GetValue(data, null).ToString());
                }
            }
            return dict;
        }

        protected string GenerateSignature(string request)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(key);
            byte[] signMain = rsa.SignData(Encoding.UTF8.GetBytes(request), new SHA1CryptoServiceProvider());
            return Convert.ToBase64String(signMain);
        }
    }
}