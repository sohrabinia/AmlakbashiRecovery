using Amlakbashi.Core.DTOs.WebService.Requests.Comments;
using Amlakbashi.Core.Entities;
using System.Collections.Generic;
using System.Linq;

namespace Amlakbashi.Application.Services.CommentServices.Interfaces
{
    public interface IReportItemAppService
    {
        IQueryable<ReportItem> GetAllAsIQueriable();
        IList<ReportItem> GetAccUserRatings(long accId, int userId);
        IList<ReportItem> GetByAccList(IList<long> accIds);
        IList<ReportItem> GetListByUserId(int userId);
        int GetCountByAccId(long accId, IQueryable<ReportItem> reportItems = null);
        int GetAdvertiseRatingOfUser(int userId, long advertiseId, int reportId);
        void Submit(int userId, long advertiseId, IList<CommentPostScoresRequest> scores);
        void SubmitAdvertiseScore(int userId, long advertiseId, int reportId,
            int score, int operatorId = 0);
    }
}
