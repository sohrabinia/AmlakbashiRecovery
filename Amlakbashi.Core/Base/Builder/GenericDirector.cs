namespace Amlakbashi.Core.Base.Builder
{
    public abstract class GenericDirector<T, B> where B : IBuilder<IPart>
    {
        protected readonly T data;
        protected readonly B builder;
        public GenericDirector(T data, B builder)
        {
            this.data = data;
            this.builder = builder;
        }
        public Product<IPart> GetProduct()
        {
            return builder.GetProduct();
        }
    }
}
