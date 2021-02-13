using System;
using Amlakbashi.Application.Services.UserServices.Interfaces;
using log4net;
using AutoMapper;
using Amlakbashi.Host.Controllers.Base;
using Amlakbashi.Host.Authentication;
using Microsoft.AspNetCore.Mvc;
using X.PagedList;

namespace Amlakbashi.Host.Controllers
{
    public class BankCardController : BaseController
    {
        private readonly IBankCardAppService bankCardService;
        private readonly ILog logger;
        private readonly IMapper mapper;
        public BankCardController(IBankCardAppService bankCardService, ILog logger, IMapper mapper)
        {
            this.bankCardService = bankCardService;
            this.logger = logger;
            this.mapper = mapper;
        }

        [Auth(UserRoles.Admin)]
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
                logger.Error("Index", exc);
                return Redirect(Request.Headers["Referer"].ToString());
            }
        }

        [Auth(UserRoles.Admin)]
        public JsonResult ToggleBankCardStatus(int id)
        {
            try
            {
                var bankCardStatus = bankCardService.ToggleBankCardStatus(id);
                return GenerateJsonResult(new { status = 1, new_status = bankCardStatus });
            }
            catch (Exception exc)
            {
                logger.Error("ToggleBankCardStatus", exc);
                return GenerateJsonResult(new { status = 0, val = "" });
            }
        }

        [Auth(UserRoles.Admin)]
        public JsonResult ToggleShabaStatus(int id)
        {
            try
            {
                var shabaStatus = bankCardService.ToggleShabaStatus(id);
                return GenerateJsonResult(new { status = 1, new_status = shabaStatus });
            }
            catch (Exception exc)
            {
                logger.Error("ToggleShabaStatus", exc);
                return GenerateJsonResult(new { status = 0 });
            }
        }
    }
}
