using System;
using Newtonsoft.Json;

namespace Amlakbashi.Core.Common.Extensions
{
    public static class JsonExtensions
    {
        public static string ToJsonString(this object obj, bool camelCase = false, bool indented = false)
        {
            var settings = new JsonSerializerSettings();
            if (indented)
            {
                settings.Formatting = Formatting.Indented;
            }
            return ToJsonString(obj, settings);
        }

        public static string ToJsonString(this object obj, JsonSerializerSettings settings)
        {
            return obj != null
                ? JsonConvert.SerializeObject(obj, settings)
                : default(string);
        }

        public static T FromJsonString<T>(this string value)
        {
            return value.FromJsonString<T>(new JsonSerializerSettings());
        }

        public static T FromJsonString<T>(this string value, JsonSerializerSettings settings)
        {
            return value != null
                ? JsonConvert.DeserializeObject<T>(value, settings)
                : default(T);
        }

        public static object FromJsonString(this string value, Type type, JsonSerializerSettings settings)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            return value != null
                ? JsonConvert.DeserializeObject(value, type, settings)
                : null;
        }
    }
}