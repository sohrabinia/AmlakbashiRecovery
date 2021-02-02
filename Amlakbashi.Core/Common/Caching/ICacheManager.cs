using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Core.Common.Caching
{
    public interface ICacheManager<T>
    {
        T Set(T entity);
        T Get(object id);

        bool Remove(object id);
    }
}
