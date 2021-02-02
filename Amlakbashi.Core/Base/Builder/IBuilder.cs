namespace Amlakbashi.Core.Base.Builder
{
    public interface IBuilder<T> where T : IPart
    {
        void BuildPart(T part);
        Product<T> GetProduct();
    }
}
