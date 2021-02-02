using System.Text;
using System.Security.Cryptography;

namespace Amlakbashi.Core.Common.Utilities
{
    public static class HashUtility
    {
        public static string GetMd5Hash(string input)
        {
            var md5Hash = MD5.Create();
            byte[] data = md5Hash.ComputeHash(
                Encoding.UTF8.GetBytes(input));
            var sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }
    }
}
