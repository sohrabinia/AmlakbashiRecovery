using Newtonsoft.Json;

namespace Amlakbashi.Core.Common.Utilities
{
    public static class SerializeUtility
    {
        public static string SerializeToJS(object o)
        {
            return JsonConvert.SerializeObject(o);
        }
    }
}
