using System;
using Entities = Amlakbashi.Core.Entities;
using Amlakbashi.Core.Entities;
using static Amlakbashi.Core.Entities.Reserve;
using Amlakbashi.Core.Common.StaticData;
using Amlakbashi.Host.Controllers.Base;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Amlakbashi.Host.Controllers.API
{
    public partial class ApiController : Controller
    {

        [Authorize(AuthenticationSchemes = bearerScheme)]
        public ActionResult UserIncreaseCredit(long price, string redirectUrl)
        {
            try
            {
                var user = GetUser();
                var payment = new Payment()
                {
                    UserID = user.Id,
                    Date = DateTime.Now,
                    TotalPrice = price * 10,
                    ProductType = Entities.User.CreditTransactionType.Credit_Increase.ToString()
                };
                accounting.InsertPayment(payment);
                return Redirect(GeneralData.WebsiteUrl + "/Cart/ConfirmAndPayment?id=" + payment.Id
                    + "&bank=2" + "&useCustomRedirect=true"
                    + "&user_id=" + user.Id
                    + "&customRedirectUrl=" + redirectUrl
                    + "&price=" + price);
            }
            catch(Exception exc)
            {
                logger.Error("PaymentApi.UserIncreaseCredit", exc);
                return null;
            }
        }

        [Authorize(AuthenticationSchemes = bearerScheme)]
        public ActionResult GuestPayReserve(long reserve_id, int pay_reserve_type, string redirectUrl,
            bool useCoupon = false, bool usePrize = false)
        {
            try
            {
                var user = GetUser();
                long payment_id;
                var result = accounting.GuestPayReserve(user.Id, reserve_id, pay_reserve_type, out payment_id, user.Id, ActionLog.ActionSourceEnum.Application, useCoupon, usePrize, 0);
                switch (result)
                {
                    case GuestPayResult.ReadyToPay:
                        return Redirect(GeneralData.WebsiteUrl + "/Cart/ConfirmAndPayment?id="
                            + payment_id
                            + "&bank=2" + "&useCustomRedirect=true"
                            + "&user_id=" + user.Id
                            + "&customRedirectUrl=" + redirectUrl);
                    default:
                        return Redirect(redirectUrl);
                }
            }
            catch (Exception exc)
            {
                logger.Error("", exc);
                return null;
            }
        }

        public JsonResult GuestPayReserveWithCredit(string cid, long reserve_id, int pay_reserve_type,
            bool useCoupon = false, bool usePrize = false)
        {
            if (!ClientAuthenticate(cid))
            {
                return null;
            }
            try
            {
                var user = GetUser();
                if (user.Id < 1)
                {
                    return GenerateJsonResult(new
                    {
                        done = false,
                        result = 3
                    });
                }
                long payment_id;
                var result = accounting.GuestPayReserveWithCredit(user.Id, reserve_id, pay_reserve_type, out payment_id, user.Id, ActionLog.ActionSourceEnum.Application, useCoupon, usePrize, 0);
                switch (result)
                {
                    case GuestPayResult.NotEnoughCredit:
                        return GenerateJsonResult(new
                        {
                            done = false,
                            result = 0
                        });
                    case GuestPayResult.Paid:
                        return GenerateJsonResult(new
                        {
                            done = true,
                            result = 1
                        });
                    case GuestPayResult.AlreadyPaid:
                        return GenerateJsonResult(new
                        {
                            done = true,
                            result = 2
                        });
                    default:
                        return GenerateJsonResult(new
                        {
                            done = false,
                            result = 3
                        });
                }
            }
            catch (Exception exc)
            {
                logger.Error("", exc);
                return GenerateJsonResult(new {
                    done = false,
                    result = 3
                });
            }
        }

        public JsonResult GuestCashPay(string cid, long reserve_id)
        {
            if (!ClientAuthenticate(cid))
            {
                return null;
            }
            try
            {
                var user = GetUser();
                if (user.Id < 1)
                {
                    return GenerateJsonResult(new
                    {
                        done = false,
                        msg = "ابتدا با حساب کاربری خود وارد شوید"
                    });
                }
                string msg;
                var done = reserveService.CashPay(reserve_id, out msg,
                    user.Id, ActionLog.ActionSourceEnum.Application ,user.Id);
                return GenerateJsonResult(new
                {
                    done = done,
                    msg = msg
                });
            }
            catch (Exception exc)
            {
                logger.Error("", exc);
                return GenerateJsonResult(new
                {
                    done = false,
                    msg = "متاسفانه خطایی رخ داده است. دوباره امتحان کنید"
                });
            }
        }

        [Authorize(AuthenticationSchemes = bearerScheme)]
        public JsonResult HostConfirmCashPay(string cid, long reserve_id, bool paid)
        {
            if (!ClientAuthenticate(cid))
            {
                return null;
            }
            try
            {
                var user = GetUser();
                string msg;
                var done = reserveService.ConfirmCashPay(reserve_id, paid, out msg,
                    user.Id, ActionLog.ActionSourceEnum.Application, user.Id);
                return GenerateJsonResult(new
                {
                    done = done,
                    msg = msg
                });
            }
            catch (Exception exc)
            {
                logger.Error("", exc);
                return GenerateJsonResult(new
                {
                    done = false,
                    msg = "متاسفانه خطایی رخ داده است. دوباره امتحان کنید"
                });
            }
        }
    }
}

