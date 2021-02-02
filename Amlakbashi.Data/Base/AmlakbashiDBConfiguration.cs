namespace Amlakbashi.Data.Base
{
    public class AmlakbashiDBConfiguration : DbConfiguration
    {
        public AmlakbashiDBConfiguration()
        {
            AddInterceptor(new SoftDeleteInterceptor());
        }
    }
}
