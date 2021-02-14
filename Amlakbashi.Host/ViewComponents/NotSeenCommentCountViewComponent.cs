using Amlakbashi.Application.Services.CommentServices.Interfaces;
using Amlakbashi.Host.Authentication;
using Microsoft.AspNetCore.Mvc;
using Entities = Amlakbashi.Core.Entities;

namespace Amlakbashi.Host.ViewComponents
{
    public class NotSeenCommentCountViewComponent: ViewComponent
    {
        private readonly IUserAccessor userAccessor;
        private readonly ICommentAppService commentService;
        public NotSeenCommentCountViewComponent(IUserAccessor userAccessor,
            ICommentAppService commentService)
        {
            this.userAccessor = userAccessor;
            this.commentService = commentService;
        }

        public IViewComponentResult Invoke()
        {
            if (userAccessor.CurrentUser.UserGeneralType == (int)Entities.User.UserGeneralTypeEnum.Guest)
            {
                return View("_NotSeenCommentCount", 0);
            }
            return View("_NotSeenCommentCount", commentService.GetNotSeenCommentCount(userAccessor.CurrentUser.Id));
        }
    }
}
