using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Amlakbashi.Core.Common.BankingEngines.PodiumEngine.GeneralInfos;

namespace Amlakbashi.Core.Common.BankingEngines.PodiumEngine.Base
{
    public abstract class PodiumRequest<R, TRes, TReq> where R : PodiumResult<TRes>
        where TRes : PodiumResultInfo where TReq : PodiumRequestInfo
    {
        private readonly HttpClient client = new HttpClient();
        private const string url = "https://api.pod.ir/srv/sc/nzh/doServiceCall";
        private const string token = "6e0012adb3ef44fe81fa5240b299090f";
        private const string tokenIssuer = "1";

        protected abstract TReq GetRequestData();
        protected abstract string GetProductId();

        public async Task<R> Send()
        {
            var data = GetRequestData();
            data.scProductId = GetProductId();
            var dict = RequestDataToDict(data);
            var content = new FormUrlEncodedContent(dict);
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("_token_", token);
            client.DefaultRequestHeaders.Add("_token_issuer_", tokenIssuer);
            var response = await client.PostAsync(url, content);
            var responseData = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<R>(responseData);
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
    }
}