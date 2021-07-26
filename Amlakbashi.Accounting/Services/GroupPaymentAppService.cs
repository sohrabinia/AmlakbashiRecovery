using Amlakbashi.Accounting.Services.Interfaces;
using Amlakbashi.Core.Common.AppService;
using Amlakbashi.Core.Common.Repository;
using Amlakbashi.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Accounting.Services
{
    internal class GroupPaymentAppService : AppServiceBase<GroupPayment, int>, IGroupPaymentAppService
    {
        public GroupPaymentAppService(IRepository<GroupPayment, int> repository) : base(repository)
        {
        }

        public IList<GroupPayment> Filter(int status)
        {
            var model = Repository.Query(q=>q);
            if (status != -1)
                model = model.Where(c => c.StatusInt == status);
            return model.OrderByDescending(u => u.CreateDate).ToList();
        }

        public GroupPayment Find(int id)
        {
            return Repository.Find(id);
        }

        public void Insert(GroupPayment newGroupPayment)
        {
            Repository.Insert(newGroupPayment);
            Repository.Save();
        }

        public void UpdateDownloadCount(int id, int downloadCount)
        {
            var data = Repository.Find(id);
            data.DownloadCount = downloadCount;
            Repository.Update(data);
            Repository.Save();
        }

        public void UpdateStatus(int id, GroupPayment.PaymentStatus status)
        {
            var data = Repository.Find(id);
            data.Status = status;
            Repository.Update(data);
            Repository.Save();
        }

        public bool ExistReserveId(long reserveId, GroupPayment.PaymentStatus status)
        {
            return Repository.Query(q => q.Any(a => a.StatusInt == (int)status &&
                  a.ReserveIds.Contains(reserveId.ToString())));
        }
    }
}
