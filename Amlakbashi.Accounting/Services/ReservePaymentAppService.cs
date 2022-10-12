using Amlakbashi.Core.Common.AppService;
using Amlakbashi.Core.Common.Repository;
using Amlakbashi.Accounting.Services.Interfaces;
using Amlakbashi.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using static Amlakbashi.Core.Entities.ReservePayment;

namespace Amlakbashi.Accounting.Services
{
    internal class ReservePaymentAppService : AppServiceBase<ReservePayment, long>, IReservePaymentAppService
    {
        public ReservePaymentAppService(IRepository<ReservePayment, long> repository) : base(repository)
        {
        }

        public IList<ReservePayment> GetAll()
        {
            return Repository.Query(q => q.ToList());
        }

        public IQueryable<ReservePayment> GetAllAsIQueriable()
        {
            return Repository.Query(q => q);
        }

        public IList<ReservePayment> Filter(long reservePaymentId, long reserveId, long advertiseId,
            int userId, int operatorId, int paymentType, int paymentMethod, long transactionId)
        {
            IQueryable<ReservePayment> model = Repository.Query(q => q);
            if (reservePaymentId > 0)
            {
                model = model.Where(x => x.Id == reservePaymentId);
            }
            if (transactionId > 0)
            {
                model = model.Where(x => x.TransactionID == transactionId);
            }
            if (reserveId > 0)
            {
                model = model.Where(x => x.ReserveID == reserveId);
            }
            if (userId > 0)
            {
                model = model.Where(x => x.UserID == userId);
            }
            if (operatorId > -1)
            {
                model = model.Where(x => x.OperatorID == operatorId);
            }
            if (advertiseId > 0)
            {
                model = model.Where(x => x.Reserve.AdvertiseID == advertiseId);
            }
            if (paymentType > -1)
            {
                if (paymentType == 100)
                {
                    model = model.Where(x =>
                    x.PaymentType == (int)ReservePayment.ReservePaymentType.GuestClearing ||
                    x.PaymentType == (int)ReservePayment.ReservePaymentType.GuestDeposite);
                }
                else
                {
                    model = model.Where(x => x.PaymentType == paymentType);
                }
            }
            if (paymentMethod > -1)
            {
                model = model.Where(w => w.PaymentMethod == paymentMethod);
            }
            return model.OrderByDescending(x => x.CreateDate).ToList();
        }

        public IList<ReservePayment> Filter(int paymentType)
        {
            return Repository.Query(q => q.Where(w => w.PaymentType == paymentType)).ToList();
        }

        public ReservePayment Find(long id)
        {
            return Repository.Query(q => q.FirstOrDefault(f => f.Id == id));
        }

        public ReservePayment Insert(ReservePayment reservePayment)
        {
            Repository.Insert(reservePayment);
            Repository.Save();
            return reservePayment;
        }

        public void Insert(IList<ReservePayment> reservePayments)
        {
            Repository.Insert(reservePayments);
            Repository.Save();
        }

        public ReservePayment Insert(int user_id, long reserve_id, long transaction_id, long ref_id,
            ReservePaymentType type, long price, ReservePaymentMethod payment_method, int operator_id = 0, bool dontSave = false)
        {
            IQueryable<ReservePayment> reservePayments = Repository.Query(q =>
                q.Where(x => x.CreateDate >= DateTime.Now.Date.AddDays(-20).Date));
            if (reservePayments.Any(x => x.TransactionID == transaction_id && x.PaymentMethod == (int)payment_method))
            {
                return null;
            }

            var reservePayment = new ReservePayment();
            reservePayment.PaymentType = (int)type;
            reservePayment.ReserveID = reserve_id;
            reservePayment.UserID = user_id;
            reservePayment.TransactionID = transaction_id;
            reservePayment.RefID = ref_id;
            reservePayment.Price = price;
            reservePayment.PaymentMethod = (int)payment_method;
            reservePayment.CreateDate = DateTime.Now;
            reservePayment.OperatorID = operator_id;
            
            if (!dontSave)
            {
                Repository.Insert(reservePayment);
                Repository.Save();
            }
            return reservePayment;
        }

        public void Update(ReservePayment editedData)
        {
            var data = Repository.Query(q => q.FirstOrDefault(f => f.Id == editedData.Id));
            data.ReserveID = editedData.ReserveID;
            data.TransactionID = editedData.TransactionID;
            data.UserID = editedData.UserID;
            data.PaymentType = editedData.PaymentType;
            data.Price = editedData.Price;
            data.PaymentMethod = editedData.PaymentMethod;
            data.OperatorID = editedData.OperatorID;
            Repository.Update(data);
            Repository.Save();
        }

        public bool Exists(long transactionId, int paymentMethod, long id = 0)
        {
            var data = Repository.Query(q => q.Where(x => x.TransactionID == transactionId && x.PaymentMethod == paymentMethod));
            if (id > 0)
            {
                data = data.Where(w => w.Id != id);
            }
            return data.Any();
        }

        public void Delete(long id)
        {
            Repository.Delete(id);
            Repository.Save();
        }

