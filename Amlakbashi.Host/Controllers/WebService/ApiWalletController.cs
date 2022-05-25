using Amlakbashi.Accounting;
using Amlakbashi.Application.Services.UserServices.Interfaces;
using Amlakbashi.Core.DTOs.WebService.Responses.Wallets;
using Amlakbashi.Core.Entities;
using Amlakbashi.Host.Authentication;
using Amlakbashi.Host.Controllers.Base;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Amlakbashi.Host.Controllers.WebService
{
    [ApiController]
    [Route("api/wallet")]
    public class ApiWalletController : ApiBaseController
    {
        private readonly IUserAppService userService;
        private readonly IAccountingFacade accounting;
        private readonly IUserAccessor userAccessor;
        public ApiWalletController(IUserAppService userService,
            IAccountingFacade accounting,
            IUserAccessor userAccessor)
        {
            this.userService = userService;
            this.accounting = accounting;
            this.userAccessor = userAccessor;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        public WalletTransactionListResponse Get(int page = 1, int pageItemCount = 20)
        {
            var walletTransactionPagedList = accounting.GetUserWalletTransactions(userAccessor.CurrentUser.Id, page, pageItemCount);
            return new WalletTransactionListResponse() {
                pagingInfo = walletTransactionPagedList.PagingInfo,
                transactionList = walletTransactionPagedList.List.Select(x=> new WalletTransactionListItemResponse()
                {
                    id = x.Id,
                    date = x.Date.ToString(),
                    price = x.Price,
                    traceNumber = x.BankTransactionID.ToString(),
                    description = x.TransactionCauseString
                }).ToList()
            };
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("amount")]
        public IActionResult GetWalletAmount()
        {
            return Ok(new { walletAmount = userAccessor.CurrentUser.WalletAmount });
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost]
        public IActionResult IncreaseWalletAmount(int price)
        {
            if (price < 1000)
            {
                return BadRequest();
            }
            var payment = new Payment()
            {
                UserID = userAccessor.CurrentUser.Id,
                Date = DateTime.Now,
                TotalPrice = price * 10,
                ProductType = CreditTransaction.WalletTransactionTypeForPayment.Credit_Increase.ToString()
            };
            accounting.InsertPayment(payment);
            return Ok(new { paymentId = payment.Id });
        }
    }
}
