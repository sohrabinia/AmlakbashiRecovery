using Amlakbashi.Core.Common.AppService;
using Amlakbashi.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Application.Services.ReserveServices.Interfaces
{
    public interface IReserveSupportAppService : IAppService<ReserveSupport,int>
    {
        void Insert(ReserveSupport item);
        void Update(ReserveSupport item);
        void UpdateSupporterSupportsActionDate(int supporterId);
        IList<ReserveSupport> GetRelatedSupports(long reserveId);
        IList<ReserveSupport> GetRelatedSupports(Reserve reserve);
        IList<ReserveSupport> GetListBySupporterId(int supporterId);
        IQueryable<Reserve> FilterBySupporterStatus(int yourUserID,
            IQueryable<Reserve> reserves, ReserveSupport.SupporterStatus supporterStatus);
        bool IsInSupporterStatus(Reserve reserve,
            ReserveSupport.SupporterStatus supporterStatus, int yourUserID);
    }
}
