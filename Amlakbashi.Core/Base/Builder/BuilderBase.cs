namespace Amlakbashi.Core.Base.Builder
{
    public abstract class BuilderBase<T> : IBuilder<T> where T : IPart
    {
        private Product<T> product { get; set; }
        public BuilderBase(Product<T> product)
        {
            this.product = product;
        }
        public void BuildPart(T part)
        {
            product.Add(part);
        }
        public Product<T> GetProduct()
        {
            return product;
        }
    }
}
