using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Core.Common.Caching
{
    public interface ICacheManager
    {
        T Set<T>(string key, T entity);
        T Get<T>(string key);
        void Remove(string key);
    }
}
