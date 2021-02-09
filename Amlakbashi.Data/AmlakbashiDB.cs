using Amlakbashi.Core.Common.Entity;
using Amlakbashi.Core.Common.Extensions;
using Amlakbashi.Core.Common.Repository;
using Amlakbashi.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Linq;

namespace Amlakbashi.Data
{
    public class AmlakbashiDB : DbContext, IDbContext
    {
        public DbSet<BlogPost> BlogPosts { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<ServicePost> ServicePostItems { get; set; }
        public DbSet<BankCard> BankCarts { get; set; }
        public DbSet<SupportChat> SupportChats { get; set; }
        public DbSet<SupportChatMessage> SupportChatMessages { get; set; }
        public DbSet<ReportItem> ReportItems { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<ReserveSupport> ReserveSupports { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserFavorite> UserFavorites { get; set; }
        public DbSet<Region> Regions { get; set; }
        public DbSet<DiscountTable> DiscountTables { get; set; }
        public DbSet<PriceTable> PriceTables { get; set; }
        public DbSet<OccupiedTable> OccupiedTables { get; set; }
        public DbSet<AdvertiseReport> AdvertiseReports { get; set; }
        public DbSet<Advertise> Advertises { get; set; }
        public DbSet<File> Files { get; set; }
        public DbSet<ActionLog> actionLogs { get; set; }
        public DbSet<DynamicCategory> DynamicCategories { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<Reserve> Reserves { get; set; }
        public DbSet<ExtrinsicReserve> ExtrinsicReserves { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<ReservePayment> ReservePayments { get; set; }
        public DbSet<DiscountCoupon> DiscountCoupons { get; set; }
        public DbSet<PrizeCreditTransaction> PrizeCreditTransactions{ get; set; }
        public DbSet<CreditTransaction> CreditTransactions{ get; set; }
        public DbSet<Cart> Carts{ get; set; }
        public DbSet<Payment> Payments{ get; set; }
        public DbSet<GroupPayment> GroupPayments{ get; set; }
        public DbSet<ReserveAutoCancel> ReserveAutoCancels{ get; set; }
        public DbSet<InstantReserveAutoCancel> InstantReserveAutoCancels{ get; set; }
        public DbSet<ReserveSendSms> ReserveSendSms{ get; set; }

        private readonly IConfiguration configuration;

        public AmlakbashiDB(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("AmlakbashiDB"));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                //other automated configurations left out
                if (typeof(ISoftDelete).IsAssignableFrom(entityType.ClrType))
                {
                    entityType.AddSoftDeleteQueryFilter();
                }
            }
        }
    }
}
