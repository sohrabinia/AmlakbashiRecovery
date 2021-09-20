using System;
using Amlakbashi.Application.Services.UserServices.Interfaces;
using log4net;
using Amlakbashi.Host.Controllers.Base;
using Amlakbashi.Host.Authentication;
using Microsoft.AspNetCore.Mvc;
using X.PagedList;
using Microsoft.AspNetCore.Authorization;
using Amlakbashi.Core.Identity;
using Amlakbashi.Core.Common.Utilities;
using Amlakbashi.Core.Entities;
using Amlakbashi.Core.DTOs.BankCardDTOs;

namespace Amlakbashi.Host.Controllers
{
    public class BankCardController : BaseController
    {
        private readonly IBankCardAppService bankCardService;
        private readonly IUserAppService userService;
        private readonly IUserAccessor userAccessor;
        private readonly ILog logger;
        public BankCardController(IBankCardAppService bankCardService,
            IUserAppService userService,
            IUserAccessor userAccessor,
            ILog logger)
        {
            this.bankCardService = bankCardService;
            this.userService = userService;
            this.userAccessor = userAccessor;
            this.logger = logger;
        }

        [Authorize(Policy = Policies.User_Bank_Info)]
        public ActionResult Index(int? page, int user_id = -1, string bank_card_number = null,
            string shaba_number = null, int bank_card_status = -1, int shaba_status = -1)
        {
            try
            {
                var model = bankCardService.Filter(user_id, bank_card_number, shaba_number, bank_card_status, shaba_status);
                var PageNumber = page ?? 1;
                var onePageOfModel = model.ToPagedList(PageNumber, 10);

                ViewBag.user_id = user_id;
                ViewBag.bank_card_number = bank_card_number;
                ViewBag.shaba_number = shaba_number;
                ViewBag.bank_card_status = bank_card_status;
                ViewBag.shaba_status = shaba_status;

                ViewBag.RowIndexStart = (PageNumber * 10) - 10;
                return View(onePageOfModel);
            }
            catch (Exception exc)
            {
                logger.Error("BankCard.Index", exc);
                return Redirect(Request.Headers["Referer"].ToString());
            }
        }

        [Authorize(Policy = Policies.User_Bank_Info)]
        public JsonResult ToggleBankCardStatus(int id)
        {
            try
            {
                var bankCardStatus = bankCardService.ToggleBankCardStatus(id);
                return GenerateJsonResult(new { status = 1, new_status = bankCardStatus });
            }
            catch (Exception exc)
            {
                logger.Error("BankCard.ToggleBankCardStatus", exc);
                return GenerateJsonResult(new { status = 0, val = "" });
            }
        }

        [Authorize(Policy = Policies.User_Bank_Info)]
        public JsonResult ToggleShabaStatus(int id)
        {
            try
            {
                var shabaStatus = bankCardService.ToggleShabaStatus(id);
                return GenerateJsonResult(new { status = 1, new_status = shabaStatus });
            }
            catch (Exception exc)
            {
                logger.Error("BankCard.ToggleShabaStatus", exc);
                return GenerateJsonResult(new { status = 0 });
            }
        }

        [Authorize(Policy = Policies.User_Bank_Info)]
        public JsonResult SetBankCardName(int bankCardId, string bankCardFName, string bankCardLName)
        {
            try
            {
                var bankCard = bankCardService.Find(bankCardId);
                bankCard.FName = bankCardFName;
                bankCard.LName = bankCardLName;
                bankCardService.Update(bankCard, userAccessor.CurrentUser.Id, ActionLog.ActionSourceEnum.AdminPanel);
                return GenerateJsonResult(new { status = 1 });
            }
            catch (Exception exc)
            {
                logger.Error("BankCard.SetBankCardName", exc);
                return GenerateJsonResult(new { status = 0 });
            }
        }

