using Amlakbashi.Application.DTOs;
using Amlakbashi.Core.Common.AppService;
using Amlakbashi.Core.DTOs.WebService.Requests.Comments;
using Amlakbashi.Core.DTOs.WebService.Responses.Comments;
using Amlakbashi.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Application.Services.CommentServices.Interfaces
{
    public interface ICommentAppService : IAppService<Comment, long>
    {
        IQueryable<Comment> GetAllAsIQueryable();
        IList<Comment> GetListBySenderUserId(int userId);
        IList<Comment> Filter(int status = (int)Comment.CommentStatus.ready,
            int comment_type = -1, int comment_id = -1, int sender_user_id = -1, long advertise_id = -1);
        Comment Find(long id);
        void Delete(long id);
        Comment GetParent(int id);
        Comment GetByAccSenderUser(long advertiseId, long senderUserId);
        Comment GetHostReply(long accId, long senderUserId);
        CommentListResponse GetForHost(int userId, bool seenByHost = true, int page = 1, int pageItemCount = 20);
        void Insert(Comment newComment);
        void SubmitGuestComment(int userId, long advertiseId, string text);
        ServiceResult<bool> SubmitHostReply(CommentPostHostRequest requst);
        void Update(Comment editedComment);
        void UpdateStatus(long id, Comment.CommentStatus status, string suspendReason = "");
        void SetAsSeenByHost(long accId);
        string GetNotVerifyReasonIfExists(long accId, int accUserid, int currentUserId);
        bool AnyComment(long advertiseId);
        int GetNotSeenCommentCount(int userId);
    }
}
