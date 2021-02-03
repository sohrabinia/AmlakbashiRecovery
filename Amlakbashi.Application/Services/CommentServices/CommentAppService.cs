using Amlakbashi.Core.Common.AppService;
using Amlakbashi.Core.Common.Repository;
using Amlakbashi.Application.Services.CommentServices.Interfaces;
using Amlakbashi.Core.Common.Caching;
using Amlakbashi.Core.Entities;
using Amlakbashi.Data;
using System.Collections.Generic;
using System.Linq;

namespace Amlakbashi.Application.Services.CommentServices
{
    internal class CommentAppService : AppServiceBase<Comment, long>, ICommentAppService
    {
        public CommentAppService(IRepository<Comment, long> repository, ICacheManager<Comment> cache) : base(repository, cache)
        {

        }

        public IList<Comment> GetAll()
        {
            return Repository.Query(q => q).ToList();
        }

        //TODO: Delete this
        public IQueryable<Comment> GetAllAsIQueryable()
        {
            return Repository.Query(q => q);
        }

        public IList<Comment> GetListByAccId(long accId)
        {
            return Repository.Query(q => q.Where(x => x.AdvertiseID == accId &&
                      x.type == (int)Comment.CommentType.advertise &&
                      x.Status == Comment.CommentStatus.publish).
                      OrderByDescending(x => x.Id).ToList());
        }

        public IList<Comment> GetListBySenderUserId(int userId)
        {
            return Repository.Query(q => q.Where(x => x.SenderUserID == userId).ToList());
        }

        public string GetNotVerifyReasonIfExists(long accId, int accUserid, int currentUserId)
        {
            var comments = Repository.Query(q => q);
            var isHost = accUserid == currentUserId;
            comments = comments.Where(X => X.AdvertiseID == accId);
            comments = comments.Where(x => x.Status == Comment.CommentStatus.suspend);
            if (isHost)
            {
                comments = comments.Where(x => x.type == Comment.CommentType.advertiseHostReply);
            }
            else
            {
                comments = comments.Where(x => x.type == (int)Comment.CommentType.advertise);
                comments = comments.Where(x => x.SenderUserID == currentUserId);
            }
            if (comments.Any())
            {
                var comment = comments.First();
                var reason = comment.SuspendReason;
                if (!string.IsNullOrEmpty(reason))
                {
                    return reason;
                }
            }
            return "";
        }

        public IList<Comment> Filter(int status = (int)Comment.CommentStatus.ready,
            int comment_type = -1, int comment_id = -1, int sender_user_id = -1, long advertise_id = -1)
        {
            IQueryable<Comment> model = Repository.Query(q => q);
            if (comment_type != -1)
            {
                model = model.Where(x => x.type == (Comment.CommentType)comment_type);
            }
            if (comment_id != -1)
            {
                model = model.Where(a => a.Id == comment_id);
            }
            if (sender_user_id != -1)
            {
                model = model.Where(a => a.SenderUserID == sender_user_id);
            }
            if (advertise_id > 0)
            {
                model = model.Where(a => a.AdvertiseID == advertise_id);
            }
            if (status != -1)
            {
                model = model.Where(x => x.Status == (Comment.CommentStatus)status);
            }
            return model.OrderByDescending(x => x.Id).ToList();
        }

        public Comment Find(long id)
        {
            return Repository.Query(q => q.FirstOrDefault(f => f.Id == id));
        }

        public void Delete(long id)
        {
            Repository.Delete(id);
            Repository.Save();
        }

        public Comment GetParent(int id)
        {
            return Repository.Query(q => q.FirstOrDefault(f => f.ParentID == id));
        }

        public Comment GetByAccSenderUser(long accId, long senderUserId)
        {
            return Repository.Query(q => q.FirstOrDefault(f => f.AdvertiseID == accId &&
                f.SenderUserID == senderUserId));
        }

        public Comment GetByAccSenderUser(long accId, long senderUserId, Comment.CommentType type, bool onlyPublished)
        {
            var comments = Repository.Query(q => q.Where(f => f.AdvertiseID == accId &&
                f.SenderUserID == senderUserId && f.type == type));
            if (onlyPublished)
            {
                comments = comments.Where(x => x.Status == Comment.CommentStatus.publish);
            }
            return comments.FirstOrDefault();
        }

        public Comment GetHostReply(long accId, long senderUserId)
        {
            return Repository.Query(q => q.FirstOrDefault(x => x.AdvertiseID == accId &&
                x.SenderUserID == senderUserId &&
                x.type == Comment.CommentType.advertiseHostReply));
        }

        public Comment GetSuspendedComment(long accId, int userId)
        {
            IQueryable<Comment> comments = Repository.Query(q => q);
            comments = comments.Where(x => x.AdvertiseID == accId);
            comments = comments.Where(x => x.SenderUserID == userId);
            comments = comments.Where(x => x.Status == Comment.CommentStatus.suspend);
            comments = comments.Where(x => x.type == (int)Comment.CommentType.advertise);
            return comments.FirstOrDefault();
        }

        public void Update(Comment editedComment)
        {
            var data = Repository.Query(q => q.FirstOrDefault(f => f.Id == editedComment.Id));
            data.Text = editedComment.Text;
            data.RecieverUserID = editedComment.RecieverUserID;
            data.SenderUserID = editedComment.SenderUserID;
            data.Status = editedComment.Status;
            data.type = editedComment.type;
            data.SuspendReason = editedComment.SuspendReason;
            Repository.Update(data);
            Repository.Save();
        }

        public void UpdateStatus(long id, Comment.CommentStatus status, string suspendReason = "")
        {
            var data = Repository.Query(q => q.FirstOrDefault(f => f.Id == id));
            data.Status = status;
            if (status == Comment.CommentStatus.suspend)
            {
                data.SuspendReason = suspendReason;
            }
            Repository.Update(data);
            Repository.Save();
        }

        public void SetAsSeenByHost(int accId)
        {
            var data = Repository.Query(q => q.Where(w => w.AdvertiseID == accId &&
                w.type == (int)Comment.CommentType.advertise &&
                w.Status == Comment.CommentStatus.publish)).ToList();
            foreach (var item in data)
            {
                item.SeenByHost = true;
                Repository.Update(item);
            }
            Repository.Save();
        }

        public void SetAsSeenByHost(long accId)
        {
            var data = Repository.Query(q => q.Where(w => w.AdvertiseID == accId &&
                w.type == (int)Comment.CommentType.advertise &&
                w.Status == Comment.CommentStatus.publish)).ToList();
            foreach (var item in data)
            {
                item.SeenByHost = true;
                Repository.Update(item);
            }
            Repository.Save();
        }

        public int GetNotSeenCommentCount(int userId)
        {
            return Repository.Query(q => q.Count(x =>
                x.Advertise.Status != Advertise.AdvertiseStatus.Deleted &&
                x.Advertise.UserID == userId &&
                x.Status == Comment.CommentStatus.publish &&
                x.type == Comment.CommentType.advertise &&
                !x.SeenByHost));
        }

        public void Insert(Comment newComment)
        {
            Repository.Insert(newComment);
            Repository.Save();
        }

        public bool AnyComment(long advertiseId)
        {
            var comments = Repository.Query(q => q);
            return comments.Any(x => x.AdvertiseID == advertiseId &&
                x.type == (int)Comment.CommentType.advertise && x.Status == Comment.CommentStatus.publish);
        }
    }
}
