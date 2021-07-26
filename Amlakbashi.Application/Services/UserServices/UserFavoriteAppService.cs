using Amlakbashi.Core.Common.AppService;
using Amlakbashi.Core.Common.Repository;
using Amlakbashi.Application.Services.UserServices.Interfaces;
using Amlakbashi.Core.Entities;

namespace Amlakbashi.Application.Services.UserServices
{
    internal class UserFavoriteAppService : AppServiceBase<UserFavorite, int>, IUserFavoriteAppService
    {
        public UserFavoriteAppService(IRepository<UserFavorite, int> repository): base(repository)
        {
        }
    }
}
