using System.Linq;

namespace Amlakbashi.Core.Common.Utilities
{
    public static class HtmlUtility
    {
        public static string AddToQueryString(string query_string,
            string name, string value)
        {
            query_string = string.IsNullOrEmpty(query_string) ?
                ("?" + name + "=" + value) :
                (query_string + "&" + name + "=" + value);
            return query_string;
        }

        public static string RemoveFromQueryString(string queryString,
            string name, string value)
        {
            var str = name + "=" + value;
            if (queryString.Contains("?" + str))
            {
                var result = queryString.Replace(str, "");
                if (result.Last() == '?')
                {
                    result = result.Remove(result.Length - 1, 1);
                }
                result = result.Replace("?&", "?");
                return result;
            }
            return queryString.Replace("&" + str, "");
        }
    }
}
