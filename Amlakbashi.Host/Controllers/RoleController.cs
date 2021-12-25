using Amlakbashi.Application.Services.UserServices.Interfaces;
using Amlakbashi.Core.DTOs.UserDTOs;
using Amlakbashi.Core.Identity;
using Amlakbashi.Host.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Amlakbashi.Host.Controllers
{
    public class RoleController : BaseController
    {
        private readonly IUserAppService userService;
        public RoleController(IUserAppService userService)
        {
            this.userService = userService;
        }

#if !DEBUG
        [Authorize(Policy = Policies.Roles_View)]
#endif
        public IActionResult Index()
        {
            var roles = userService.GetAllRoles();
            return View(roles);
        }

#if !DEBUG
        [Authorize(Policy = Policies.Roles_Edit)]
#endif
        [HttpGet]
        public IActionResult EditUserRole(int userId)
        {
            var user = userService.Find(userId);
            var userRoles = userService.GetUserRoles(user.MainMobile);
            var allRoles = userService.GetAllRoleNames();
            UserRoleManagementDTO dto = new UserRoleManagementDTO()
            {
                UserId = user.Id,
                Fullname = user.FullName,
                MainMobile = user.MainMobile,
                AllRoles = allRoles,
                CurrentRoles = userRoles
            };
            return View(dto);
        }

#if !DEBUG
        [Authorize(Policy = Policies.Roles_Edit)]
#endif
        [HttpPost]
        public IActionResult EditUserRole(string mainMobile, List<string> selectedRoles)
        {
            userService.UpdateUserRoles(mainMobile, selectedRoles);
            return RedirectToAction(nameof(Index));
        }

#if !DEBUG
        [Authorize(Policy = Policies.Roles_View)]
#endif
        public IActionResult RoleUserList(string roleName)
        {
            ViewBag.roleName = roleName;
            var userList = userService.GetRoleUserList(roleName);
            return View(userList);
        }
    }
}
