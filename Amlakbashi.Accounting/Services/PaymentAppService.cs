using Amlakbashi.Accounting.Services.Interfaces;
using Amlakbashi.Core.Common.AppService;
using Amlakbashi.Core.Common.Enums;
using Amlakbashi.Core.Common.Repository;
using Amlakbashi.Core.Common.Utilities;
using Amlakbashi.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Accounting.Services
{
    internal class PaymentAppService : AppServiceBase<Payment, int>, IPaymentAppService
    {
        public PaymentAppService(IRepository<Payment, int> repository) : base(repository)
        {
        }

        public IList<Payment> Filter(long referenceNumber, int status, int userId, long reserveId, DateTime fromDate, DateTime toDate)
        {
            var model = Repository.Query(q => q.Where(p => p.Date <= toDate && p.Date >= fromDate));
            if (userId != -1)
            {
                model = model.Where(c => c.UserID == userId);
            }
            if (status != -1)
            {
                if (status == 0)
                    model = model.Where(p => p.Status == Payment.PaymentStatus.NotPaid);
                else
                    model = model.Where(p => p.Status == Payment.PaymentStatus.Paid);
            }
            if (referenceNumber > 0)
            {
                model = model.Where(p => p.RefID == referenceNumber);
            }
            if (reserveId > 0)
            {
                model = model.Where(p => p.ReserveID == reserveId);
            }
            return model.OrderByDescending(p => p.Id).ToList();
        }

        public IQueryable<Payment> Filter(int status, DateTime fromDate, DateTime toDate)
        {
            var model = Repository.Query(q => q.Where(
                p => p.Date <= toDate && p.Date >= fromDate));
            if (status != -1)
            {
                if (status == 0)
                    model = model.Where(p => p.Status == Payment.PaymentStatus.NotPaid);
                else
                    model = model.Where(p => p.Status == Payment.PaymentStatus.Paid);
            }
            return model;
        }

        public IList<Payment> GetRange(DateTime fromDate, DateTime toDate, int status, IList<int> userIds = null,
            bool byTotalPrice = false)
        {
            var data = Repository.Query(q => q.Where(w => w.Date >= fromDate && w.Date <= toDate && w.Status == (Payment.PaymentStatus)status));
            if (userIds != null && userIds.Count == 0)
            {
                data = data.Where(w => userIds.Contains(w.UserID));
            }
            if (byTotalPrice)
            {
                data = data.Where(w => w.TotalPrice > 0);
            }
            return data.ToList();
        }

        public int GetPaymentTriesCount(long reserveId, out string lastTryDateStr)
        {
            var payments = Repository.Query(q=>q.Where(x => x.ReserveID == reserveId && x.Status == Payment.PaymentStatus.NotPaid));
            if (payments.Any())
            {
                var paymentsList = payments.OrderByDescending(x => x.Date).ToList();
                var lastDate = paymentsList.Last().Date;
                lastTryDateStr = DateTimeUtility.GregorianToPersianDate(lastDate);
                lastTryDateStr += ("_" + lastDate.ToString("HH:mm"));
                return paymentsList.Count;
            }
            else
            {
                lastTryDateStr = "";
                return 0;
            }
        }

        public Payment Find(int id)
        {
            return Repository.Find(id);
        }

        public int Insert(Payment newPayment)
        {
            Repository.Insert(newPayment);
            Repository.Save();
            return newPayment.Id;
        }

        public bool CheckTransactionId(string transactionId, BankEnum bank = BankEnum.Unknown)
        {
            if (bank == BankEnum.Unknown)
            {
                return Repository.Query(q => q.Any(w => w.Authority == transactionId));
            }
            else
            {
                return Repository.Query(q => q.Any(w => w.BankId == bank && w.Authority == transactionId));
            }
        }

        public void Update(Payment editedPayment)
        {
            var payment = Repository.Find(editedPayment.Id);
            payment.Authority = editedPayment.Authority;
            payment.BankId = editedPayment.BankId;
            payment.Date = editedPayment.Date;
            payment.RefID = editedPayment.RefID;
            payment.Status = editedPayment.Status;
            payment.PayDate = editedPayment.PayDate;
            payment.WalletTransactionId = editedPayment.WalletTransactionId;
            payment.ReservePaymentId = editedPayment.ReservePaymentId;
            payment.TraceNumber = editedPayment.TraceNumber;
            Repository.Update(payment);
            Repository.Save();
        }

        public IQueryable<Payment> GetAllAsIQueryable()
        {
            return Repository.Query(q => q);
        }
    }
}