        public long GetPaymentPrice(long reserve_id, ReservePaymentType type, out DateTime date,
             out long transactionId, int targetUserID = 0)
        {
            var payments = Repository.Query(q => q);
            payments = payments.Where(x => x.ReserveID == reserve_id);
            payments = payments.Where(x => x.PaymentType == (int)type);
            if (targetUserID > 0)
            {
                payments = payments.Where(x => x.UserID == targetUserID);
            }
            var any_payments = payments.Any();
            if (any_payments)
            {
                var payment = payments.AsEnumerable().Last();
                date = payment.CreateDate;
                transactionId = payment.TransactionID;
            }
            else
            {
                date = DateTime.MinValue;
                transactionId = 0;
            }
            return any_payments ? payments.Sum(x => x.Price) : 0;
        }

        public List<long> GetPaidReserveIds()
        {
            return Repository.Query(q => q.Where(x => x.PaymentType == (int)ReservePaymentType.GuestClearing
                  || x.PaymentType == (int)ReservePaymentType.GuestDeposite).Select(s => s.ReserveID).Distinct().ToList());
        }

        public long GetPaidAmount(long reserveId, Reserve.StatusStringType payType, long exceptPaymentId = -1)
        {
            var reservePayments = Repository.Find<Reserve, long>(reserveId)
                .ReservePayments.Where(x => x.Id != exceptPaymentId);
            long payedPrice = 0;
            switch (payType)
            {
                case Reserve.StatusStringType.Guest:
                    var guest_payments = reservePayments.Where(
                        x => x.PaymentType == (int)ReservePaymentType.GuestDeposite ||
                        x.PaymentType == (int)ReservePaymentType.GuestClearing);
                    var site_refunds_to_guest = reservePayments.Where(
                        x => x.PaymentType == (int)ReservePaymentType.SiteRefundToGuest);
                    payedPrice =
                        (guest_payments.Any() ? guest_payments.Sum(x => x.Price) : 0)
                        -
                        (site_refunds_to_guest.Any() ? site_refunds_to_guest.Sum(x => x.Price) : 0);
                    break;
                case Reserve.StatusStringType.Host:
                    //Not yet have host payment
                    break;
                case Reserve.StatusStringType.Site:
                    var site_payments = reservePayments.Where(x =>
                        x.PaymentType == (int)ReservePaymentType.SiteClearingToHost ||
                        x.PaymentType == (int)ReservePaymentType.SiteDepositeToHost ||
                        x.PaymentType == (int)ReservePaymentType.SiteRefundToGuest);
                    payedPrice = site_payments.Any() ? site_payments.Sum(x => x.Price) : 0;
                    break;
                default:
                    break;
            }
            return payedPrice;
        }

        public long GetPaidAmount(IList<ReservePayment> reserve_payments,
            Reserve.StatusStringType type_of_pay, long except_payment_id = -1)
        {
            reserve_payments = reserve_payments.Where(x => x.Id != except_payment_id).ToList();
            long paidAmount = 0;
            switch (type_of_pay)
            {
                case Reserve.StatusStringType.Guest:
                    var guest_payments = reserve_payments.Where(
                        x => x.PaymentType == (int)ReservePaymentType.GuestDeposite ||
                        x.PaymentType == (int)ReservePaymentType.GuestClearing);
                    var site_refunds_to_guest = reserve_payments.Where(
                        x => x.PaymentType == (int)ReservePaymentType.SiteRefundToGuest);
                    paidAmount =
                        (guest_payments.Any() ? guest_payments.Sum(x => x.Price) : 0)
                        -
                        (site_refunds_to_guest.Any() ? site_refunds_to_guest.Sum(x => x.Price) : 0);
                    break;
                case Reserve.StatusStringType.Host:
                    //Not yet have host payment
                    break;
                case Reserve.StatusStringType.Site:
                    var site_payments = reserve_payments.Where(x =>
                        x.PaymentType == (int)ReservePaymentType.SiteClearingToHost ||
                        x.PaymentType == (int)ReservePaymentType.SiteDepositeToHost ||
                        x.PaymentType == (int)ReservePaymentType.SiteRefundToGuest);
                    paidAmount = site_payments.Any() ? site_payments.Sum(x => x.Price) : 0;
                    break;
                default:
                    break;
            }
            return paidAmount;
        }

        public bool ReserveShouldRefund(long reserveId, Reserve.ReserveStatus status, out bool refundDone)
        {
            var data = Repository.Query(q=>q.Where(x => x.ReserveID == reserveId));
            if (data.Any(x => x.PaymentType == (int)ReservePaymentType.SiteRefundToGuest))
            {
                refundDone = true;
                return true;
            }
            refundDone = false;
            switch (status)
            {
                case Reserve.ReserveStatus.WaitForResponse:
                case Reserve.ReserveStatus.WaitForReserve:
                case Reserve.ReserveStatus.Rejected:
                case Reserve.ReserveStatus.Reserved:
                case Reserve.ReserveStatus.CashPay:
                case Reserve.ReserveStatus.Started:
                case Reserve.ReserveStatus.Completed:
                case Reserve.ReserveStatus.CancelRequestByGuest:
                case Reserve.ReserveStatus.CancelRequestByHost:
                    return false;
            }
            return data.Any(x => x.PaymentType == (int)ReservePaymentType.GuestClearing ||
                x.PaymentType == (int)ReservePaymentType.GuestDeposite) &&
                data.Any(x=> x.PaymentType == (int)ReservePaymentType.SiteClearingToHost) == false;
        }
    }
}
