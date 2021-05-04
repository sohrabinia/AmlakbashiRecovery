using Amlakbashi.Core.Common.Extensions;
using Amlakbashi.Core.Common.Repository;
using Amlakbashi.Core.Entities;
using Amlakbashi.Core.Infrastructure.UserContact.Interfaces;
using Amlakbashi.Mediator.Commands.AccountingCommands;
using Amlakbashi.Mediator.Commands.UserCommands;
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
        private readonly IMediator mediator;
        public AccountingCommandHandler(IUserContactFacade userContact,
            IRepository<DiscountCoupon, long> discountCouponRepository,
            IMediator mediator)
        {
            this.userContact = userContact;
            this.discountCouponRepository = discountCouponRepository;
            this.mediator = mediator;
        }

        public Task<Unit> Handle(ScheduleSendMessageGroupPaymentCommand request, CancellationToken cancellationToken)
        {
            mediator.Enqueue(new SendMessageClassicCommand(true, request.UserContactDTO));
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
