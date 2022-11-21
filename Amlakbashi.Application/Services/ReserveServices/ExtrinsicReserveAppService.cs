using Amlakbashi.Application.Services.ReserveServices.Interfaces;
using Amlakbashi.Core.Common.AppService;
using Amlakbashi.Core.Common.Repository;
using Amlakbashi.Core.Common.Utilities;
using Amlakbashi.Core.Entities;
using Amlakbashi.Mediator.Commands.AdvertiseCommands;
using MediatR;
using System;
using static Amlakbashi.Core.Entities.ActionLog;

namespace Amlakbashi.Application.Services.ReserveServices
{
    public class ExtrinsicReserveAppService : BaseAppService<ExtrinsicReserve, long>, IExtrinsicReserveAppService
    {
        private readonly IMediator mediator;
        public ExtrinsicReserveAppService(IRepository<ExtrinsicReserve, long> repository,
            IMediator mediator) : base(repository)
        {
            this.mediator = mediator;
        }

        public void Insert(long advertiseId, DateTime date,
            ActionSourceEnum actionSource, int doerUserID, int count = 1)
        {
            mediator.Send(new InsertExtrinsicReserveCommand(advertiseId,
                DateTimeUtility.GregorianToPersianDate(date),
                 DateTimeUtility.GregorianToPersianDate(date.AddDays(1)),
                 actionSource, doerUserID, count));
        }

        public void Insert(long advertiseId, string from_date,
            string to_date, ActionSourceEnum actionSource, int doerUserId, int count = 1)
        {
            mediator.Send(new InsertExtrinsicReserveCommand(advertiseId,
                from_date, to_date, actionSource, doerUserId, count));
        }
    }
}
