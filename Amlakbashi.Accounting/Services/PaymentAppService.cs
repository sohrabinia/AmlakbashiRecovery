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

        public IList<Payment> Filter(long referenceNumber, int status, int userId,
            long reserveId, DateTime fromDate, DateTime toDate, BankEnum bank, int type)
        {
            var model = Repository.Query(q => q.Where(p => p.CreateDate <= toDate && p.CreateDate >= fromDate));
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
                model = model.Where(p => p.ReferenceNumber == referenceNumber);
            }
            if (reserveId > 0)
            {
                model = model.Where(p => p.ReserveID == reserveId);
            }
            if (bank != BankEnum.Unknown)
            {
                model = model.Where(x => x.Bank == bank);
            }
            if (type == 0 || type == 1)
            {
                model = model.Where(x => x.Type == (Payment.PaymentType)type);
            }
            return model.OrderByDescending(p => p.Id).ToList();
        }

        public IQueryable<Payment> Filter(int status, DateTime fromDate, DateTime toDate)
        {
            var model = Repository.Query(q => q.Where(
                p => p.CreateDate <= toDate && p.CreateDate >= fromDate));
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
            var data = Repository.Query(q => q.Where(w => w.CreateDate >= fromDate && w.CreateDate <= toDate && w.Status == (Payment.PaymentStatus)status));
            if (userIds != null && userIds.Count == 0)
            {
                data = data.Where(w => userIds.Contains(w.UserID));
            }
            if (byTotalPrice)
            {
                data = data.Where(w => w.Amount > 0);
            }
            return data.ToList();
        }

        public int GetPaymentTriesCount(long reserveId, out string lastTryDateStr)
        {
            var payments = Repository.Query(q=>q.Where(x => x.ReserveID == reserveId && x.Status == Payment.PaymentStatus.NotPaid));
            if (payments.Any())
            {
                var paymentsList = payments.OrderByDescending(x => x.CreateDate).ToList();
                var lastDate = paymentsList.Last().CreateDate;
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
                return Repository.Query(q => q.Any(w => w.TransactionId == transactionId));
            }
            else
            {
                return Repository.Query(q => q.Any(w => w.Bank == bank && w.TransactionId == transactionId));
            }
        }

        public void Update(Payment editedPayment)
        {
            var payment = Repository.Find(editedPayment.Id);
            payment.TransactionId = editedPayment.TransactionId;
            payment.Bank = editedPayment.Bank;
            payment.CreateDate = editedPayment.CreateDate;
            payment.ReferenceNumber = editedPayment.ReferenceNumber;
            payment.Status = editedPayment.Status;
            payment.PayDate = editedPayment.PayDate;
            payment.WalletTransactionId = editedPayment.WalletTransactionId;
            payment.ReservePaymentId = editedPayment.ReservePaymentId;
            payment.TraceNumber = editedPayment.TraceNumber;
            Repository.Update(payment);
            Repository.Save();
        }

        public void UpdateTransactionId(int paymentId, string transactionId)
        {
            var payment = Repository.Find(paymentId);
            payment.TransactionId = transactionId;
            Repository.Update(payment);
            Repository.Save();
        }

        public IQueryable<Payment> GetAllAsIQueryable()
        {
            return Repository.Query(q => q);
        }
    }
}
