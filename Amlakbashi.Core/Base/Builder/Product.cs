using System.Collections.Generic;

namespace Amlakbashi.Core.Base.Builder
{
    public class Product<T> : IProduct<T> where T : IPart
    {
        protected readonly List<T> parts = new List<T>();

        public void Add(T part)
        {
            parts.Add(part);
        }

        public List<T> GetAllParts()
        {
            return parts;
        }
    }
}
