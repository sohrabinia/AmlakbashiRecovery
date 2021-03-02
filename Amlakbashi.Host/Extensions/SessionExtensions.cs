using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;

namespace Amlakbashi.Host.Extensions
{
    public static class SessionExtensions
    {
        public static void SetObjectAsJson(this ISession session, string key, object value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static T GetObjectFromJson<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }

        public static void SetBool(this ISession session, string key, bool value)
        {
            session.SetInt32(key, value ? 1 : 0);
        }

        public static bool GetBool(this ISession session, string key)
        {
            var value = session.GetInt32(key);
            return value != null && value == 1; 
        }

        public static void SetObjectAsJson(this ITempDataDictionary tempData, string key, object value)
        {
            tempData[key] = JsonConvert.SerializeObject(value);
        }

        public static T GetObjectFromJson<T>(this ITempDataDictionary tempData, string key)
        {
            var value = tempData[key];
            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value as string);
        }
    }
}
