using Amlakbashi.Core.Common.AppService;
using Amlakbashi.Core.Common.Repository;
using Amlakbashi.Application.Services.CommentServices.Interfaces;
using Amlakbashi.Core.Entities;
using System.Collections.Generic;
using System.Linq;
using System;
using Amlakbashi.Core.DTOs.WebService.Responses.Comments;
using Amlakbashi.Core.DTOs.WebService.Requests.Comments;
using Amlakbashi.Application.DTOs;
using Amlakbashi.Core.Common.Extensions;
using Amlakbashi.Core.DTOs;

namespace Amlakbashi.Application.Services.CommentServices
{
    internal class CommentAppService : AppServiceBase<Comment, long>, ICommentAppService
    {
        public CommentAppService(IRepository<Comment, long> repository) : base(repository)
        {
        }

        //TODO: Delete this
        public IQueryable<Comment> GetAllAsIQueryable()
        {
            return Repository.Query(q => q);
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

        public CommentListResponse GetForHost(int userId, bool seenByHost = true, int page = 1, int pageItemCount = 20)
        {
            var user = Repository.Find<User, int>(userId);
            if (user == null || user.UserGeneralType != 1)
            {
                return null;
            }
            var pagedList = user.Advertises.SelectMany(x =>
                x.Comments.Where(y => y.Status == Comment.CommentStatus.publish &&
                y.type == Comment.CommentType.advertise && y.SeenByHost == seenByHost)).ToPagedList(page, pageItemCount);
            if (pagedList.List.Any() && seenByHost == false)
            {
                UpdateHostCommentsToSeened(userId);
            }
            var response = new CommentListResponse()
            {
                pagingInfo = pagedList.PagingInfo,
                comments = pagedList.List.Select(x => (CommentResponse)x).ToList()
            };
            return response;
        }

        private void UpdateHostCommentsToSeened(int userId)
        {
            var user = Repository.Find<User, int>(userId);
            if (user == null || user.UserGeneralType != 1)
            {
                return;
            }
            var unseenComments = user.Advertises.SelectMany(x =>
                x.Comments.Where(y => y.Status == Comment.CommentStatus.publish &&
                y.type == Comment.CommentType.advertise && y.SeenByHost == false));
            foreach (var item in unseenComments)
            {
                item.SeenByHost = true;
                Repository.Update(item);
            }
            Repository.Save();
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

        public Comment GetHostReply(long accId, long senderUserId)
        {
            return Repository.Query(q => q.FirstOrDefault(x => x.AdvertiseID == accId &&
                x.SenderUserID == senderUserId &&
                x.type == Comment.CommentType.advertiseHostReply));
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

        public void SubmitGuestComment(int userId, long advertiseId, string text)
        {
            var advertise = Repository.Find<Advertise, long>(advertiseId);
            var comment = advertise.Comments.FirstOrDefault(f => f.SenderUserID == userId);
            if (comment == null)
            {
                comment = new Comment()
                {
                    SenderUserID = userId,
                    Status = Comment.CommentStatus.ready,
                    type = Comment.CommentType.advertise,
                    Text = text,
                    CreateDate = DateTime.Now,
                    LastModifyDate = DateTime.Now,
                    LastModifyDatetick = DateTime.Now.Ticks,
                    AdvertiseID = advertiseId
                };
                Repository.Insert(comment);
            }
            else
            {
                comment.Text = text;
                comment.Status = (int)Comment.CommentStatus.ready;
                comment.LastModifyDate = DateTime.Now;
                comment.LastModifyDatetick = DateTime.Now.Ticks;
                Repository.Update(comment);
            }
            Repository.Save();
        }

        public ServiceResult<bool> SubmitHostReply(CommentHostSubmitRequest requst)
        {
            var serviceResult = new ServiceResult<bool>();
            var comment = Repository.Find(requst.commentId);
            if (comment == null || comment.HostReplyId != null)
            {
                serviceResult.AddError("comment id is invalid");
                return serviceResult;
            }
            if (comment.Advertise.UserID != requst.userId)
            {
                serviceResult.AddError("invalid user");
                return serviceResult;
            }
            comment.HostReply = new Comment()
            {
                AdvertiseID = comment.AdvertiseID,
                SeenByHost = true,
                CreateDate = DateTime.Now,
                LastModifyDate = DateTime.Now,
                Status = Comment.CommentStatus.ready,
                type = Comment.CommentType.advertiseHostReply,
                Text = requst.text
            };
            Repository.Update(comment);
            Repository.Save();
            serviceResult.Result = true;
            return serviceResult;
        }

        public bool AnyComment(long advertiseId)
        {
            var comments = Repository.Query(q => q);
            return comments.Any(x => x.AdvertiseID == advertiseId &&
                x.type == (int)Comment.CommentType.advertise && x.Status == Comment.CommentStatus.publish);
        }
    }
}
