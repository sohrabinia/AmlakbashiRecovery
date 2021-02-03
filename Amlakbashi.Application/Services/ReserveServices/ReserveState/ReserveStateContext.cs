using Amlakbashi.Application.Services.ReserveServices.ReserveState.Interfaces;
using Amlakbashi.Core.Common.Extensions;
using Amlakbashi.Core.Common.Repository;
using Amlakbashi.Core.Common.State;
using Amlakbashi.Core.Entities;
using Amlakbashi.Mediator.Commands.AdvertiseCommands;
using Amlakbashi.Mediator.Events.ReserveEvents;
using Autofac.Features.Indexed;
using MediatR;
using System;
using System.Linq;
using static Amlakbashi.Core.Entities.Reserve;

namespace Amlakbashi.Application.Services.ReserveServices.ReserveState
{
    public class ReserveStateContext : StateContextBase<ReserveStatus>, IReserveStateContext
    {
        private readonly IIndex<ReserveStatus, IReserveState> states;
        private readonly IRepository<Reserve, long> Repository;
        private readonly IMediator mediator;
        private ReserveStatus currentReserveStatus;
        private IReserveState reserveState;
        private long reserveId;
        public ReserveStateContext(IIndex<ReserveStatus, IReserveState> states,
            IMediator mediator,
            IRepository<Reserve, long> Repository) :
            base(ReserveStatus.Default, null)
        {
            this.states = states;
            this.Repository = Repository;
            this.mediator = mediator;
        }

        protected override StateBase<ReserveStatus> state
        {
            get
            {
                return reserveState as StateBase<ReserveStatus>;
            }
            set
            {
                reserveState = value as IReserveState;
            }
        }

        public IReserveStateContext UseReserve(long reserveId)
        {
            var reserve = Repository.Query(q => q.FirstOrDefault(f => f.Id == reserveId));
            if (reserve.Status == ReserveStatus.Default)
                throw new Exception("Reserve State Cannot Be Default");
            if (reserve.Status == ReserveStatus.Deleted)
                throw new Exception("Deleted Reserve Not Supported In State Context");
            TransitionTo(states[reserve.Status] as StateBase<ReserveStatus>);
            reserveState.Initialize(reserveId);
            currentReserveStatus = reserve.Status;
            this.reserveId = reserveId;
            return this;
        }

        public bool SetStatus(ReserveStatus status, bool sendSms,
            ActionLog.ActionSourceEnum actionSource, int doerUserId, bool force = false)
        {
            if (reserveState.Initialized == false)
                throw new Exception("Reserve State Has Not Been Initialized");
            if (status == ReserveStatus.Default)
                throw new Exception("Reserve State Cannot Be Default");
            if (status == ReserveStatus.Deleted)
                throw new Exception("Cannot Set To Deleted Through State Context, Please Set Status To Deleted Directly");
            if (currentReserveStatus == status)
            {
                return true;
            }
            var targetState = states[status];
            targetState.Initialize(reserveId);
            if (force == false && reserveState.CanTransitTo(status) == false)
            {
                return false;
            }
            var prevStatus = currentReserveStatus;
            TransitionTo(targetState as StateBase<ReserveStatus>);
            currentReserveStatus = status;
            reserveState.OnTransition(prevStatus, sendSms, actionSource, doerUserId);
            var reserve = Repository.Find(reserveId);
            if (StatusIsReserving(prevStatus) != StatusIsReserving(currentReserveStatus))
            {
                mediator.Send(new UpdateAdvertiseOccupiedCommand(reserve.AdvertiseID));
            }
            mediator.Publish(new ChangeReserveStateEvent(reserveId, currentReserveStatus, reserve.HostResponse));
            return true;
        }
    }
}
