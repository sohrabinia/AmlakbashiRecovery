using System;
using System.Linq;
using Entities = Amlakbashi.Core.Entities;
using Amlakbashi.Application.Services.UserServices.Interfaces;
using Amlakbashi.Core.Entities;
using Amlakbashi.Application.Services.ReserveServices.Interfaces;
using Amlakbashi.Accounting;
using Amlakbashi.Core.Infrastructure.UserContact.Interfaces;
using Amlakbashi.Core.Infrastructure.UserContact;
using log4net;
using Amlakbashi.Host.Controllers.Base;
using Amlakbashi.Host.Authentication;
using Microsoft.AspNetCore.Mvc;
using X.PagedList;
using Microsoft.AspNetCore.Authorization;
using Amlakbashi.Core.Identity;

namespace Amlakbashi.Host.Controllers
{
    public class ReservePaymentController : BaseController
    {
        private readonly IUserAppService userService;
        private readonly IAccountingFacade accounting;
        private readonly IReserveAppService reserveService;
        private readonly IUserContactFacade userContact;
        private readonly IUserAccessor userAccessor;
        private readonly ILog logger;
        public ReservePaymentController(IUserAppService userService,
            IReserveAppService reserveService,
            IAccountingFacade accounting,
            IUserContactFacade userContact,
            IUserAccessor userAccessor,
            ILog logger)
        {
            this.userService = userService;
            this.accounting = accounting;
            this.reserveService = reserveService;
            this.userContact = userContact;
            this.userAccessor = userAccessor;
            this.logger = logger;
        }

        [Authorize(Policy = Policies.Reserve_View)]
        public ActionResult Index(int? page, long reserve_payment_id = -1, long reserve_id = -1,
            long advertise_id = -1,int user_id = -1, int operator_id = -1,
            int payment_type = -1, long transaction_id = -1, int status = 0)
        {
            try
            {
                var model = accounting.FilterReservePayment(reserve_payment_id, reserve_id,
                    advertise_id, user_id, operator_id, payment_type, transaction_id, status);
                var PageNumber = page ?? 1;
                var onePageOfModel = model.ToPagedList(PageNumber, 10);
                ViewBag.reserve_payment_id = reserve_payment_id;
                ViewBag.reserve_id = reserve_id;
                ViewBag.advertise_id = advertise_id;
                ViewBag.user_id = user_id;
                ViewBag.payment_type = payment_type;
                ViewBag.transaction_id = transaction_id;
                ViewBag.status = status;
                ViewBag.RowIndexStart = (PageNumber * 10) - 10;
                return View(onePageOfModel);
            }
            catch (Exception exc)
            {
                logger.Error("PaymentController.Index", exc);
                return Redirect(Request.Headers["referer"].ToString());
            }
        }

        [HttpGet]
        [Authorize(Policy = Policies.Reserve_Payment_Actions)]
        public ActionResult AddEdit(long reserve_payment_id = 0)
        {
            ReservePayment model;
            if (reserve_payment_id > 0)
            {
                model = accounting.FindReservePayment(reserve_payment_id);
            }
            else
            {
                var obj = new ReservePayment();
                obj.OperatorID = userAccessor.CurrentUser.Id;
                obj.PaymentType = -1;
                obj.PaymentMethod = (int)ReservePayment.ReservePaymentMethod.BankCard;
                model = obj;
            }
            return View(model);
        }

