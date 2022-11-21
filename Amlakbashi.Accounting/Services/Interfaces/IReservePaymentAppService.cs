using Amlakbashi.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using static Amlakbashi.Core.Entities.ReservePayment;

namespace Amlakbashi.Accounting.Services.Interfaces
{
    internal interface IReservePaymentAppService
    {
        IList<ReservePayment> GetAll();
        IQueryable<ReservePayment> GetAllAsIQueriable();
        IList<ReservePayment> Filter(long reservePaymentId, long reserveId, long advertiseId,
            int userId, int operatorId, int paymentType, int paymentMethod, long transactionId);
        IList<ReservePayment> Filter(int paymentType);
        ReservePayment Find(long id);
        ReservePayment Insert(ReservePayment reservePayment);
        void Insert(IList<ReservePayment> reservePayments);
        ReservePayment Insert(int user_id, long reserve_id, long transaction_id, long ref_id, ReservePaymentType type,
            long price, ReservePaymentMethod payment_method, int operator_id = 0, bool dontSave = false);
        void Update(ReservePayment editedData);
        void Delete(long id);
        bool Exists(long transactionId, int paymentMethod, long id = 0);
        long GetPaymentPrice(long reserve_id, ReservePaymentType type, out DateTime date,
             out long transactionId, int targetUserID = 0);
        List<long> GetPaidReserveIds();
        long GetPaidAmount(long reserveId, Reserve.StatusStringType payType, long exceptPaymentId = -1);
        long GetPaidAmount(IList<ReservePayment> reserve_payments, Reserve.StatusStringType type_of_pay,
            long except_payment_id = -1);
        bool ReserveShouldRefund(long reserveId, Reserve.ReserveStatus status, out bool refundDone);
    }
}