        [Authorize(Policy = Policies.User_Bank_Info)]
        public IActionResult SetUserBankCard(int userId)
        {
            try
            {
                var bankCard = bankCardService.GetByUserId(userId);
                var dto = new SetBankCardDTO()
                {
                    UserId = userId
                };
                if (bankCard != null)
                {
                    dto.FName = bankCard.FName;
                    dto.LName = bankCard.LName;
                    dto.BankCardNumber = bankCard.BankCardNumber;
                    dto.ShebaNumber = bankCard.ShabaNumber;
                    dto.VerifyBankCardNumber = bankCard.BankCardStatus == 0 ? true : false;
                    dto.VerifyShebaNumber = bankCard.ShabaStatus == 0 ? true : false;
                }
                return PartialView("_SetUserCardBank", dto);
            }
            catch (Exception exc)
            {
                logger.Error("BankCard.SetUserCardBank", exc);
                ViewBag.error = "عملیات با خطای فنی مواجه شد";
                return PartialView("_SetUserCardBank");
            }
        }

        [Authorize(Policy = Policies.User_Bank_Info)]
        [HttpPost]
        public IActionResult SetUserBankCard(SetBankCardDTO newBankCard)
        {
            try
            {
                var user = userService.Find(newBankCard.UserId);
                var bankCard = bankCardService.GetByUserId(newBankCard.UserId);

                if (string.IsNullOrEmpty(newBankCard.BankCardNumber) == false &&
                    BankUtility.ValidateBankCardNumber(newBankCard.BankCardNumber) == false)
                {
                    return GenerateJsonResult(new
                    {
                        status = 0,
                        msg = "شماره کارت وارد شده صحیح نمی باشد"
                    });
                }

                if (string.IsNullOrEmpty(newBankCard.ShebaNumber) == false &&
                    newBankCard.ShebaNumber.Length != 24)
                {
                    return GenerateJsonResult(new
                    {
                        status = 0,
                        msg = "شماره شبا وارد شده صحیح نمی باشد"
                    });
                }

                if (bankCard != null)
                {
                    bankCard.BankCardNumber = newBankCard.BankCardNumber;
                    bankCard.FName = newBankCard.FName;
                    bankCard.LName = newBankCard.LName;
                    bankCard.ShabaNumber = newBankCard.ShebaNumber;
                    bankCard.BankCardStatus = string.IsNullOrEmpty(newBankCard.BankCardNumber) == false &&
                        newBankCard.VerifyBankCardNumber ? (int)BankCard.BankCardStatusEnum.Verified : 
                        (int)BankCard.BankCardStatusEnum.NotVerified;
                    bankCard.ShabaStatus = string.IsNullOrEmpty(newBankCard.ShebaNumber) == false &&
                        newBankCard.VerifyShebaNumber ? (int)BankCard.BankCardStatusEnum.Verified : 
                        (int)BankCard.BankCardStatusEnum.NotVerified;
                    bankCard.LastModifyDate = DateTime.Now;
                    bankCardService.Update(bankCard, userAccessor.CurrentUser.Id, ActionLog.ActionSourceEnum.AdminPanel);
                }
                else
                {
                    bankCard = new BankCard()
                    {
                        BankCardNumber = newBankCard.BankCardNumber,
                        FName = newBankCard.FName,
                        LName = newBankCard.LName,
                        BankCardStatus = string.IsNullOrEmpty(newBankCard.BankCardNumber) == false &&
                            newBankCard.VerifyBankCardNumber ? (int)BankCard.BankCardStatusEnum.Verified :
                            (int)BankCard.BankCardStatusEnum.NotVerified,
                        ShabaNumber = newBankCard.ShebaNumber,
                        ShabaStatus = string.IsNullOrEmpty(newBankCard.ShebaNumber) == false &&
                            newBankCard.VerifyShebaNumber ? (int)BankCard.BankCardStatusEnum.Verified :
                            (int)BankCard.BankCardStatusEnum.NotVerified,
                        UserID = newBankCard.UserId,
                        CreateDate = DateTime.Now,
                        LastModifyDate = DateTime.Now
                    };
                    bankCardService.Insert(bankCard, userAccessor.CurrentUser.Id, ActionLog.ActionSourceEnum.AdminPanel);
                }
                return GenerateJsonResult(new
                {
                    status = 1,
                    msg = "تغییرات با موفقیت ذخیره شد"
                });
            }
            catch (Exception exc)
            {
                logger.Error("BankCard.SetUserCardBank", exc);
                return GenerateJsonResult(new
                {
                    status = 0,
                    msg = "عملیات با خطا مواجه شد"
                });
            }
        }
    }
}
