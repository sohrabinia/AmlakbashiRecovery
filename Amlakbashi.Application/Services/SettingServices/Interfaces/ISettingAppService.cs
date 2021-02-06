using Amlakbashi.Core.Common.AppService;
using Amlakbashi.Core.Entities;

namespace Amlakbashi.Application.Services.SettingServices.Interfaces
{
    public interface ISettingAppService : IAppService<Setting,int>
    {
        Setting Find(string name);
        void Update(string name, string value);
    }
}
