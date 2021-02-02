using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Amlakbashi.Core.Common.Utilities
{
    public static class DeepClone
    {
        public static T Clone<T>(this T obj)
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, obj);
                ms.Position = 0;

                return (T)formatter.Deserialize(ms);
            }
        }
    }
}
