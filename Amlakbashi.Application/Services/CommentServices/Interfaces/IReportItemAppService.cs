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
        IList<ReportItem> GetAll();
        IQueryable<ReportItem> GetAllAsIQueriable();
        IList<ReportItem> GetAccUserRatings(long accId, int userId);
        IList<ReportItem> GetByAccList(IList<long> accIds);
        IList<ReportItem> GetListByUserId(int userId);
        Dictionary<int, List<ReportItem>> GetByAccId(long advertiseId);
        int GetCountByAccId(long accId, IQueryable<ReportItem> reportItems = null);
        float GetOverallRatingDecimal(long accId, int reportId = -1);
        int GetAdvertiseRatingOfUser(int userId, long advertiseId, int reportId);
        int GetAdvertiseRating(long advertise_id, out int count);
        float GetAverageRatingForAdvertise(long advertise_id, int user_id = 0);
        void SubmitAdvertiseScore(int userId, long advertiseId, int reportId,
            int score, int operatorId = 0);
    }
}
