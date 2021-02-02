using Amlakbashi.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Core.Common.Repository
{
    public interface IAccountingRepository
    {
        User FindUser(int userId);
        Reserve FindReserve(long reserveId);
        IQueryable<Reserve> GetReservesThatHaveReservePayment();
        BankCard FindBankCard(int bankCardId);
        BankCard FindBankCardByUserId(int userId);
        IQueryable<Reserve> GetAllReserves();
    }
}
