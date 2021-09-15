using Amlakbashi.Accounting;
using Amlakbashi.Application.Services.UserServices.Interfaces;
using Amlakbashi.Core.DTOs.WalletDTOs;
using Amlakbashi.Core.Entities;
using Amlakbashi.Core.Identity;
using Amlakbashi.Core.Infrastructure.UserContact;
using Amlakbashi.Host.Authentication;
using Amlakbashi.Host.Controllers.Base;
using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using X.PagedList;
using Entities = Amlakbashi.Core.Entities;

namespace Amlakbashi.Host.Controllers
{
    public class WalletController : BaseController
    {
        private readonly ILog logger;
        private readonly IAccountingFacade accounting;
        private readonly IUserAccessor userAccessor;
        private readonly IUserAppService userService;
        public WalletController(ILog logger,
            IAccountingFacade accounting,
            IUserAccessor userAccessor,
            IUserAppService userService)
        {
            this.logger = logger;
            this.accounting = accounting;
            this.userAccessor = userAccessor;
            this.userService = userService;
        }

        [Authorize(Policy = Policies.Payment_View)]
        public IActionResult Index(CreditTransactionIndexDTO dto)
        {
            try
            {
                accounting.FilterCreditTransactions(dto);
                dto.CreditTransactionList = dto.CreditTransactionList.ToPagedList(dto.page, dto.pageModelCount);
                return View(dto);
            }
            catch (Exception exc)
            {
                logger.Error("Wallet.GetUserWalletInfo", exc);
                return Redirect("/errors/Http500");
            }
        }

        [Authorize(Policy = Policies.Payment_Actions)]
        [HttpGet]
        public IActionResult EditCreditTransaction(long id)
        {
            try
            {
                var creditTransaction = accounting.FindCreditTransaction(id);
                return View(creditTransaction);
            }
            catch (Exception exc)
            {
                logger.Error("Wallet.EditCreditTransaction(get)", exc);
                return Redirect("/errors/Http500");
            }
        }

        [Authorize(Policy = Policies.Payment_Actions)]
        [HttpPost]
        public IActionResult EditCreditTransaction(CreditTransaction editedCreditTransaction)
        {
            try
            {
                var hasError = false;
                if (editedCreditTransaction.Price == 0)
                {
                    ViewBag.error = "مبلغ نمی تواند صفر باشد";
                    hasError = true;
                }
                if (editedCreditTransaction.TransactionCause == CreditTransaction.WalletTransactionReason.Other &&
                    string.IsNullOrEmpty(editedCreditTransaction.TransactionCauseString))
                {
                    ViewBag.error = "لطفا نوع تراکنش را مشخص کنید";
                    hasError = true;
                }

                if (hasError)
                {
                    return View(accounting.FindCreditTransaction(editedCreditTransaction.Id));
                }

                var creditTransaction = accounting.EditCreditTransaction(editedCreditTransaction, userAccessor.CurrentUser.Id);
                ViewBag.success = true;
                return View(creditTransaction);
            }
            catch (Exception exc)
            {
                logger.Error("Wallet.EditCreditTransaction(post)", exc);
                return Redirect("/errors/Http500");
            }
        }

