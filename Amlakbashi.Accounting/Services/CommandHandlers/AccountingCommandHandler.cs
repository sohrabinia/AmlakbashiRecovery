using Amlakbashi.Core.Common.Repository;
using Amlakbashi.Core.Entities;
using Amlakbashi.Core.Infrastructure.UserContact.Interfaces;
using Amlakbashi.Mediator.Commands.AccountingCommands;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Amlakbashi.Accounting.Services.CommandHandlers
{
    public class AccountingCommandHandler : IRequestHandler<ScheduleSendMessageGroupPaymentCommand>,
        IRequestHandler<AddDiscountCouponCommand>
    {
        private readonly IUserContactFacade userContact;
        private readonly IRepository<DiscountCoupon, long> discountCouponRepository;
        public AccountingCommandHandler(IUserContactFacade userContact,
            IRepository<DiscountCoupon, long> discountCouponRepository)
        {
            this.userContact = userContact;
            this.discountCouponRepository = discountCouponRepository;
        }

        public Task<Unit> Handle(ScheduleSendMessageGroupPaymentCommand request, CancellationToken cancellationToken)
        {
            userContact.SendMessageClassic(true, request.UserContactDTO);
            return Task.FromResult(Unit.Value);
        }

        public Task<Unit> Handle(AddDiscountCouponCommand request, CancellationToken cancellationToken)
        {
            var obj = new DiscountCoupon()
            {
                UserID = request.UserId,
                CreateTime = DateTime.Now,
                Type = request.Type,
                Status = DiscountCoupon.StatusEnum.NotUsed,
                Percent = request.Percent,
                PresentorUserID = request.PresentorUserId
            };
            discountCouponRepository.Insert(obj);
            discountCouponRepository.Save();
            return Task.FromResult(Unit.Value);
        }
    }
}