        [Authorize(Policy = Policies.Reserve_Payment_Edit)]
        public JsonResult CheckAddEdit(long reserve_payment_id, long reserve_id, int user_id, int payment_type, long price, long transaction_id, int method_id, bool confirmed = false, bool? send_sms = null)
        {
            try
            {
                var reserve = reserveService.Find(reserve_id);
                var invalid_data = false;
                var error_message = "";
                if (payment_type < 0)
                {
                    invalid_data = true;
                    error_message = "لطفا نوع پرداخت را انتخاب کنید";
                }
                if (price <= 0)
                {
                    invalid_data = true;
                    error_message = "لطفا قیمت را وارد کنید.";
                }
                else if (reserve_id <= 0)
                {
                    invalid_data = true;
                    error_message = "لطفا کد رزرو را وارد کنید.";
                }
                else if (transaction_id <= 0)
                {
                    invalid_data = true;
                    error_message = "لطفا شماره تراکنش را وارد کنید.";
                }
                else if (accounting.ReservePaymentExists( transaction_id, method_id, reserve_payment_id))
                {
                    invalid_data = true;
                    error_message = "شماره تراکنش تکراری میباشد.";
                }
                else if (user_id <= 0)
                {
                    invalid_data = true;
                    error_message = "لطفا کد کاربر را وارد کنید.";
                }
                if (invalid_data)
                {
                    return GenerateJsonResult(new { status = 0, msg = "خطا: " + error_message } );
                }
                switch ((ReservePayment.ReservePaymentType)payment_type)
                {
                    case ReservePayment.ReservePaymentType.GuestDeposite:
                    case ReservePayment.ReservePaymentType.GuestClearing:
                    case ReservePayment.ReservePaymentType.SiteRefundToGuest:
                        var guest = userService.Find(reserve.UserID);
                        var guest_name = guest.FName + " " + guest.LName;
                        if (string.IsNullOrEmpty(guest_name))
                            guest_name = guest.GetPhoneNumber(Entities.User.PhoneType.MainMobile);
                        return GenerateJsonResult(new { status = 1, guest_name = guest_name, askBeforeSubmit = true });
                }
                if (reserve_payment_id > 0)
                {
                    return GenerateJsonResult(new { status = 1, askBeforeSubmit = false });
                }
                var user = userService.Find(reserve.Advertise.UserID);
                if (!confirmed)
                {
                    var host_name = user.FName + " " + user.LName;
                    if (string.IsNullOrEmpty(host_name))
                        host_name = user.GetPhoneNumber(Entities.User.PhoneType.MainMobile);
                    return GenerateJsonResult(new { status = 2, host_name = host_name,
                        price = string.Format("{0:n0}", price) + " تومان" });
                }
                if (send_sms == null)
                {
                    return GenerateJsonResult(new { status = 3 });
                }
                if ((bool)send_sms)
                {
                    userService.SendMessage(new UserContactDTO()
                    {
                        UserMainMobile = user.MainMobile,
                        UserAppNotificationToken = user.AppNotificationToken,
                        UserEmail = user.Email,
                        UserFcmAppNotificationToken = user.FcmAppNotificationToken,
                        UserNotificationToken = user.NotificationToken,
                        Type = UserContactType.SiteClearingHost,
                        AdvertiseId = reserve.AdvertiseID.ToString(),
                        Price = price.ToString(),
                        TransactionId = transaction_id.ToString(),
                        ReserveId = reserve.Id.ToString()
                    });
                }
                return GenerateJsonResult(new { status = 1, askBeforeSubmit = false });
            }
            catch (Exception exc)
            {
                logger.Error("PaymentController.CheckAddEdit", exc);
                return GenerateJsonResult(new { status = 0 });
            }
        }

