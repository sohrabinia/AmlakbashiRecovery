using Amlakbashi.Core.Common.Extensions;
using Amlakbashi.Core.Common.Repository;
using Amlakbashi.Core.Entities;
using Amlakbashi.Core.Identity.Entities;
using Amlakbashi.Core.Infrastructure.UserContact;
using Amlakbashi.Core.Infrastructure.UserContact.Interfaces;
using Amlakbashi.Mediator.Commands.AdvertiseCommands;
using Amlakbashi.Mediator.Commands.UserCommands;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Amlakbashi.Application.Services.ReserveServices.CommandHandlers
{
    public class ReserveSendSmsCommandHandler : IRequestHandler<RefreshReserveSendSmsCommand>,
        IRequestHandler<ScheduleReserveSendSmsCommand>
    {
        private readonly IRepository<ReserveSendSms, long> repository;
        private readonly IUserContactFacade userContact;
        private readonly UserManager<AppUser> userManager;
        private readonly IMediator mediator;
        public ReserveSendSmsCommandHandler(IRepository<ReserveSendSms, long> repository,
            IUserContactFacade userContact, UserManager<AppUser> userManager,
            IMediator mediator)
        {
            this.repository = repository;
            this.userContact = userContact;
            this.userManager = userManager;
            this.mediator = mediator;
        }

        public Task<Unit> Handle(RefreshReserveSendSmsCommand request, CancellationToken cancellationToken)
        {
            var now = DateTime.Now;
            IQueryable<ReserveSendSms> queue = repository.Query(q => q.Where(w => w.ScheduledTime <= now));
            foreach (var item in queue)
            {
                var user = repository.Find<User, int>(item.userId);
                var identityUser = userManager.FindByNameAsync(user.PhoneNumber).Result;
                if (identityUser == null)
                {
                    return Task.FromResult(Unit.Value);
                }
                var reserve = !string.IsNullOrEmpty(item.reserve_id) ?
                    repository.Find<Reserve, long>(long.Parse(item.reserve_id)) :
                    null;
                mediator.Enqueue(new SendMessageClassicCommand(item.initial, new UserContactDTO()
                {
                    UserMainMobile = user.PhoneNumber,
                    UserAppNotificationToken = user.AppNotificationToken,
                    UserEmail = identityUser.Email,
                    EmailConfirmed = identityUser.EmailConfirmed,
                    UserFcmAppNotificationToken = user.FcmAppNotificationToken,
                    UserNotificationToken = user.NotificationToken,
                    ReserveStatus = reserve == null ? Reserve.ReserveStatus.Default :
                        reserve.Status,
                    Type = (UserContactType)item.type,
                    AdvertiseId = item.advertise_id,
                    UserId = item.user_id,
                    ReserveId = item.reserve_id,
                    TransactionId = item.transaction_id,
                    AudienceMobile = item.audience_mobile,
                    Price = item.price,
                    RemainPrice = item.remain_price,
                    DoerTitle = item.doer_title,
                    CauseString = item.cause_string,
                    Code = item.code,
                    Extra1 = item.extra_1,
                    Extra2 = item.extra_2,
                    Extra3 = item.extra_3
                }));
            }
            repository.Delete(q => q.ScheduledTime <= now);
            repository.Save();
            return Task.FromResult(Unit.Value);
        }

        public Task<Unit> Handle(ScheduleReserveSendSmsCommand request, CancellationToken cancellationToken)
        {
            repository.Insert(request.Data);
            repository.Save();
            return Task.FromResult(Unit.Value);
        }
    }
}
