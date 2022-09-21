using Amlakbashi.Accounting;
using Amlakbashi.Application.Services.UserServices.Interfaces;
using Amlakbashi.Core.Common.Enums;
using Amlakbashi.Core.Common.Utilities;
using Amlakbashi.Core.DTOs.PaymentDTOs;
using Amlakbashi.Core.Identity;
using Amlakbashi.Host.Authentication;
using Amlakbashi.Host.Controllers.Base;
using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;
using Entities = Amlakbashi.Core.Entities;

namespace Amlakbashi.Host.Controllers
{
    public class PaymentController : BaseController
    {
        private readonly IAccountingFacade accounting;
        private readonly IUserAppService userService;
        private readonly IUserAccessor userAccessor;
        private readonly ILog logger;
        public PaymentController(ILog logger, IAccountingFacade accounting,
            IUserAppService userService, IUserAccessor userAccessor)
        {
            this.accounting = accounting;
            this.userService = userService;
            this.userAccessor = userAccessor;
            this.logger = logger;
        }

        [Authorize(Policy = Policies.Payment_View)]
        public ActionResult Index(int? page, long referenceNumber = 0, int status = -1, int userId = -1, long reserveId = 0,
            string fromDate = "", string toDate = "", BankEnum bank = BankEnum.Unknown, int type = -1)
        {
            try
            {
                if (string.IsNullOrEmpty(fromDate))
                    fromDate = DateTimeUtility.ConvertDate(DateTime.Now.AddMonths(-1));

                if (string.IsNullOrEmpty(toDate))
                    toDate = DateTimeUtility.ConvertDate(DateTime.Now.AddDays(1));

                DateTime from_date = DateTimeUtility.ConvertDate(fromDate);
                DateTime to_date = DateTimeUtility.ConvertDate(toDate);
                var model = accounting.FilterPayments(referenceNumber, status, userId, reserveId, from_date, to_date, bank, type);

                ViewBag.incomeSum = model.Where(w => w.Type == Entities.Payment.PaymentType.Income &&
                    w.Status == Entities.Payment.PaymentStatus.Paid).Select(p => (long?)p.Amount).Sum() ?? 0;
                ViewBag.expenditureSum = model.Where(w => w.Type == Entities.Payment.PaymentType.Expenditure &&
                    w.Status == Entities.Payment.PaymentStatus.Paid).Select(p => (long?)p.Amount).Sum() ?? 0;
                ViewBag.uid = userId;
                ViewBag.status = status;
                ViewBag.reserveId = reserveId;
                ViewBag.from_str = fromDate;
                ViewBag.to_str = toDate;
                ViewBag.bank = (int)bank;
                ViewBag.type = type;
                var PageNumber = page ?? 1;
                var onePageOfModel = model.ToPagedList(PageNumber, 20);
                ViewBag.RowIndexStart = (PageNumber * 20) - 20;

                List<PaymentIndexDTO> paymentDTOs = new List<PaymentIndexDTO>();
                foreach (var item in onePageOfModel)
                {
                    var dto = new PaymentIndexDTO()
                    {
                        Payment = item,
                        UserPhoneNumber = userService.Find(item.UserID).GetPhoneNumber(Entities.User.PhoneType.MainMobile)
                    };
                    paymentDTOs.Add(dto);
                }
                ViewBag.dto = paymentDTOs;
                return View(onePageOfModel);
            }
            catch (Exception exc)
            {
                logger.Error("Payment.Index", exc);
                return Redirect(Request.Headers["Referer"].ToString());
            }
        }

        [Authorize(Policy = Policies.Payment_Actions)]
        public IActionResult CheckPodiumPaymentStatus(long paymentId)
        {
            try
            {
                var result = accounting.CheckPayaPaymentStatus(paymentId);
                return PartialView("_CheckPodiumPaymentStatus", result);
            }
            catch (Exception exc)
            {
                logger.Error("Payment.CheckPodiumPaymentStatus", exc);
                return PartialView("_CheckPodiumPaymentStatus");
            }
        }

        [Authorize(Policy = Policies.Payment_Actions)]
        public IActionResult RegisterPodiumNotPaidPayment(long paymentId)
        {
            try
            {
                var result = accounting.RegisterNotPaidPayment(paymentId, userAccessor.CurrentUser.Id);
                return GenerateJsonResult(new
                {
                    status = result ? 1 : 0
                });
            }
            catch (Exception exc)
            {
                logger.Error("Payment.RegisterNotPaidPayment", exc);
                return GenerateJsonResult(new
                {
                    status = 0
                });
            }
        }

        [Authorize(Policy = Policies.Payment_Actions)]
        public async Task<IActionResult> CheckPasargadPaymentResult(int paymentId)
        {
            try
            {
                var result = await accounting.CheckPaymentResult(paymentId);
                return PartialView("_CheckPasargadPaymentResult", result);
            }
            catch (Exception exc)
            {
                logger.Error("Payment.CheckPasargadPaymentResult", exc);
                return PartialView("_CheckPasargadPaymentResult");
            }
        }

        [Authorize(Policy = Policies.Payment_Actions)]
        public async Task<IActionResult> EditPaymentByReinquiry(int paymentId)
        {
            try
            {
                var result = await accounting.EditPaymentByReinquiry(paymentId);
                return GenerateJsonResult(new
                {
                    status = result ? 1 : 0
                });
            }
            catch (Exception exc)
            {
                logger.Error("Payment.EditPaymentByReinquiry", exc);
                return GenerateJsonResult(new
                {
                    status = 0
                });
            }
        }

        [Authorize(Policy = Policies.Payment_Actions)]
        [HttpPost]
        public IActionResult PodiumRepayment(int paymentId)
        {
            try
            {
                var result = accounting.PodiumRepayment(paymentId, userAccessor.CurrentUser.Id);
                return GenerateJsonResult(new
                {
                    status = result.HasError ? 0 : 1,
                    msg = result.HasError ? result.ErrorMessage : result.Message
                });
            }
            catch (Exception exc)
            {
                logger.Error("Payment.PodiumRepayment", exc);
                return GenerateJsonResult(new
                {
                    status = 0,
                    msg = "عملیات با خطای فنی مواجه شد"
                });
            }
        }
    }
}
