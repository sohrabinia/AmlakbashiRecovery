using Amlakbashi.Core.Common.Repository;
using Amlakbashi.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Amlakbashi.Data.Repositories
{
    public class AccountingRepository : IAccountingRepository
    {
        private AmlakbashiDB context;
        private DbSet<User> userDbSet;
        private DbSet<BankCard> bankCardDbSet;
        private DbSet<Reserve> reserveDbSet;

        public AccountingRepository(AmlakbashiDB _context)
        {
            this.context = _context;
            userDbSet = _context.Set<User>();
            bankCardDbSet = _context.Set<BankCard>();
            reserveDbSet = _context.Set<Reserve>();
        }

        public User FindUser(int userId)
        {
            return userDbSet.Find(userId);
        }

        public Reserve FindReserve(long reserveId)
        {
            return reserveDbSet.Find(reserveId);
        }

        public IQueryable<Reserve> GetReservesThatHaveReservePayment()
        {
            return reserveDbSet.Where(w => w.ReservePayments.Any());
        }

        public BankCard FindBankCard(int bankCardId)
        {
            return bankCardDbSet.Find(bankCardId);
        }

        public BankCard FindBankCardByUserId(int userId)
        {
            return bankCardDbSet.FirstOrDefault(f => f.UserID == userId);
        }

        public IQueryable<Reserve> GetAllReserves()
        {
            return reserveDbSet.AsQueryable();
        }
    }
}
