using Amlakbashi.Accounting;
using Amlakbashi.Application.Services.UserServices.Interfaces;
using Amlakbashi.Core.Common.Utilities;
using Amlakbashi.Core.DTOs.PaymentDTOs;
using Amlakbashi.Core.Identity;
using Amlakbashi.Host.Controllers.Base;
using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using X.PagedList;
using Entities = Amlakbashi.Core.Entities;

namespace Amlakbashi.Host.Controllers
{
    public class PaymentController : BaseController
    {
        private readonly IAccountingFacade accounting;
        private readonly IUserAppService userService;
        private readonly ILog logger;
        public PaymentController(ILog logger, IAccountingFacade accounting, IUserAppService userService)
        {
            this.accounting = accounting;
            this.userService = userService;
            this.logger = logger;
        }

        [Authorize(Policy = Policies.Payment_View)]
        public ActionResult Index(int? page, long referenceNumber = 0, int status = -1, int userId = -1, long reserveId = 0,
            string fromDate = "", string toDate = "")
        {
            try
            {
                if (string.IsNullOrEmpty(fromDate))
                    fromDate = DateTimeUtility.ConvertDate(DateTime.Now.AddMonths(-1));

                if (string.IsNullOrEmpty(toDate))
                    toDate = DateTimeUtility.ConvertDate(DateTime.Now.AddDays(1));

                DateTime from_date = DateTimeUtility.ConvertDate(fromDate);
                DateTime to_date = DateTimeUtility.ConvertDate(toDate);
                var model = accounting.FilterPayments(referenceNumber, status, userId, reserveId, from_date, to_date);

                ViewBag.incomeSum = model.Where(w => w.Type == Entities.Payment.PaymentType.Income).Select(p => (long?)p.TotalPrice).Sum() ?? 0;
                ViewBag.expenditureSum = model.Where(w => w.Type == Entities.Payment.PaymentType.Expenditure).Select(p => (long?)p.TotalPrice).Sum() ?? 0;
                ViewBag.uid = userId;
                ViewBag.status = status;
                ViewBag.reserveId = reserveId;
                ViewBag.from_str = fromDate;
                ViewBag.to_str = toDate;
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
                logger.Error("Cart.PaymentIndex", exc);
                return Redirect(Request.Headers["Referer"].ToString());
            }
        }

        [Authorize(Policy = Policies.Payment_Actions)]
        public IActionResult CheckPodiumPaymentStatus(long paymentId)
        {
            try
            {
                var result = accounting.CheckShebaPaymentStatus(paymentId);
                return PartialView("_CheckPodiumPaymentStatus", result);
            }
            catch (Exception exc)
            {
                logger.Error("ReservePayment.CheckPodiumPaymentStatus", exc);
                return PartialView("_CheckPodiumPaymentStatus");
            }
        }
    }
}
