using Amlakbashi.Application.Services.SettingServices.Interfaces;
using Amlakbashi.Core.Common.AppService;
using Amlakbashi.Core.Common.Caching;
using Amlakbashi.Core.Common.Repository;
using Amlakbashi.Core.Entities;
using System;
using System.Linq;

namespace Amlakbashi.Application.Services.SettingServices
{
    public class SettingAppService : AppServiceBase<Setting, int>, ISettingAppService
    {
        public SettingAppService(IRepository<Setting, int> repository, ICacheManager<Setting> cache) : base(repository, cache)
        {
        }

        public Setting Find(int id)
        {
            return Repository.Find(id);
        }

        public Setting Find(string name)
        {
            return Repository.Query(q => q.SingleOrDefault(s => s.Name == name));
        }

        public void Update(string name, string value)
        {
            var item = Repository.Query(q => q.SingleOrDefault(s => s.Name == name));
            if (item != null)
            {
                item.Value = value;
                Repository.Update(item);
                Repository.Save();
            }
            else
            {
                item = new Setting();
                item.Name = name;
                item.Value = value;
                Repository.Insert(item);
                Repository.Save();
            }
        }
    }
}
