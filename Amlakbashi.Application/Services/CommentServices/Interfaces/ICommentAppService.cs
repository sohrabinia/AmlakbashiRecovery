using Amlakbashi.Core.Common.AppService;
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
        IList<Comment> GetAll();
        IQueryable<Comment> GetAllAsIQueryable();
        IList<Comment> GetListByAccId(long accId);
        IList<Comment> GetListBySenderUserId(int userId);
        IList<Comment> Filter(int status = (int)Comment.CommentStatus.ready,
            int comment_type = -1, int comment_id = -1, int sender_user_id = -1, long advertise_id = -1);
        Comment Find(long id);
        void Delete(long id);
        Comment GetParent(int id);
        Comment GetByAccSenderUser(long advertiseId, long senderUserId);
        Comment GetByAccSenderUser(long accId, long senderUserId, Comment.CommentType type, bool onlyPublished);
        Comment GetHostReply(long accId, long senderUserId);
        Comment GetSuspendedComment(long accId, int userId);
        void Insert(Comment newComment);
        void Update(Comment editedComment);
        void UpdateStatus(long id, Comment.CommentStatus status, string suspendReason = "");
        void SetAsSeenByHost(int accId);
        void SetAsSeenByHost(long accId);
        string GetNotVerifyReasonIfExists(long accId, int accUserid, int currentUserId);
        bool AnyComment(long advertiseId);
        int GetNotSeenCommentCount(int userId);
    }
}
