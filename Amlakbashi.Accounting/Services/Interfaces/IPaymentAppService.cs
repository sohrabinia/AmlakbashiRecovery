using Amlakbashi.Core.Common.AppService;
using Amlakbashi.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Amlakbashi.Accounting.Services.Interfaces
{
    internal interface IPaymentAppService : IAppService<Payment, int>
    {
        IList<Payment> Filter(long referenceNumber, int status, int userId, long reserveId, DateTime fromDate, DateTime toDate);
        IQueryable<Payment> Filter(int status, DateTime fromDate, DateTime toDate);
        IList<Payment> GetRange(DateTime fromDate, DateTime toDate, int status, IList<int> userIds = null,
            bool byTotalPrice = false);
        int GetPaymentTriesCount(long reserveId, out string lastTryDateStr);
        Payment Find(int id);
        int Insert(Payment newPayment);
        void Update(Payment editedPayment);
        IQueryable<Payment> GetAllAsIQueryable();
    }
}
