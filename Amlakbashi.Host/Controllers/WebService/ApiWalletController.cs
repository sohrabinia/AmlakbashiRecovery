using Amlakbashi.Accounting;
using Amlakbashi.Application.Services.UserServices.Interfaces;
using Amlakbashi.Core.DTOs.WebService.Responses.Wallets;
using Amlakbashi.Core.Entities;
using Amlakbashi.Host.Controllers.Base;
using Amlakbashi.Host.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Amlakbashi.Host.Controllers.WebService
{
    [ApiController]
    [Route("api/wallet")]
    public class ApiWalletController : ApiBaseController
    {
        private readonly IUserAppService userService;
        private readonly IAccountingFacade accounting;
        public ApiWalletController(IUserAppService userService,
            IAccountingFacade accounting)
        {
            this.userService = userService;
            this.accounting = accounting;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        public WalletTransactionListResponse Get(int page = 1, int pageItemCount = 20)
        {
            var walletTransactionPagedList = accounting.GetUserWalletTransactions(User.GetId(), page, pageItemCount);
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
            var user = userService.Find(User.GetId());
            return Ok(new { walletAmount = user.WalletAmount });
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
                UserID = User.GetId(),
                CreateDate = DateTime.Now,
                Amount = price * 10,
                ProductType = CreditTransaction.WalletTransactionTypeForPayment.Credit_Increase.ToString()
            };
            accounting.InsertPayment(payment);
            return Ok(new { paymentId = payment.Id });
        }
    }
}