        [HttpPost]
        [Authorize(Policy = Policies.Reserve_Payment_Edit)]
        public ActionResult AddEdit(ReservePayment reserve_payment)
        {
            try
            {
                var invalid_data = false;
                var error_message = "";
                if (reserve_payment.PaymentType < 0)
                {
                    invalid_data = true;
                    error_message = "لطفا نوع پرداخت را انتخاب کنید";
                }
                if (reserve_payment.Price <= 0)
                {
                    invalid_data = true;
                    error_message = "لطفا قیمت را وارد کنید.";
                }
                else if (reserve_payment.ReserveID <= 0)
                {
                    invalid_data = true;
                    error_message = "لطفا کد رزرو را وارد کنید.";
                }
                else if (reserve_payment.TransactionID <= 0)
                {
                    invalid_data = true;
                    error_message = "لطفا شماره تراکنش را وارد کنید.";
                }
                else if (accounting.ReservePaymentExists(reserve_payment.TransactionID,
                    reserve_payment.PaymentMethod, reserve_payment.Id))
                {
                    invalid_data = true;
                    error_message = "شماره تراکنش تکراری میباشد.";
                }
                else if (reserve_payment.UserID <= 0)
                {
                    invalid_data = true;
                    error_message = "لطفا کد کاربر را وارد کنید.";
                }
                if (invalid_data)
                {
                    ViewBag.OperationStatus = 0;
                    ViewBag.AlertMessage = "خطا: " + error_message;
                    return View(reserve_payment);
                }
                ReservePayment obj_reserve_payment = null;
                var is_edit = reserve_payment.Id > 0;
                if (is_edit)
                {
                    obj_reserve_payment = accounting.FindReservePayment(reserve_payment.Id);
                    obj_reserve_payment.ReserveID = reserve_payment.ReserveID;
                    obj_reserve_payment.TransactionID = reserve_payment.TransactionID;
                    obj_reserve_payment.UserID = reserve_payment.UserID;
                    obj_reserve_payment.PaymentType = reserve_payment.PaymentType;
                    obj_reserve_payment.Price = reserve_payment.Price;
                    obj_reserve_payment.PaymentMethod = reserve_payment.PaymentMethod;
                    obj_reserve_payment.OperatorID = userAccessor.CurrentUser.Id;
                    accounting.UpdateReservePayment(obj_reserve_payment);
                }
                else
                {
                    reserve_payment.CreateDate = DateTime.Now;
                    reserve_payment.OperatorID = userAccessor.CurrentUser.Id;
                    obj_reserve_payment = accounting.InsertReservePayment(reserve_payment);
                }
                if (reserve_payment.PaymentType == (int)ReservePayment.ReservePaymentType.GuestDeposite
                    || reserve_payment.PaymentType == (int)ReservePayment.ReservePaymentType.GuestClearing)
                {
                    var reserve = reserveService.Find(reserve_payment.ReserveID);
                    var payed_price = accounting.GetReservePaidAmount(reserve.ReservePayments.ToList(),
                        Reserve.StatusStringType.Guest);
                    if (reserve.Status == Reserve.ReserveStatus.WaitForReserve)
                    {
                        if (payed_price + reserve.CouponPrice + reserve.PrizePrice >= reserve.DepositPrice)
                            reserveService.SetStatus(reserve_payment.ReserveID, Reserve.ReserveStatus.Reserved,
                                true, ActionLog.ActionSourceEnum.AdminPanel, userAccessor.CurrentUser.Id);
                    }
                }
                ViewBag.OperationStatus = 1;
                ViewBag.AlertMessage = is_edit ? "پرداخت مورد نظر با موفقیت ویرایش شد" : "پرداخت مورد نظر با موفقیت اضافه شد";
                var obj = new ReservePayment();
                obj.OperatorID = userAccessor.CurrentUser.Id;
                obj.PaymentType = -1;
                obj.PaymentMethod = (int)ReservePayment.ReservePaymentMethod.BankCard;
                return View(is_edit ? obj_reserve_payment : obj);
            }
            catch(Exception exc)
            {
                logger.Error("PaymentController.AddEdit", exc);
                ViewBag.OperationStatus = 0;
                ViewBag.AlertMessage = "خطا: " + exc.Message;
                return View(reserve_payment);
            }
        }

        [Authorize(Policy = Policies.Reserve_Payment_Edit)]
        public JsonResult Delete(long reserve_payment_id)
        {
            try
            {
                accounting.DeleteReservePayment(reserve_payment_id);
                return GenerateJsonResult(new { status = 1 });
            }
            catch (Exception exc)
            {
                logger.Error("PaymentController.Delete", exc);
                return GenerateJsonResult(new { status = 0 });
            }
        }
    }
}