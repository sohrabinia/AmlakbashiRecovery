using Amlakbashi.Core.Common.AppService;
using Amlakbashi.Core.Entities;
using System.Collections.Generic;
using static Amlakbashi.Core.Entities.Post;

namespace Amlakbashi.Application.Services.PostServices.Interfaces
{
    public interface IPostAppService : IAppService<Post, long>
    {
        Post Find(long id);
        void Update(Post item, List<int> serviceIds);
        void Insert(Post item, int userId, List<int> serviceIds);
        void Delete(long id);
        void SetStatus(long id, PostStatus status);
        IList<Post> Filter(PostStatus status, int serviceId);
        IList<int> GetRelatedServiceIds(long id);
        IList<Post> GetAll();
    }
}
