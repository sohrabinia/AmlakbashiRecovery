using Amlakbashi.Accounting;
using Amlakbashi.Application.Services.ReserveServices.Interfaces;
using Amlakbashi.Application.Services.UserServices.Interfaces;
using Amlakbashi.Core.DTOs.CartDTOs;
using Amlakbashi.Core.Entities;
using Entities = Amlakbashi.Core.Entities;
using Amlakbashi.Core.Infrastructure.UserContact.Interfaces;
using Amlakbashi.Host.Authentication;
using Amlakbashi.Host.Controllers.Base;
using log4net;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using X.PagedList;
using Amlakbashi.Core.Common.Utilities;
using Amlakbashi.Core.Common.StaticData;
using Amlakbashi.Host.Extensions;
using System.Xml.Linq;
using Newtonsoft.Json;
using System.Dynamic;
using Microsoft.AspNetCore.Authorization;
using Amlakbashi.Core.Identity;
using System.Text;
using Amlakbashi.Core.Common.Enums;
using Amlakbashi.Core.DTOs.PaymentDTOs.BankEPayDTOs;

namespace Amlakbashi.Host.Controllers
{
    public class CartController : BaseController
    {
        private readonly IUserAppService userService;
        private readonly IReserveAppService reserveService;
        private readonly IAccountingFacade accounting;
        private readonly IUserContactFacade userContact;
        private readonly IUserAccessor userAccessor;
        private readonly ILog logger;
        public CartController(IUserAppService userService,
            IReserveAppService reserveService,
            IAccountingFacade accounting,
            IUserContactFacade userContact,
            IUserAccessor userAccessor,
            ILog logger
            )
        {
            this.userService = userService;
            this.reserveService = reserveService;
            this.accounting = accounting;
            this.userContact = userContact;
            this.userAccessor = userAccessor;
            this.logger = logger;
        }

        [Authorize(Policy = Policies.Payment_View)]
        public ActionResult Admin()
        {
            try
            {
                return View();
            }
            catch
            {
            }
            return RedirectToAction("AccessDenied", "Errors");
        }

        [Authorize(Policy = Policies.Payment_Actions)]
        public ActionResult Index(int? page, int status = -1, int uid = -1, long refid = -1)
        {
            try
            {
                IEnumerable<Cart> model = accounting.FilterCarts(status, uid, refid);
                ViewBag.uid = uid;
                ViewBag.status = status;
                ViewBag.refid = refid;
                var PageNumber = page ?? 1;
                var onePageOfModel = model.ToPagedList(PageNumber, 20);
                ViewBag.RowIndexStart = (PageNumber * 20) - 20;

                List<CartIndexDTO> cartDTOs = new List<CartIndexDTO>();
                foreach (var item in onePageOfModel)
                {
                    var dto = new CartIndexDTO()
                    {
                        Cart = item,
                        UserPhoneNumber = userService.Find(item.UserID).GetPhoneNumber(Entities.User.PhoneType.MainMobile)
                    };
                    cartDTOs.Add(dto);
                }
                ViewBag.dto = cartDTOs;
                return View(onePageOfModel);
            }
            catch (Exception exc)
            {
                logger.Error("Cart.Index", exc);
                return Redirect(Request.Headers["Referer"].ToString());
            }
        }

        public ActionResult TransactionResult()
        {
            try
            {
                return View();
            }
            catch (Exception exc)
            {
                logger.Error("Cart.TransactionResult", exc);
                return Redirect(Request.Headers["Referer"].ToString());
            }
        }

        public ActionResult PerformPay(int paymentId, string redirectUrl = null)
        {
            var payment = accounting.FindPayment(paymentId);
            if (payment == null || payment.Status == Payment.PaymentStatus.Paid)
            {
                return RedirectToRoute(new
                {
                    controller = "errors",
                    action = "http404"
                });
            }
            payment.Date = DateTime.Now;
            payment.BankId = 0;
            accounting.UpdatePayment(payment);
            ViewBag.PayPrice = payment.TotalPrice;
            ViewBag.PayDate = DateTimeUtility.ConvertDate(payment.Date);
            ViewBag.redirectUrl = redirectUrl;
            return View(payment);
        }

        public ActionResult TestPerformPay(int paymentId, string redirectUrl = null)
        {
            var payment = accounting.FindPayment(paymentId);
            if (payment == null || payment.Status == Payment.PaymentStatus.Paid)
            {
                return RedirectToRoute(new
                {
                    controller = "errors",
                    action = "http404"
                });
            }
            payment.Date = DateTime.Now;
            payment.BankId = 0;
            accounting.UpdatePayment(payment);
            ViewBag.PayPrice = payment.TotalPrice;
            ViewBag.PayDate = DateTimeUtility.ConvertDate(payment.Date);
            ViewBag.redirectUrl = redirectUrl;
            return View(payment);
        }

        public IActionResult Epay(BankEnum bank, int id, string RedirectUrl = null)
        {
            try
            {
                var result = accounting.GeneratePaymentData(bank, id, RedirectUrl);
                return View("EPay", result);
            }
            catch (Exception exc)
            {
                logger.Error("Cart.Epay", exc);
                TempData["payment_error_msg"] = "عملیات پرداخت با خطا مواجه شد";
                return RedirectToAction("cart", "transactionresult");
            }
        }

