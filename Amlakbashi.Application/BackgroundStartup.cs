using Amlakbashi.Core.Common.Extensions;
using Amlakbashi.Mediator.Commands.AdvertiseCommands;
using Amlakbashi.Mediator.Commands.ReserveCommands;
using Hangfire;
using MediatR;
using Microsoft.Win32.SafeHandles;
using System;
using System.Runtime.InteropServices;

namespace Amlakbashi.Application
{
    public class BackgroundStartup : IDisposable
    {        
        bool disposed = false;
        SafeHandle handle = new SafeFileHandle(IntPtr.Zero, true);
        private readonly IMediator mediator;
        public BackgroundStartup(IMediator mediator)
        {
            this.mediator = mediator;
        }

        public void Startup()
        {
            RecurringJob.RemoveIfExists("UpdateAdvertiseScore");
            RecurringJob.RemoveIfExists("UpdateAdvertiseScore2");
            RecurringJob.RemoveIfExists("UpdateAdvertiseScore3");
            RecurringJob.RemoveIfExists("RefreshEveryOneMinute");
            RecurringJob.RemoveIfExists("RefreshEveryTwentyMinutes");

            mediator.AddOrUpdate("UpdateUserScore", new UpdateUserScoreCommand(0), "40 8 * * *");
            mediator.AddOrUpdate("UpdateReserveSupportExpiration", new UpdateReserveSupportExpirationCommand(), "0 23 * * *");
            mediator.AddOrUpdate("UnsetAllTodayIsEmptyRecords", new UpdateTodayIsEmptyRecordsCommand(), "0 4 * * *");
            mediator.AddOrUpdate("UpdateAllArchives", new UpdateReserveArchivesCommand(), "0 5 * * *");
            mediator.AddOrUpdate("RemoveOldExtrinsicReserve", new RemoveOldExtrinsicReserveCommand(), "0 3 * * *");

            mediator.AddOrUpdate("RefreshReserveAutoCancels", new RefreshReserveAutoCancelCommand(), Cron.MinuteInterval(5));
            mediator.AddOrUpdate("RefreshSendSms", new RefreshReserveSendSmsCommand(), Cron.MinuteInterval(1));
            mediator.AddOrUpdate("RefreshInstantReserveAutoCancel", new RefreshInstantReserveAutoCancelCommand(), Cron.MinuteInterval(1));
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;
            if (disposing)
            {
                handle.Dispose();
            }
            disposed = true;
        }
    }
}
