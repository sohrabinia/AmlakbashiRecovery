using Amlakbashi.Core.Common.Entity;
using Amlakbashi.Core.Common.Extensions;
using Amlakbashi.Core.Common.Repository;
using Amlakbashi.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace Amlakbashi.Data
{
    public class AmlakbashiDB : DbContext, IDbContext
    {
        public DbSet<BlogPost> BlogPosts { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<ServicePost> ServicePosts { get; set; }
        public DbSet<BankCard> BankCards { get; set; }
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
        public DbSet<Tag> Tags { get; set; }
        public DbSet<LeadEvent> LeadEvents { get; set; }

        private readonly IConfiguration configuration;

        public AmlakbashiDB(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.UseLazyLoadingProxies().UseSqlServer(configuration.GetConnectionString("AmlakbashiDB"));
            optionsBuilder.UseLazyLoadingProxies()
                .ConfigureWarnings(warnings => warnings.Ignore(CoreEventId.DetachedLazyLoadingWarning))
                .UseSqlServer(configuration.GetConnectionString("AmlakbashiDB"));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(ISoftDelete).IsAssignableFrom(entityType.ClrType))
                {
                    entityType.AddSoftDeleteQueryFilter();
                }
            }

            modelBuilder.Entity<Comment>(entity =>
            {
                entity.HasOne(x => x.Parent)
                    .WithMany(x => x.Childs)
                    .HasForeignKey(x => x.ParentID);
            });

            modelBuilder.Entity<DynamicCategory>()
                .HasMany(p => p.Advertises)
                .WithMany(p => p.Categories)
                .UsingEntity<Dictionary<string, object>>(
                    "DynamicCategoryAdvertises",
                    j => j
                        .HasOne<Advertise>()
                        .WithMany()
                        .HasForeignKey("Advertise_Id")
                        .HasConstraintName("FK_DynamicCategoryAdvertises_Residences_Advertise_Id")
                        .OnDelete(DeleteBehavior.Cascade),
                    j => j
                        .HasOne<DynamicCategory>()
                        .WithMany()
                        .HasForeignKey("DynamicCategory_Id")
                        .HasConstraintName("FK_dbo.DynamicCategoryAdvertises_dbo.DynamicCategories_DynamicCategory_Id")
                        .OnDelete(DeleteBehavior.Cascade));

            modelBuilder.Entity<Advertise>()
                .HasMany(p => p.Photos)
                .WithMany(p => p.Advertises)
                .UsingEntity<Dictionary<string, object>>(
                    "FileAdvertises",
                    j => j
                        .HasOne<File>()
                        .WithMany()
                        .HasForeignKey("File_Id")
                        .HasConstraintName("FK_dbo.FileAdvertises_dbo.Files_File_Id")
                        .OnDelete(DeleteBehavior.Cascade),
                    j => j
                        .HasOne<Advertise>()
                        .WithMany()
                        .HasForeignKey("Advertise_Id")
                        .HasConstraintName("FK_FileAdvertises_Residences_Advertise_Id")
                        .OnDelete(DeleteBehavior.Cascade));

            modelBuilder.Entity<Advertise>()
                .HasMany(p => p.Tags)
                .WithMany(p => p.Residences)
                .UsingEntity<Dictionary<string, object>>(
                    "ResidenceTags",
                    j => j
                        .HasOne<Tag>()
                        .WithMany()
                        .HasForeignKey("TagId")
                        .HasConstraintName("FK_ResidenceTags_Tags_TagId")
                        .OnDelete(DeleteBehavior.Cascade),
                    j => j
                        .HasOne<Advertise>()
                        .WithMany()
                        .HasForeignKey("ResidenceId")
                        .HasConstraintName("FK_ResidenceTags_Residences_ResidenceId")
                        .OnDelete(DeleteBehavior.Cascade));
        }
    }
}
