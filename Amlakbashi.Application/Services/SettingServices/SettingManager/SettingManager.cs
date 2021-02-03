using Amlakbashi.Application.Services.SettingServices.Interfaces;

namespace Amlakbashi.Application.Services.SettingServices.SettingManager
{
    public class SettingManager : ISettingManager
    {
        private readonly ISettingAppService settingService;
        public SettingManager(ISettingAppService settingService)
        {
            this.settingService = settingService;
        }
        private const string maxScoreName = "max_score";
        public long MaxScore
        {
            get
            {
                var item = settingService.Find(maxScoreName);
                if (item != null)
                {
                    long result;
                    if (item.Value != null && long.TryParse(item.Value, out result))
                    {
                        return result;
                    }
                    return 0;
                }
                return 0;
            }
            set
            {
                settingService.Update(maxScoreName, value.ToString());
            }
        }
    }
}
