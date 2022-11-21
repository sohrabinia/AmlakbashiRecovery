using Amlakbashi.Core.Common.AppService;
using Amlakbashi.Core.Common.Repository;
using Amlakbashi.Application.Services.ActionLogServices.Interfaces;
using Amlakbashi.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Application.Services.ActionLogServices
{
    internal class ActionLogAppService : BaseAppService<ActionLog, long>, IActionLogAppService
    {
        public ActionLogAppService(IRepository<ActionLog, long> repository) : base(repository)
        {
        }

        public IList<ActionLog> Filter(int userId, int actionType, int actionSource, long relatedId)
        {
            IQueryable<ActionLog> model = Repository.Query(q => q);
            if (userId > 0)
            {
                model = model.Where(x => x.UserID == userId);
            }
            if (actionType > -1)
            {
                model = model.Where(x => x.Type == actionType);
            }
            if (actionSource > 0)
            {
                model = model.Where(x => x.ActionSource == actionSource);
            }
            if (relatedId > 0)
            {
                model = model.Where(x => x.RelatedID == relatedId);
            }

            return model.OrderByDescending(x => x.Id).ToList();
        }

        public ActionLog Find(long id)
        {
            return Repository.Query(q => q.FirstOrDefault(f => f.Id == id));
        }
    }
}
