using Amlakbashi.Core.Entities;

namespace Amlakbashi.Application.Services.SettingServices.Interfaces
{
    public interface ISettingAppService
    {
        Setting Find(string name);
        void Update(string name, string value);
    }
}
