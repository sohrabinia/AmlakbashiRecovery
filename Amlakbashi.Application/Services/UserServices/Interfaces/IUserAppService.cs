using Amlakbashi.Core.Common.AppService;
using Amlakbashi.Core.DTOs.UserDTOs;
using Amlakbashi.Core.Entities;
using Amlakbashi.Core.Identity.Entities;
using Amlakbashi.Core.Infrastructure.UserContact;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

namespace Amlakbashi.Application.Services.UserServices.Interfaces
{
    public interface IUserAppService : IAppService<User, int>
    {
        IList<User> GetAll();
        IQueryable<User> GetAllAsIQueryable();
        IQueryable<User> GetAllById(int id);
        int CountNewUserInDates(DateTime fromDate, DateTime toDate, List<int> userList = null);
        User Find(int id, bool includeFavorite = false);
        User Find(int? id);
        User GetByAdminLoginCode(string code);
        User GetByMainMobile(string mainMobile);
        User GetActivatedUserByMainMobile(string mainMobile, bool includeFavorite = false);
        User GetActivatedUserByEmail(string email, bool includeFavorite = false);
        void Insert(User user, int currentUserId = 0, ActionLog.ActionSourceEnum source = ActionLog.ActionSourceEnum.AdminPanel);
        bool Update(UserDTO dto, int currentUserId, bool userHasRefunedInProgress,
            ActionLog.ActionSourceEnum source, out List<string> errors);
        void UpdateState(int userId, bool state, int currentUserId = 0,
            ActionLog.ActionSourceEnum source = ActionLog.ActionSourceEnum.AdminPanel);
        void UpdateContactPhone(int userId, bool state);
        void UpdateProfilePhoto(int userId, long photoId, User.UserPhotoState state);
        void UpdatePhotoStatus(int userId, User.UserPhotoState state, int currentUserId = 0,
            ActionLog.ActionSourceEnum source = ActionLog.ActionSourceEnum.AdminPanel);
        void UpdateCreateDate(int userId, DateTime time);
        void UpdateSendVerification(int userId, DateTime time, string code = null);
        void UpdatePresentorUser(int userId, int pid);
        void UpdateFName(int userId, string newFName);
        void UpdateLName(int userId, string newLName);
        void UpdateFNameLName(int userId, string newFName, string newLName);
        void UpdateDesc(int userId, string desc);
        void UpdateLoginCode(int userId, string token);
        void UpdateLastNotifPermetionTicks(int userId, long ticks);
        void UpdateInstantReserveAccess(int userId, User.InstantReserveAccessEnum instantReserveAccess,
            int currentUserId = 0, ActionLog.ActionSourceEnum source = ActionLog.ActionSourceEnum.AdminPanel);
        void UpdateUserNotificationToken(int userId, string token);
        void UpdateFcmNotificationToken(int userId, string token);
        void UpdateAppNotificationToken(int userId, string token);
        void UpdateUserGeneralType(int userId, User.UserGeneralTypeEnum userGeneralType);
        void UpdateForgetCode(int userId, string code);
        void Delete(int userId, int currentUserId = 0, ActionLog.ActionSourceEnum source = ActionLog.ActionSourceEnum.AdminPanel);
        void AddFavorite(int userId, long advertiseId);
        void DeleteFavorite(int userId, long advertiseId);
        void SendCustomSms(int delay, string mobile, string template);
        void SendNotificationApplication(string token, string title, string body, string targetAction, string targetId);
        void SendGroupNotification(List<string> tokens, string title, string body, string clickAction);
        void SendVerificationSms(string localNumber, string code);
        bool VerifyLogin(string mobile, out int user_id, string presentorCode, out string errorMsg);
        void SendMessage(UserContactDTO userContact);
        void SendSms(UserContactDTO userContact);
        IList<string> GetAllIdentityUsernamesByState(User.UserState state = User.UserState.Acticved);
        AppUser GetActivatedIdentityUser(string phrase, bool isEmail = false);
        AppUser GetIdentityUser(string phrase, bool isEmail = false);
        void AddIdentityUser(AppUser user);
        bool AddClaimsToUser(string username, IList<Claim> claims);
        void RemoveClaimsFromUser(string username, IList<Claim> claims);
        IdentityResult AddIdentityUserPassword(string username, string password);
        IdentityResult ChangeIdentityUserPassword(string username, string password);
        IdentityResult ChangeIdentityUserPassword(string username, string currentPassword, string newPassword);
        IdentityResult ChangePassword(string username, string currentPassword, string newPassword);
        void UpdateIdentityUser(AppUser user);
        bool VerifyLoginCode(string mobileInternational, string code);
        IList<AppRole> GetAllRoles();
        IList<string> GetAllRoleNames();
        IList<string> GetUserRoles(string username);
        void UpdateUserRoles(string username, IList<string> selectedRoles);
        IList<User> GetRoleUserList(string roleName);
        bool SignInRegister(int user_id, string fname, string lname,
            out Dictionary<string, string> errors);
        bool SignInRegisterOld(int user_id, string fname, string lname,
            string password, string confirmPassword, out Dictionary<string, string> errors);
        JwtSecurityToken JwtSignIn(AppUser identityUser, byte[] key);
        IEnumerable<User> IdentityUsersToUsers(IEnumerable<AppUser> identityUsers);
        IEnumerable<AppUser> GetAllSupportEmployees();
        IEnumerable<AppUser> GetAllEmployees();
        bool UserAllowPolicy(AppUser identityUser, string policy);
        void SignOut();
    }
}
