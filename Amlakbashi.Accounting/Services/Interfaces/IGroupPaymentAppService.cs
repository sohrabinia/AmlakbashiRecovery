using Amlakbashi.Core.Common.AppService;
using Amlakbashi.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Accounting.Services.Interfaces
{
    internal interface IGroupPaymentAppService : IAppService<GroupPayment, int>
    {
        IList<GroupPayment> Filter(int status);
        GroupPayment Find(int id);
        void Insert(GroupPayment newGroupPayment);
        void UpdateDownloadCount(int id, int downloadCount);
        void UpdateStatus(int id, GroupPayment.PaymentStatus status);
        bool ExistReserveId(long reserveId, GroupPayment.PaymentStatus status);
    }
}
