using Amlakbashi.Core.Entities;
using System.Collections.Generic;

namespace Amlakbashi.Application.Services.PostServices.Interfaces
{
    public interface IServiceAppService
    {
        Service Find(int id);
        IList<Service> GetAll();
        IList<Service> GetRoots();
        IList<Service> GetChildren(int parentId);
        bool Validate(Service item);
        void Update(Service item);
        void Insert(Service item);
        void Delete(int id);
    }
}