        [HttpPost]
        public IActionResult RegisterSamanEpay(SamanEpayResponseDTO response)
        {
            try
            {
                string msg = string.Empty;
                bool paymentResult = accounting.RegisterSamanEpay(response, out msg);
                return RedirectAfterEpay(paymentResult, msg, response.ResNum, response.RedirectUrl, response.RRN);
            }
            catch (Exception exc)
            {
                logger.Error("Cart.RegisterSamanEpay", exc);
                TempData["payment_error_msg"] = "عملیات پرداخت با خطا مواجه شد";
                return RedirectToAction("cart", "transactionresult");
            }
        }

        public IActionResult RegisterPasargadEpay(PasargadEpayResponseDTO response)
        {
            try
            {
                string msg = string.Empty;
                string referenceNumber = string.Empty;
                bool paymentResult = accounting.RegisterPasargadEpay(response, out msg, out referenceNumber);
                return RedirectAfterEpay(paymentResult, msg, response.iN, response.RedirectUrl, referenceNumber);
            }
            catch (Exception exc)
            {
                logger.Error("Cart.RegisterPasargadEpay", exc);
                TempData["payment_error_msg"] = "عملیات پرداخت با خطا مواجه شد";
                return RedirectToAction("cart", "transactionresult");
            }
        }

        private IActionResult RedirectAfterEpay(bool epayResult, string msg, int paymentId,
            string redirectUrl, string referenceNumber = null)
        {
            var payment = accounting.FindPayment(paymentId);
            if (string.IsNullOrEmpty(redirectUrl) == false)
            {
                if (epayResult)
                {
                    return Redirect(redirectUrl + "?done=true&price=" + payment.TotalPrice);
                }
                else
                {
                    return Redirect(redirectUrl + "?done=false");
                }
            }

            if (epayResult)
            {
                TempData["payment_success_msg"] = msg;
            }
            else
            {
                TempData["payment_error_msg"] = msg;
            }
            
            string redirect_controller;
            string redirect_action;
            if (payment.ProductType.Contains("Reserve"))
            {
                redirect_controller = "reserve";
                redirect_action = "reserveitemmanager?selecttype=1&category=2";

            }
            else if (payment.ProductType == CreditTransaction.WalletTransactionTypeForPayment.Credit_Increase.ToString())
            {
                redirect_controller = "user";
                redirect_action = "usercreditmanager";
            }
            else if (payment.ProductType == CreditTransaction.WalletTransactionTypeForPayment.Credit_Inc_Then_Res.ToString())
            {
                redirect_controller = "reserve";
                redirect_action = "reserveitemmanager?selecttype=1&category=2";
            }
            else
            {
                redirect_controller = "cart";
                redirect_action = "transactionresult";
            }
            if (redirect_action == "reserveitemmanager")
            {
                TempData["payment_transaction_id"] = referenceNumber;
                TempData.SetObjectAsJson("payment_reserve_id", payment.ReserveID);
            }
            return Redirect(string.Format("/{0}/{1}", redirect_controller, redirect_action));
        }

        //[Authorize(Policy = Policies.Payment_Actions)]
        //public IActionResult CheckPasargadPaymentResult(int paymentId)
        //{
        //    try
        //    {
        //        var result = accounting.CheckPaymentResult(paymentId);
        //        return PartialView("_CheckPaymentResult", result);
        //    }
        //    catch (Exception exc)
        //    {
        //        logger.Error("Cart.CheckPasargadPaymentResult", exc);
        //        return PartialView("_CheckPaymentResult");
        //    }
        //}

#if DEBUG
        public ActionResult LocalPay(int paymentId, string redirectUrl = null)
        {
            try
            {
                string msg;
                if (accounting.TestFinalizePayment(paymentId, userAccessor.CurrentUser.Id, out msg))
                {
                    TempData["payment_success_msg"] = msg;
                }
                else
                {
                    TempData["payment_error_msg"] = msg;
                }
            }
            catch (Exception exc)
            {
                TempData["payment_error_msg"] = exc.Message;
            }
            var objpay = accounting.FindPayment(paymentId);
            if (string.IsNullOrEmpty(redirectUrl) == false)
            {
                return Redirect(redirectUrl);
            }
            string redirect_controller;
            string redirect_action;
            if (objpay.ProductType.Contains("Reserve"))
            {
                redirect_controller = "reserve";
                redirect_action = "reserveitemmanager?selecttype=1&category=2";

            }
            else if (objpay.ProductType == CreditTransaction.WalletTransactionTypeForPayment.Credit_Increase.ToString())
            {
                redirect_controller = "user";
                redirect_action = "usercreditmanager";
            }
            else if (objpay.ProductType == CreditTransaction.WalletTransactionTypeForPayment.Credit_Inc_Then_Res.ToString())
            {
                redirect_controller = "reserve";
                redirect_action = "reserveitemmanager?selecttype=1&category=2";
            }
            else
            {
                redirect_controller = "cart";
                redirect_action = "transactionresult";
            }
            return Redirect(string.Format("/{0}/{1}", redirect_controller, redirect_action));
        }
#endif
    }
}
