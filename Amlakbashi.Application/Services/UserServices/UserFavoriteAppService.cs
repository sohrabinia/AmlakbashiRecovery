using Amlakbashi.Core.Common.AppService;
using Amlakbashi.Core.Common.Repository;
using Amlakbashi.Application.Services.UserServices.Interfaces;
using Amlakbashi.Core.Common.Caching;
using Amlakbashi.Core.Entities;
using Amlakbashi.Data;

namespace Amlakbashi.Application.Services.UserServices
{
    internal class UserFavoriteAppService : AppServiceBase<UserFavorite, int>, IUserFavoriteAppService
    {
        public UserFavoriteAppService(IRepository<UserFavorite, int> repository, ICacheManager<UserFavorite> cache): base(repository, cache)
        {

        }
    }
}
