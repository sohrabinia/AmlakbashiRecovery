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
using System.Linq;
using X.PagedList;
using Amlakbashi.Core.Common.Utilities;
using Amlakbashi.Core.DTOs.PaymentDTOs;
using Amlakbashi.Core.Common.StaticData;
using Amlakbashi.Accounting.PaymentContext;
using Amlakbashi.Host.Extensions;
using System.Xml.Linq;
using Newtonsoft.Json;
using System.Dynamic;

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

        [Auth(UserRoles.Admin)]
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

        [Auth(UserRoles.Admin)]
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

        [Auth(UserRoles.Admin)]
        public ActionResult PaymentIndex(int? page, long refid = 0, int status = -1, int uid = -1,
            string from_str = "", string to_str = "")
        {
            try
            {
                if (string.IsNullOrEmpty(from_str))
                    from_str = DateTimeUtility.ConvertDate(DateTime.Now.AddMonths(-1));

                if (string.IsNullOrEmpty(to_str))
                    to_str = DateTimeUtility.ConvertDate(DateTime.Now.AddDays(1));

                DateTime from_date = DateTimeUtility.ConvertDate(from_str);
                DateTime to_date = DateTimeUtility.ConvertDate(to_str);
                var model = accounting.FilterPayments(refid, status, uid, from_date, to_date);

                long sum = model.Select(p => (long?)p.TotalPrice).Sum() ?? 0;
                ViewBag.sum = sum;
                ViewBag.uid = uid;
                ViewBag.status = status;
                ViewBag.from_str = from_str;
                ViewBag.to_str = to_str;
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
                Payment objpay = accounting.FindPayment(id);
                #region [ Pasargad ]

                var amount = objpay.TotalPrice;
                string redirectAddress;
                if (useCustomRedirect)
                {
                    redirectAddress = GeneralData.WebsiteUrl + "/Cart/VerifyPasargadPayment"
                        + "?user_id=" + user_id
                        + "&useCustomRedirect=" + "true"
                        + "&customRedirectUrl=" + customRedirectUrl
                        + "&price=" + amount;
                }
                else
                {
                    redirectAddress = GeneralData.WebsiteUrl + "/Cart/VerifyPasargadPayment";
                    //var redirectAddress = "http://localhost:53552/Cart/VerifyPasargadPayment";
                    //var redirectAddress = "http://test.amlakbashi.com/Cart/VerifyPasargadPayment";
                }
                var result = accounting.GeneratePaymentData(BanksEnum.Pasargad,
                    (int)id, redirectAddress);
                return View("Bank", result);
                #endregion
            }
            catch (Exception exc)
            {
                logger.Error("Cart.ConfirmAndPayment", exc);
                TempData["payment_error_msg"] = "خطایی در هنگام پرداخت رخ داده . لطفا دوباره امتحان کنید .";
                return RedirectToAction("cart", "transactionresult");
            }
        }


        [Auth]
        public ActionResult PerformPay(int payment_id)
        {
            var objpay = accounting.FindPayment(payment_id);
            objpay.Date = DateTime.Now;
            ViewBag.PayPrice = objpay.TotalPrice;
            ViewBag.PayDate = DateTimeUtility.ConvertDate(objpay.Date);
            objpay.Authority = "";
            objpay.BankId = 0;
            accounting.UpdatePayment(objpay);
            return View(objpay);
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
                BanksEnum.Pasargad, pid, user_id,
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
                // TODO: check this on test
                //Response.Write("Invalid Input");
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
            else if (objpay.ProductType == Entities.User.CreditTransactionType.Credit_Increase.ToString())
            {
                redirect_controller = "user";
                redirect_action = "usercreditmanager";
            }
            else if (objpay.ProductType == Entities.User.CreditTransactionType.Credit_Inc_Then_Res.ToString())
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

#if DEBUG
        [Auth(UserRoles.Admin)]
        public ActionResult TestVerifyPasargad(int pid)
        {
            try
            {
                string msg;
                if (accounting.TestFinalizePayment(pid, userAccessor.CurrentUser.Id, out msg))
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
            var objpay = accounting.FindPayment(pid);
            string redirect_controller;
            string redirect_action;
            if (objpay.ProductType.Contains("Reserve"))
            {
                redirect_controller = "reserve";
                redirect_action = "reserveitemmanager?selecttype=1&category=2";

            }
            else if (objpay.ProductType == Entities.User.CreditTransactionType.Credit_Increase.ToString())
            {
                redirect_controller = "user";
                redirect_action = "usercreditmanager";
            }
            else if (objpay.ProductType == Entities.User.CreditTransactionType.Credit_Inc_Then_Res.ToString())
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
