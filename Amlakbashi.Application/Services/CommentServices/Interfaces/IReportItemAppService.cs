using Amlakbashi.Core.Common.AppService;
using Amlakbashi.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Application.Services.CommentServices.Interfaces
{
    public interface IReportItemAppService : IAppService<ReportItem, long>
    {
        IQueryable<ReportItem> GetAllAsIQueriable();
        IList<ReportItem> GetAccUserRatings(long accId, int userId);
        IList<ReportItem> GetByAccList(IList<long> accIds);
        IList<ReportItem> GetListByUserId(int userId);
        int GetCountByAccId(long accId, IQueryable<ReportItem> reportItems = null);
        int GetAdvertiseRatingOfUser(int userId, long advertiseId, int reportId);
        void SubmitAdvertiseScore(int userId, long advertiseId, int reportId,
            int score, int operatorId = 0);
    }
}
