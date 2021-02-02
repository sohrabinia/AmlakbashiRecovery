using System.Collections.Generic;

namespace Amlakbashi.Core.Base.Builder
{
    internal interface IProduct<T> where T : IPart
    {
        void Add(T part);
        List<T> GetAllParts();
    }
}
