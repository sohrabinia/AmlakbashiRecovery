using Amlakbashi.Core.Common.AppService;
using Amlakbashi.Core.Common.Repository;
using System.Collections.Generic;
using Amlakbashi.Application.Services.PostServices.Interfaces;
using Amlakbashi.Core.Common.Caching;
using Amlakbashi.Core.Entities;
using System.Linq;
using System.Transactions;
using MediatR;
using Amlakbashi.Mediator.Events.PostEvents;

namespace Amlakbashi.Application.Services.PostServices
{
    internal class ServiceAppService : AppServiceBase<Service, int>, IServiceAppService
    {
        private readonly IMediator mediator;
        private readonly IRepository<ServicePost, int> servicePostRepository;
        public ServiceAppService(IRepository<Service, int> repository,
            IRepository<ServicePost, int> servicePostRepository,
            IMediator mediator,
            ICacheManager<Service> cache) : base(repository, cache)
        {
            this.mediator = mediator;
            this.servicePostRepository = servicePostRepository;
        }

        public Service Find(int id)
        {
            return Repository.Query(q =>
                q.FirstOrDefault(w => w.Id == id));
        }

        public IList<Service> GetAll()
        {
            return Repository.Query(q => q).ToList();
        }

        public IList<Service> GetRoots()
        {
            var list = Repository.Query(q =>
                q.Where(w => w.ParentId == -1));
            return list.ToList();
        }

        public IList<Service> GetChildren(int parentId)
        {
            var list = Repository.Query(q =>
                q.Where(w => w.ParentId == parentId));
            return list.ToList();
        }
        public bool Validate(Service item)
        {
            if (item.ParentId == item.Id)
                return false;
            var children = Repository.Query(q =>
                q.Where(w => w.ParentId == item.Id));
            foreach (var child in children)
            {
                if (Validate(item) == false)
                    return false;
            }
            return true;
        }
        public void Update(Service item)
        {
            var currItem = Repository.Query(q =>
                q.FirstOrDefault(f => f.Id == item.Id));
            currItem.Title = item.Title;
            currItem.ParentId = item.ParentId;
            Repository.Update(currItem);
            Repository.Save();
        }

        public void Insert(Service item)
        {
            Repository.Insert(item);
            Repository.Save();
        }

        public void Delete(int id)
        {
            using (var tran = new TransactionScope())
            {
                mediator.Publish(new DeleteServiceEvent(id));
                tran.Complete();
            }
        }
    }
}
