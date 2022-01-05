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

        public ActionResult ConfirmAndPayment(long id, int bank, bool useCustomRedirect = false, int user_id = 0, string customRedirectUrl = null, long price = 0)
        {
            try
            {
                Payment payment = accounting.FindPayment(id);
                if (payment == null || payment.Status == Payment.PaymentStatus.Paid)
                {
                    return RedirectToRoute(new
                    {
                        controller = "errors",
                        action = "http404"
                    });
                }

                string redirectAddress;
                if (useCustomRedirect)
                {
                    redirectAddress = GeneralData.WebsiteUrl + "/Cart/VerifyPasargadPayment"
                        + "?user_id=" + user_id
                        + "&useCustomRedirect=" + "true"
                        + "&customRedirectUrl=" + customRedirectUrl
                        + "&price=" + payment.TotalPrice;
                }
                else
                {
                    redirectAddress = GeneralData.WebsiteUrl + "/Cart/VerifyPasargadPayment";
                    //var redirectAddress = "http://localhost:53552/Cart/VerifyPasargadPayment";
                    //var redirectAddress = "http://test.amlakbashi.com/Cart/VerifyPasargadPayment";
                }
                var result = accounting.GeneratePaymentData(BankEnum.Pasargad, (int)id, redirectAddress);
                return View("Bank", result);
            }
            catch (Exception exc)
            {
                logger.Error("Cart.ConfirmAndPayment", exc);
                TempData["payment_error_msg"] = "خطایی در هنگام پرداخت رخ داده . لطفا دوباره امتحان کنید .";
                return RedirectToAction("cart", "transactionresult");
            }
        }

        public ActionResult PerformPay(int payment_id, string redirectUrl = null)
        {
            var payment = accounting.FindPayment(payment_id);
            if (payment == null || payment.Status == Payment.PaymentStatus.Paid)
            {
                return RedirectToRoute(new
                {
                    controller = "errors",
                    action = "http404"
                });
            }
            payment.Date = DateTime.Now;
            payment.Authority = "";
            payment.BankId = 0;
            accounting.UpdatePayment(payment);
            ViewBag.PayPrice = payment.TotalPrice;
            ViewBag.PayDate = DateTimeUtility.ConvertDate(payment.Date);
            ViewBag.redirectUrl = redirectUrl;
            return View(payment);
        }

        public ActionResult VerifyPasargadPayment(int user_id = 0, bool useCustomRedirect = false,
            string customRedirectUrl = null, long price = 0)
        {
            if (user_id < 1)
            {
                user_id = userAccessor.DoerUser.Id;
            }
            string tref = Request.Query["tref"];
            var pid = int.Parse(Request.Query["iN"]);
            var date = DateTime.Parse(Request.Query["iD"]);
            string msg = "";
            string paymentResult;
            bool invalidInput;
            bool payment_done = accounting.FinalizePayment(
                BankEnum.Pasargad, pid, user_id,
                date, tref, out paymentResult, out msg, out invalidInput,
                useCustomRedirect ? ActionLog.ActionSourceEnum.Application :
                ActionLog.ActionSourceEnum.WebsiteDashboard, user_id);
            HttpContext.Session.SetObjectAsJson("resp", paymentResult);
            if (payment_done)
            {
                TempData["payment_success_msg"] = msg;
            }
            else
            {
                TempData["payment_error_msg"] = msg;
            }
            if (invalidInput)
            {
                var bytes = Encoding.UTF8.GetBytes("Invalid Input");
                HttpContext.Response.Body.WriteAsync(bytes, 0, bytes.Length);
            }
            if (useCustomRedirect)
            {
                if (payment_done)
                {
                    return Redirect(customRedirectUrl + "?done=true&price=" + price);
                }
                else
                {
                    return Redirect(customRedirectUrl + "?done=false");
                }
            }
            var objpay = accounting.FindPayment(pid);
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
            if (redirect_action == "reserveitemmanager")
            {
                XDocument doc = XDocument.Parse(HttpContext.Session.GetObjectFromJson<string>("resp")); //or XDocument.Load(path)
                string jsonText = JsonConvert.SerializeXNode(doc);
                dynamic resp = ((dynamic)JsonConvert.DeserializeObject<ExpandoObject>(jsonText)).resultObj;
                TempData["payment_transaction_id"] = (string)resp.referenceNumber;
                TempData.SetObjectAsJson("payment_reserve_id", accounting.FindPayment(pid).ReserveID);
            }
            return Redirect(string.Format("/{0}/{1}", redirect_controller, redirect_action));
        }

        [Authorize(Policy = Policies.Payment_Actions)]
        public IActionResult CheckPasargadPaymentResult(int paymentId)
        {
            try
            {
                var result = accounting.CheckPaymentResult(paymentId);
                return PartialView("_CheckPaymentResult", result);
            }
            catch (Exception exc)
            {
                logger.Error("Cart.CheckPasargadPaymentResult", exc);
                return PartialView("_CheckPaymentResult");
            }
            
        }

#if DEBUG
        //[Authorize(Roles = Roles.TechnicalManager + "," + Roles.TechnicalEmployee)]
        public ActionResult LocalPay(int payment_id, string redirectUrl = null)
        {
            try
            {
                string msg;
                if (accounting.TestFinalizePayment(payment_id, userAccessor.CurrentUser.Id, out msg))
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
            var objpay = accounting.FindPayment(payment_id);
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
