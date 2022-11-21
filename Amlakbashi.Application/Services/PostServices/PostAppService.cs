using Amlakbashi.Core.Common.AppService;
using Amlakbashi.Core.Common.Repository;
using System;
using System.Collections.Generic;
using Amlakbashi.Application.Services.PostServices.Interfaces;
using Amlakbashi.Core.Entities;
using System.Linq;
using static Amlakbashi.Core.Entities.Post;
using System.Transactions;

namespace Amlakbashi.Application.Services.PostServices
{
    internal class PostAppService : BaseAppService<Post, long>, IPostAppService
    {
        private readonly IRepository<ServicePost, int> servicePostRepository;
        public PostAppService(
            IRepository<Post, long> repository,
            IRepository<ServicePost, int> servicePostRepository) : base(repository)
        {
            this.servicePostRepository = servicePostRepository;
        }

        public void Delete(long id)
        {
            Repository.Delete(id);
            Repository.Save();
        }

        public IList<Post> Filter(Post.PostStatus status, int serviceId)
        {
            var statusInt = (int)status;
            var list = Repository.Query(q => q.Where(w => w.Status == statusInt));
            if (serviceId != -1)
            {
                var servicePostList = servicePostRepository.Query(q => q.Where(w => w.ServiceID == serviceId));
                var postIds = servicePostList.Select(s => s.PostID).Distinct().ToList();
                list = list.Where(w => postIds.Contains(w.Id));
            }
            list = list.OrderByDescending(o => o.LastModifyDate);
            return list.ToList();
        }

        public Post Find(long id)
        {
            return Repository.Query(q =>
                q.FirstOrDefault(f => f.Id == id));
        }

        public IList<Post> GetAll()
        {
            return Repository.Query(q => q).ToList();
        }

        public IList<int> GetRelatedServiceIds(long id)
        {
            var servicePostList = servicePostRepository.Query(q =>
                q.Where(w => w.PostID == id));
            return servicePostList.Select(s => s.ServiceID).ToList();
        }

        public void Insert(Post item, int userId, List<int> serviceIds)
        {
            using (var tran = new TransactionScope())
            {
                item.PostDate = DateTime.Now;
                item.SetStatus(PostStatus.ReadyToPublish);
                item.UserID = userId;
                item.LastModifyDate = DateTime.Now;
                Repository.Insert(item);
                Repository.Save();
                servicePostRepository.Delete(
                    q => q.PostID == item.Id);

                foreach (int servideId in serviceIds)
                {
                    var servicePostItem = new ServicePost();
                    servicePostItem.PostID = item.Id;
                    servicePostItem.ServiceID = servideId;
                    servicePostRepository.Insert(servicePostItem);
                }
                servicePostRepository.Save();
                tran.Complete();
            }
        }

        public void SetStatus(long id, PostStatus status)
        {
            var item = Repository.Query(q =>
                q.FirstOrDefault(f => f.Id == id));
            item.SetStatus(status);
            Repository.Update(item);
            Repository.Save();
        }

        public void Update(Post item, List<int> serviceIds)
        {
            using (var tran = new TransactionScope())
            {
                var currItem = Repository.Query(q => q.FirstOrDefault(f => f.Id == item.Id));
                currItem.Title = item.Title;
                currItem.FileID = item.FileID;
                currItem.Link = item.Link;
                currItem.Abstract = item.Abstract;
                currItem.Description = item.Description;
                currItem.PhotoID = item.PhotoID;
                currItem.LastModifyDate = DateTime.Now;
                Repository.Update(currItem);
                Repository.Save();

                servicePostRepository.Delete(q => q.PostID == currItem.Id);
                foreach (int servideId in serviceIds)
                {
                    var servicePostItem = new ServicePost();
                    servicePostItem.PostID = item.Id;
                    servicePostItem.ServiceID = servideId;
                    servicePostRepository.Insert(servicePostItem);
                }
                servicePostRepository.Save();
                tran.Complete();
            }
        }
    }
}