        [Authorize(Policy = Policies.Payment_Actions)]
        public IActionResult GetUserWalletInfo(int userId)
        {
            try
            {
                var user = userService.Find(userId);
                var bankCard = user.BankCards.FirstOrDefault();
                var bankCardName = bankCard != null ?
                    ((bankCard.FName != null ? bankCard.FName + " " : "") +
                    (bankCard.LName != null ? bankCard.LName : "")) : "";
                var hostName = string.IsNullOrEmpty(user.FullName) ?
                    user.GetPhoneNumber(Entities.User.PhoneType.MainMobile) : user.FullName;

                var model = new UserWalletInfoDTO()
                {
                    UserId = user.Id,
                    UserName = hostName,
                    BankCardId = bankCard != null ? bankCard.Id : 0,
                    BankCardName = bankCardName,
                    BankCardNumber = bankCard != null ? bankCard.BankCardNumber : "",
                    BankCardVerified = bankCard != null &&
                            bankCard.BankCardStatus == (int)BankCard.BankCardStatusEnum.Verified,
                    ShebaNumber = bankCard != null ? bankCard.ShabaNumber : "",
                    ShebaVerified = bankCard != null &&
                            bankCard.ShabaStatus == (int)BankCard.BankCardStatusEnum.Verified,
                    WalletCredit = user.Credit
                };

                return PartialView("_UserWalletInfo", model);
            }
            catch (Exception exc)
            {
                logger.Error("Wallet.GetUserWalletInfo", exc);
                return PartialView();
            }
        }

        [Authorize(Policy = Policies.Payment_Actions)]
        [HttpGet]
        public IActionResult AutoClearingWallet(int userId)
        {
            try
            {
                var user = userService.Find(userId);
                var bankCard = user.BankCards.FirstOrDefault();
                var bankCardName = bankCard != null ?
                    ((bankCard.FName != null ? bankCard.FName + " " : "") +
                    (bankCard.LName != null ? bankCard.LName : "")) : "";

                if (user.Credit <= 0)
                {
                    ViewBag.error = true;
                    ViewBag.errorMsg = "موجودی کیف پول کمتر از حد مجاز است";
                    return PartialView("_AutoClearingWallet");
                }
                if (bankCard == null || bankCard.ShabaStatus != (int)BankCard.BankCardStatusEnum.Verified)
                {
                    ViewBag.error = true;
                    ViewBag.errorMsg = "شماره شبا تایید نشده است";
                    return PartialView("_AutoClearingWallet");
                }

                ViewBag.userId = userId;
                ViewBag.shebaNumber = bankCard.ShabaNumber;
                ViewBag.bankCardName = bankCardName;
                ViewBag.price = user.Credit * 10;
                return PartialView("_AutoClearingWallet");
            }
            catch (Exception exc)
            {
                logger.Error("Wallet.AutoClearingWallet(get)", exc);
                ViewBag.error = true;
                ViewBag.errorMsg = "عملیات با خطا مواجه شد";
                return PartialView();
            }
        }

        [Authorize(Policy = Policies.Payment_Actions)]
        [HttpPost]
        public IActionResult AutoClearingWallet(int userId, bool sendSms)
        {
            try
            {
                var result = accounting.WalletClearingAutoPayment(userId, userAccessor.CurrentUser.Id);
                if (result.HasError == false && sendSms)
                {
                    var user = userService.Find(userId);
                    var identityUser = userService.GetIdentityUser(user.MainMobile);
                    userService.SendMessage(new UserContactDTO()
                    {
                        UserMainMobile = user.MainMobile,
                        UserAppNotificationToken = user.AppNotificationToken,
                        UserEmail = identityUser.Email,
                        UserFcmAppNotificationToken = user.FcmAppNotificationToken,
                        UserNotificationToken = user.NotificationToken,
                        Type = UserContactType.UserCreditDecrease,
                        TransactionId = result.TraceNumber.ToString(),
                        Price = result.PayablePrice.ToString(),
                        CauseString = CreditTransaction.GetCreditTransactionCauseString(
                            CreditTransaction.WalletTransactionReason.Other, "تسویه کیف پول")
                    });
                }
                return GenerateJsonResult(new
                {
                    status = result.HasError ? 0 : 1,
                    msg = result.HasError ? result.ErrorMessage : result.Message
                });
            }
            catch (Exception exc)
            {
                logger.Error("Wallet.AutoClearingWallet(post)", exc);
                return GenerateJsonResult(new
                {
                    status = 0,
                    msg = "عملیات با خطای فنی مواجه شد"
                });
            }
        }
    }
}
