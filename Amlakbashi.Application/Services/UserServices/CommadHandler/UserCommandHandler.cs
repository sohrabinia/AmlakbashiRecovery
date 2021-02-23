using Amlakbashi.Core.Common.Extensions;
using Amlakbashi.Core.Common.Repository;
using Amlakbashi.Core.Common.Utilities;
using Amlakbashi.Core.Entities;
using Amlakbashi.Core.Infrastructure.UserContact.Interfaces;
using Amlakbashi.Mediator.Commands.AdvertiseCommands;
using Amlakbashi.Mediator.Commands.UserCommands;
using Amlakbashi.Mediator.Events.UserEvents;
using log4net;
using MediatR;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Amlakbashi.Application.Services.UserServices.CommadHandler
{
    public class UserCommandHandler : IRequestHandler<ScheduleSendCustomSms>,
        IRequestHandler<ScheduleSendGroupNotificationCommand>,
        IRequestHandler<ScheduleSendNotificationCommand>,
        IRequestHandler<ChangeInstantReserveAccessCommand>,
        IRequestHandler<UpdateUserScoreCommand>,
        IRequestHandler<SendMessageCommand>,
        IRequestHandler<SendSmsCommand>
    {
        private readonly IRepository<User, int> repository;
        private readonly IUserContactFacade userContact;
        private readonly IMediator mediator;
        private readonly ILog logger;
        public UserCommandHandler(IMediator mediator,
            IUserContactFacade userContact,
            IRepository<User, int> repository,
            ILog logger)
        {
            this.userContact = userContact;
            this.mediator = mediator;
            this.repository = repository;
            this.logger = logger;
        }

        public Task<Unit> Handle(ScheduleSendCustomSms request, CancellationToken cancellationToken)
        {
            userContact.SendTemplateSms(request.Mobile, request.Template);
            return Task.FromResult(Unit.Value);
        }

        public Task<Unit> Handle(ScheduleSendGroupNotificationCommand request, CancellationToken cancellationToken)
        {
            var delay = 1;
            foreach (var token in request.Token)
            {
                mediator.Schedule(new ScheduleSendNotificationCommand(token, request.Title, request.Body, request.ClickAction), new TimeSpan(0, 0, delay));
                delay++;
            }
            return Task.FromResult(Unit.Value);
        }

        public Task<Unit> Handle(ScheduleSendNotificationCommand request, CancellationToken cancellationToken)
        {
            userContact.SendNotification(request.Token, request.Title, request.Body, request.ClickAction);
            return Task.FromResult(Unit.Value);
        }

        public Task<Unit> Handle(ChangeInstantReserveAccessCommand request, CancellationToken cancellationToken)
        {
            var user = repository.Query(q => q.FirstOrDefault(f => f.Id == request.userId));
            var shallowUser = user.ShallowCopy();
            user.InstantReserveAccess = request.instantReserveAccess;
            repository.Update(user);
            repository.Save();
            if (request.doerUserId > 0)
            {
                mediator.Publish(new UserUpdateEvent(shallowUser, user, request.actionSource, request.doerUserId));
            }
            return Task.FromResult(Unit.Value);
        }

        public Task<Unit> Handle(UpdateUserScoreCommand request, CancellationToken cancellationToken)
        {
            try
            {
                IQueryable<User> all_user = repository.Query(q => q
                    .Include(i => i.HostReserves)
                    .Where(x => (request.UserId < 1 ? true : x.Id == request.UserId) &&
                    x.State != 3 && x.UserGeneralType > 0));
                all_user = all_user.Where(w => w.Advertises.Any());
                long score_item;
                Dictionary<int, long> result = new Dictionary<int, long>();
                foreach (var item in all_user)
                {
                    try
                    {
                        score_item = item.AmlakbashiScore;

                        //ownership score calculate
                        switch (item.OwnerShip)
                        {
                            case /*UserDepend.OwnerType.real_owner*/10:
                            case /*UserDepend.OwnerType.owner*/3:
                                score_item += 100;
                                break;
                        }

                        //end ownership score calculate

                        //by host canceled reserves score calculate

                        var reserves = item.HostReserves.OrderBy(x => x.CreateDate);

                        //var canceled_request_sequence = new List<int>();

                        int current_squence_length = 0;
                        int score_to_reduce = 0;

                        int currentSequenceMultiplier;
                        foreach (var reserve in reserves)
                        {
                            if ((int)reserve.Status == 12/*(int)Reserve.ReserveStatus.CanceledByHost*/ &&
                            reserve.CancelState > Reserve.ReserveStatus.Rejected && reserve.CancelState < Reserve.ReserveStatus.CancelRequestByGuest)
                            //(reserve.CancelState == 5/*(int)Reserve.ReserveStatus.Reserved*/ ||
                            //reserve.CancelState == 6/*(int)Reserve.ReserveStatus.CashPay*/ ||
                            //reserve.CancelState == 7/*(int)Reserve.ReserveStatus.Started*/ ||
                            //reserve.CancelState == 8/*(int)Reserve.ReserveStatus.Completed*/))
                            {
                                current_squence_length++;
                                //canceled_request_sequence.Add(reserve.CancelState == 2/*(int)Reserve.ReserveStatus.WaitForReserve*/ ? 1 : 2);
                                currentSequenceMultiplier = reserve.CancelState == Reserve.ReserveStatus.WaitForReserve ? 1 : 2;
                                score_to_reduce += currentSequenceMultiplier * MathUtility.GenerateFibonacci(50, 80, current_squence_length);
                            }
                            else
                            {
                                current_squence_length = 0;
                                //canceled_request_sequence.Add(0);
                                currentSequenceMultiplier = 0;
                            }
                            //reserved score calculate
                            if ((int)reserve.Status > 4 && (int)reserve.Status < 9)
                            {
                                score_item += 5;
                            }
                        }
                        //int current_squence_length = 0;
                        //int score_to_reduce = 0;
                        //foreach (var is_canceled in canceled_request_sequence)
                        //{
                        //    if (is_canceled > 0)
                        //    {
                        //        current_squence_length++;
                        //        score_to_reduce += is_canceled * MathUtility.GenerateFibonacci(50, 80, current_squence_length);
                        //    }
                        //    else
                        //    {
                        //        current_squence_length = 0;
                        //    }   
                        //}

                        score_item -= score_to_reduce;

                        //end by host canceled reserves score calculate

                        //reserved score calculate
                        //foreach (var reserve in reserves)
                        //{
                        //    //if (reserve.Status == (int)Reserve.ReserveStatus.Reserved ||
                        //    //    reserve.Status == (int)Reserve.ReserveStatus.CashPay ||
                        //    //    reserve.Status == (int)Reserve.ReserveStatus.Started ||
                        //    //    reserve.Status == (int)Reserve.ReserveStatus.Completed)
                        //    if (reserve.Status > 4 && reserve.Status < 9)
                        //        {
                        //            score_item += 5;
                        //        }
                        //}

                        //end reserved score calculate

                        //item.UserScore = score_item;
                        //repository.Update(item);
                        result.Add(item.Id, score_item);

                    }
                    catch
                    {
                        //PostDepend.AddError(" خطا در محاسبه ی امتیاز کاربر" + item.UserID.ToString());
                    }
                }
                var userIds = result.Select(s => s.Key).ToList();
                var usersToSave = repository.Query(q => q.Where(w => userIds.Contains(w.Id)));
                foreach (var item in usersToSave)
                {
                    var score = result[item.Id];
                    if (item.UserScore != score)
                    {
                        item.UserScore = score;
                    }
                    //repository.Update(item);
                }
                repository.Save();
            }
            catch (Exception exc)
            {
                logger.Error("UserCommandHandler.UpdateUserScoreCommand", exc);
            }
            return Task.FromResult(Unit.Value);
        }

        public Task<Unit> Handle(SendMessageCommand request, CancellationToken cancellationToken)
        {
            userContact.SendMessage(request.UserContact);
            return Task.FromResult(Unit.Value);
        }

        public Task<Unit> Handle(SendSmsCommand request, CancellationToken cancellationToken)
        {
            userContact.SendMessageClassic(true, request.UserContact);
            return Task.FromResult(Unit.Value);
        }
    }
}
