using Amlakbashi.Core.Common.AppService;
using Amlakbashi.Core.Common.Repository;
using Amlakbashi.Core.Entities;
using System.Linq;
using System.Collections.Generic;
using System;
using MediatR;
using Amlakbashi.Core.Common.Utilities;
using Amlakbashi.Core.Infrastructure.UserContact.Interfaces;
using Amlakbashi.Core.Infrastructure.UserContact;
using Amlakbashi.Mediator.Commands.AccountingCommands;
using Amlakbashi.Mediator.Commands.UserCommands;
using Amlakbashi.Core.Common.Extensions;
using Amlakbashi.Mediator.Events.UserEvents;
using Amlakbashi.Core.DTOs.UserDTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Amlakbashi.Core.Identity.Entities;
using Amlakbashi.Core.Infrastructure.LocalizationHelpers;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Amlakbashi.Core.Identity;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Amlakbashi.Core.DTOs.WebService.Requests.User;
using System.Text;
using Amlakbashi.Core.Common.StaticData;

namespace Amlakbashi.Application.Services.UserServices
{
    internal class UserAppService : AppServiceBase<User, int>, Interfaces.IUserAppService
    {
        private readonly IMediator mediator;
        private readonly IUserContactFacade userContact;
        private readonly UserManager<AppUser> userManager;
        private readonly IPasswordValidator<AppUser> passwordValidator;
        private readonly RoleManager<AppRole> roleManager;
        private readonly SignInManager<AppUser> signInManager;
        public UserAppService(IRepository<User, int> repository,
            IUserContactFacade userContact,
            IMediator mediator,
            UserManager<AppUser> userManager,
            IPasswordValidator<AppUser> passwordValidator,
            RoleManager<AppRole> roleManager,
            SignInManager<AppUser> signInManager) : base(repository)
        {
            this.mediator = mediator;
            this.userContact = userContact;
            this.userManager = userManager;
            this.passwordValidator = passwordValidator;
            this.roleManager = roleManager;
            this.signInManager = signInManager;
        }

        public IList<User> GetAll()
        {
            return Repository.Query(q => q).ToList();
        }

        public IQueryable<User> GetAllAsIQueryable()
        {
            return Repository.Query(q => q);
        }

        public IQueryable<User> GetAllById(int id)
        {
            return Repository.Query(q => q.Where(w => w.Id == id));
        }

        public int CountNewUserInDates(DateTime fromDate, DateTime toDate, List<int> userList = null)
        {
            if (userList == null)
            {
                return userManager.Users.Where(w => w.CreateDate >= fromDate && w.CreateDate <= toDate).Count();
            }
            var usersMainMobile = Repository.Query(q => q.Where(u => userList.Contains(u.Id)).Select(s => s.MainMobile));
            return userManager.Users.Where(w => w.CreateDate >= fromDate && w.CreateDate <= toDate)
                .Select(s => usersMainMobile.Contains(s.UserName)).Count();
        }

        public User Find(int id, bool includeFavorite = false)
        {
            if (includeFavorite)
                return Repository.Query(q => q.Include(i => i.Favorite).FirstOrDefault(f => f.Id == id));
            return Repository.Query(q => q.FirstOrDefault(f => f.Id == id));
        }

        public User Find(int? id)
        {
            if (id != null)
            {
                return Repository.Query(q => q.FirstOrDefault(f => f.Id == id));
            }
            return null;
        }

        public User GetByMainMobile(string mainMobile)
        {
            if (PhoneUtility.ValidateLocalNumber(mainMobile))
                mainMobile = PhoneUtility.LocalNumberToInternational(mainMobile, 98);
            var user = Repository.Query(q => q.FirstOrDefault(f => f.MainMobile == mainMobile));
            if (user == null)
            {
                user = new User();
            }
            return user;
        }

        public User GetActivatedUserByMainMobile(string mainMobile, bool includeFavorite = false)
        {
            var identityUser = userManager.FindByNameAsync(mainMobile).Result;
            if (identityUser != null &&
                (identityUser.State == User.UserState.Acticved || identityUser.State == User.UserState.ReserveBanned))
            {
                if (includeFavorite)
                {
                    return Repository.Query(q => q.Include(i => i.Favorite).FirstOrDefault(
                        f => f.MainMobile == mainMobile));
                }
                return Repository.Query(q => q.FirstOrDefault(
                    f => f.MainMobile == mainMobile));
            }
            return null;
        }

        public User GetActivatedUserByEmail(string email, bool includeFavorite = false)
        {
            var identityUser = userManager.FindByEmailAsync(email).Result;
            if (identityUser != null && identityUser.State == User.UserState.Acticved)
            {
                if (includeFavorite)
                {
                    return Repository.Query(q => q.Include(i => i.Favorite).FirstOrDefault(
                        f => f.MainMobile == identityUser.UserName));
                }
                return Repository.Query(q => q.FirstOrDefault(
                    f => f.MainMobile == identityUser.UserName));
            }
            return null;
        }

        public void Insert(User user, int currentUserId = 0, ActionLog.ActionSourceEnum source = ActionLog.ActionSourceEnum.AdminPanel)
        {
            Repository.Insert(user);
            Repository.Save();
            if (currentUserId > 0)
            {
                mediator.Publish(new UserUpdateEvent(null, user, source, currentUserId));
            }
        }

        public async Task<AppUser> RegisterAsync(LoginRequest request)
        {
            var identityUser = await userManager.FindByNameAsync(request.phoneNumber);
            if (identityUser != null)
            {
                return null;
            }

            var verifyCode = new Random().Next(1111, 9999).ToString();
            identityUser = new AppUser()
            {
                UserName = request.phoneNumber,
                PhoneNumber = request.phoneNumber,
                CreateDate = DateTime.Now,
                State = User.UserState.InActived,
                Code = verifyCode,
                SendVerification = DateTime.Now,
                Email = request.email
            };
            var result = await userManager.CreateAsync(identityUser);

            if (result.Succeeded)
            {
                var user = new User()
                {
                    Mobile = request.phoneNumber,
                    MainMobile = request.phoneNumber,
                    AmlakbashiScore = 1000
                };
                Repository.Insert(user);
                Repository.Save();
                if (string.IsNullOrEmpty(request.referralCode) == false)
                {
                    SetReferralCode(user.Id, int.Parse(request.referralCode));
                }
                return identityUser;
            }
            return null;
        }

        public async Task UpdatePhoneNumberConfirmedAsync(string guid, bool confirm)
        {
            var identityUser = await userManager.FindByIdAsync(guid);
            identityUser.PhoneNumberConfirmed = confirm;
            await userManager.UpdateAsync(identityUser);
        }

        public async Task UpdateEmailConfirmedAsync(string guid, bool confirm)
        {
            var identityUser = await userManager.FindByIdAsync(guid);
            identityUser.EmailConfirmed = confirm;
            await userManager.UpdateAsync(identityUser);
        }

        public void SetReferralCode(int userId, int referralUserId)
        {
            var referralUser = Repository.Find(referralUserId);
            if (referralUser != null)
            {
                var user = Repository.Find(userId);
                user.PresentorUserID = referralUserId;
                Repository.Update(user);
                Repository.Save();
                mediator.Send(new AddDiscountCouponCommand(user.Id, user.PresentorUserID,
                    5, DiscountCoupon.DiscountCouponType.Present));
                var contact = new UserContactDTO()
                {
                    UserMainMobile = user.MainMobile,
                    UserAppNotificationToken = user.AppNotificationToken,
                    UserEmail = "",
                    EmailConfirmed = false,
                    UserFcmAppNotificationToken = user.FcmAppNotificationToken,
                    UserNotificationToken = user.NotificationToken,
                    Type = UserContactType.CouponPresent,
                    Extra1 = referralUser.FullName,
                    Extra2 = "5%"
                };
                mediator.Enqueue(new SendMessageCommand(contact));
            }
        }

        public void SendVerifyCode(AppUser identityUser)
        {
            var isIranNumber = PhoneUtility.IsNumberForIran(identityUser.PhoneNumber);
            if (isIranNumber)
            {
                var callableNumber = PhoneUtility.InternationalNumberToLocal(identityUser.PhoneNumber);
                SendVerificationSms(callableNumber, identityUser.Code);
            }
            else
            {
                string strbody = $"<div style='direction:rtl;text-align:right;'><div>کد ورود شما در املاک باشی: {identityUser.Code}</div></div>";
#if !DEBUG
                EmailUtility.SendEmail(EmailSenderDepartment.Verification, new List<string>() { identityUser.Email }, "تایید ایمیل", strbody);
#endif
            }
        }

        public bool Update(UserEditDTO editedUser, int adminId)
        {
            var user = Repository.Find(editedUser.Id);
            var shallowUser = user.ShallowCopy();

            var identityUser = GetIdentityUser(user.MainMobile);
            if (identityUser.State != editedUser.UserState)
            {
                identityUser.State = editedUser.UserState;
                UpdateIdentityUser(identityUser);
            }

            user.FName = editedUser.FName;
            user.LName = editedUser.LName;
            user.OwnerShip = editedUser.OwnerShip;
            user.ContactPhone = editedUser.ContactPhone ? "1" : null;
            user.AmlakbashiScore = editedUser.AmlakbashiScore;
            user.Address = editedUser.Address;
            user.ForbiddenRegionsAccess = editedUser.ForbiddenRegionsAccess;
            user.Mobile = editedUser.Mobile;
            user.Mobile2 = editedUser.Mobile2;
            user.Tell = editedUser.Tell;
            user.ThirdPersonTell = editedUser.ThirdPersonTell;

            if (editedUser.CancelInstantReserveLimit > 0 &&
                editedUser.CancelInstantReserveLimit != user.CancelInstantReserveLimit)
            {
                user.CancelInstantReserveLimit = editedUser.CancelInstantReserveLimit;
                if (user.Advertises.Sum(x => x.InstantReserveCancels) > user.CancelInstantReserveLimit)
                {
                    user.InstantReserveAccess = User.InstantReserveAccessEnum.Banned;
                    foreach (var item in user.Advertises)
                    {
                        item.InstantReserveStatus = Advertise.InstantReserveStatusEnum.None;
                    }
                }
                else
                {
                    if (user.InstantReserveAccess == User.InstantReserveAccessEnum.Banned)
                    {
                        user.InstantReserveAccess = User.InstantReserveAccessEnum.None;
                    }
                }
            }
            Repository.Update(user);
            Repository.Save();
            mediator.Publish(new UserUpdateEvent(shallowUser, user, ActionLog.ActionSourceEnum.AdminPanel, adminId));
            return true;
        }

        public bool Update(UserDTO dto, int currentUserId, bool userHasRefunedInProgress,
            ActionLog.ActionSourceEnum source, out List<string> errors)
        {
            if (dto.Validate(out errors) == false)
                return false;
            var user = Repository.Find(dto.id);
            var shallowUser = user.ShallowCopy();
            user.FName = dto.fname;
            user.LName = dto.lname;
            if (dto.mobile1.Substring(0, 2) == "00")
            {
                var corrected = PhoneUtility.CorrectPhoneNumberIfPossible(dto.mobile1);
                user.SetPhoneNumber(User.PhoneType.OtherMobile1, corrected);
            }
            else
            {
                user.SetLocalPhoneNumber(User.PhoneType.OtherMobile1, dto.mobile1, 98);
            }
            if (!string.IsNullOrEmpty(dto.mobile2))
            {
                if (dto.mobile2.Substring(0, 2) == "00")
                {
                    var corrected = PhoneUtility.CorrectPhoneNumberIfPossible(dto.mobile2);
                    user.SetPhoneNumber(User.PhoneType.OtherMobile2, corrected);
                }
                else
                {
                    user.SetLocalPhoneNumber(User.PhoneType.OtherMobile2, dto.mobile2, 98);
                }
            }
            if (!string.IsNullOrEmpty(dto.tell))
            {
                if (dto.tell.Substring(0, 2) == "00")
                {
                    var corrected = PhoneUtility.CorrectPhoneNumberIfPossible(dto.tell);
                    user.SetPhoneNumber(User.PhoneType.LandLine, corrected);
                }
                else
                {
                    user.SetLocalPhoneNumber(User.PhoneType.LandLine, dto.tell, 98);
                }
            }
            if (!string.IsNullOrEmpty(dto.thirdPersonTell))
            {
                if (dto.thirdPersonTell.Substring(0, 2) == "00")
                {
                    var corrected = PhoneUtility.CorrectPhoneNumberIfPossible(dto.thirdPersonTell);
                    user.SetPhoneNumber(User.PhoneType.ThirdPerson, corrected);
                }
                else
                {
                    user.SetLocalPhoneNumber(User.PhoneType.ThirdPerson, dto.thirdPersonTell, 98);
                }
            }

            var bankCardObj = user.BankCards == null || user.BankCards.Any() == false ? null : user.BankCards.FirstOrDefault();

            var hasChange = ((bankCardObj == null && dto.shabaNumber == null &&
                dto.bankCardNumber == null && dto.bankFname == null && dto.bankLname == null) ||
                (bankCardObj != null && dto.shabaNumber == bankCardObj.ShabaNumber &&
                dto.bankCardNumber == bankCardObj.BankCardNumber &&
                dto.bankFname == bankCardObj.FName && dto.bankLname == bankCardObj.LName)) == false;

            if (hasChange)
            {
                if (dto.shabaNumber != null)
                {
                    dto.shabaNumber = StringUtility.PersianNumberToEnglish(dto.shabaNumber);
                }
                if (dto.bankCardNumber != null)
                {
                    dto.bankCardNumber = StringUtility.PersianNumberToEnglish(dto.bankCardNumber);
                }
                var bankCard = new BankCard()
                {
                    UserID = (int)dto.id,
                    BankCardNumber = dto.bankCardNumber,
                    ShabaNumber = dto.shabaNumber,
                    FName = dto.bankFname,
                    LName = dto.bankLname,
                    CreateDate = bankCardObj == null ? DateTime.Now : bankCardObj.CreateDate,
                    LastModifyDate = DateTime.Now,
                    BankCardStatus = (int)BankCard.BankCardStatusEnum.NotVerified,
                    ShabaStatus = (int)BankCard.BankCardStatusEnum.NotVerified,
                    Id = bankCardObj == null ? 0 : bankCardObj.Id
                };
                if (bankCardObj != null)
                {
                    var shallowBankCard = bankCardObj.ShallowCopy();
                    bankCardObj.BankCardStatus = (int)BankCard.BankCardStatusEnum.NotVerified;
                    bankCardObj.ShabaStatus = (int)BankCard.BankCardStatusEnum.NotVerified;
                    bankCardObj.FName = bankCard.FName;
                    bankCardObj.LName = bankCard.LName;
                    bankCardObj.BankCardNumber = bankCard.BankCardNumber;
                    bankCardObj.ShabaNumber = bankCard.ShabaNumber;
                    bankCardObj.LastModifyDate = DateTime.Now;
                    mediator.Publish(new BankCardUpdateEvent(shallowBankCard, bankCardObj,
                        source, currentUserId));
                }
                else
                {
                    bankCard.CreateDate = DateTime.Now;
                    bankCard.LastModifyDate = DateTime.Now;
                    bankCard.BankCardStatus = (int)BankCard.BankCardStatusEnum.NotVerified;
                    bankCard.ShabaStatus = (int)BankCard.BankCardStatusEnum.NotVerified;
                    user.BankCards.Add(bankCard);
                    mediator.Publish(new BankCardUpdateEvent(null, bankCard,
                        source, currentUserId));
                }
            }
            Repository.Update(user);
            Repository.Save();
            mediator.Publish(new UserUpdateEvent(shallowUser, user, source, currentUserId));
            return true;
        }

        public async Task<bool> UpdateAsync(UserPutProfileRequest request)
        {
            var user = Repository.Find(request.id);
            var shallowUser = user.ShallowCopy();
            user.FName = request.firstName;
            user.LName = request.lastName;
            user.Mobile = PhoneUtility.CorrectPhoneNumberIfPossible(request.phoneNumber2);
            user.Mobile2 = PhoneUtility.CorrectPhoneNumberIfPossible(request.phoneNumber3);
            user.Tell = PhoneUtility.CorrectPhoneNumberIfPossible(request.landLinePhoneNumber);
            user.ThirdPersonTell = PhoneUtility.CorrectPhoneNumberIfPossible(request.thirdPersonPhoneNumber);

            var identityUser = GetIdentityUser(user.MainMobile);
            identityUser.Email = request.email;
            await UpdateIdentityUserAsync(identityUser);

            var bankCard = user.BankCards == null || user.BankCards.Any() == false ? null : user.BankCards.FirstOrDefault();
            var hasBankCardChanged = ((bankCard == null && request.shebaNumber == null &&
                request.bankCardNumber == null && request.bankCardOwnerFirstName == null && request.bankCardOwnerLastName == null) ||
                (bankCard != null && request.shebaNumber == bankCard.ShabaNumber &&
                request.bankCardNumber == bankCard.BankCardNumber &&
                request.bankCardOwnerFirstName == bankCard.FName && request.bankCardOwnerLastName == bankCard.LName)) == false;

            if (hasBankCardChanged)
            {
                if (request.shebaNumber != null)
                {
                    request.shebaNumber = StringUtility.PersianNumberToEnglish(request.shebaNumber);
                }
                if (request.bankCardNumber != null)
                {
                    request.bankCardNumber = StringUtility.PersianNumberToEnglish(request.bankCardNumber);
                }

                if (bankCard != null)
                {
                    var shallowBankCard = bankCard.ShallowCopy();
                    bankCard.BankCardStatus = (int)BankCard.BankCardStatusEnum.NotVerified;
                    bankCard.ShabaStatus = (int)BankCard.BankCardStatusEnum.NotVerified;
                    bankCard.FName = request.bankCardOwnerFirstName;
                    bankCard.LName = request.bankCardOwnerLastName;
                    bankCard.BankCardNumber = request.bankCardNumber;
                    bankCard.ShabaNumber = request.shebaNumber;
                    bankCard.LastModifyDate = DateTime.Now;
                    await mediator.Publish(new BankCardUpdateEvent(shallowBankCard, bankCard,
                        ActionLog.ActionSourceEnum.WebsiteDashboard, user.Id));
                }
                else
                {
                    var newBankCard = new BankCard()
                    {
                        UserID = request.id,
                        BankCardNumber = request.bankCardNumber,
                        ShabaNumber = request.shebaNumber,
                        FName = request.bankCardOwnerFirstName,
                        LName = request.bankCardOwnerLastName,
                        CreateDate = DateTime.Now,
                        LastModifyDate = DateTime.Now,
                        BankCardStatus = (int)BankCard.BankCardStatusEnum.NotVerified,
                        ShabaStatus = (int)BankCard.BankCardStatusEnum.NotVerified
                    };
                    user.BankCards.Add(newBankCard);
                    await mediator.Publish(new BankCardUpdateEvent(null, newBankCard,
                        ActionLog.ActionSourceEnum.WebsiteDashboard, user.Id));
                }
            }
            Repository.Update(user);
            Repository.Save();
            await mediator.Publish(new UserUpdateEvent(shallowUser, user, ActionLog.ActionSourceEnum.WebsiteDashboard, user.Id));
            return true;
        }

        public void UpdateState(int userId, bool state, int currentUserId = 0,
            ActionLog.ActionSourceEnum source = ActionLog.ActionSourceEnum.AdminPanel)
        {
            var user = Repository.Query(q => q.FirstOrDefault(f => f.Id == userId));
            var identityUser = userManager.FindByNameAsync(user.MainMobile).Result;
            if (state)
            {
                identityUser.State = User.UserState.Acticved;
            }
            else
            {
                identityUser.State = User.UserState.InActived;
            }
            userManager.UpdateAsync(identityUser).Wait();
        }

        public void UpdateContactPhone(int userId, bool state)
        {
            var user = Repository.Query(q => q.FirstOrDefault(f => f.Id == userId));
            if (state)
            {
                user.ContactPhone = "1";
            }
            else
            {
                user.ContactPhone = null;
            }
            Repository.Update(user);
            Repository.Save();
        }

        public void UpdateProfilePhoto(int userId, long photoId, User.UserPhotoState state)
        {
            var user = Repository.Query(q => q.FirstOrDefault(f => f.Id == userId));
            user.PhotoID = photoId;
            user.PhotoStatus = (int)state;
            Repository.Update(user);
            Repository.Save();
        }

        public void UpdatePhotoStatus(int userId, User.UserPhotoState state, int currentUserId = 0,
            ActionLog.ActionSourceEnum source = ActionLog.ActionSourceEnum.AdminPanel)
        {
            var user = Repository.Query(q => q.FirstOrDefault(f => f.Id == userId));
            var shallowUser = user.ShallowCopy();
            user.PhotoStatus = (int)state;
            Repository.Update(user);
            Repository.Save();
            if (currentUserId > 0)
            {
                mediator.Publish(new UserUpdateEvent(shallowUser, user, source, currentUserId));
            }
        }

        public void UpdateCreateDate(int userId, DateTime time)
        {
            var user = Repository.Query(q => q.FirstOrDefault(f => f.Id == userId));
            var identityUser = userManager.FindByNameAsync(user.MainMobile).Result;
            identityUser.CreateDate = time;
            userManager.UpdateAsync(identityUser).Wait();
        }

        public void UpdateSendVerification(int userId, DateTime time, string code = null)
        {
            var user = Repository.Query(q => q.FirstOrDefault(f => f.Id == userId));
            var identityUser = userManager.FindByNameAsync(user.MainMobile).Result;
            identityUser.SendVerification = time;
            if (code != null)
            {
                identityUser.Code = code;
            }
            userManager.UpdateAsync(identityUser).Wait();
        }

        public async Task<string> UpdateVerifyCodeAsync(string guid)
        {
            var identityUser = await userManager.FindByIdAsync(guid);
            if (identityUser != null)
            {
                var newCode = new Random().Next(1111, 9999).ToString();
                identityUser.Code = newCode;
                identityUser.SendVerification = DateTime.Now;
                var result = await userManager.UpdateAsync(identityUser);
                if (result.Succeeded)
                {
                    return newCode;
                }
            }
            return null;
        }

        public void UpdatePresentorUser(int userId, int pid)
        {
            var user = Repository.Query(q => q.FirstOrDefault(f => f.Id == userId));
            user.PresentorUserID = pid;
            Repository.Update(user);
            Repository.Save();
        }

        public void UpdateFNameLName(int userId, string newFName, string newLName)
        {
            var user = Repository.Query(q => q.FirstOrDefault(f => f.Id == userId));
            user.FName = newFName;
            user.LName = newLName;
            Repository.Update(user);
            Repository.Save();
        }

        public async Task UpdateEmailAsync(string guid, string email, bool confirm)
        {
            var identityUser = await userManager.FindByIdAsync(guid);
            identityUser.Email = email;
            identityUser.EmailConfirmed = confirm;
            await userManager.UpdateAsync(identityUser);
        }

        public void UpdateDesc(int userId, string desc)
        {
            var user = Repository.Query(q => q.FirstOrDefault(f => f.Id == userId));
            user.Address = desc;
            Repository.Update(user);
            Repository.Save();
        }

        public void UpdateLastNotifPermetionTicks(int userId, long ticks)
        {
            var user = Repository.Query(q => q.FirstOrDefault(f => f.Id == userId));
            user.LastNotifPermitionTicks = ticks;
            Repository.Update(user);
            Repository.Save();
        }

        public void UpdateInstantReserveAccess(int userId, User.InstantReserveAccessEnum instantReserveAccess,
            int currentUserId = 0, ActionLog.ActionSourceEnum source = ActionLog.ActionSourceEnum.AdminPanel)
        {
            mediator.Send(new ChangeInstantReserveAccessCommand(userId,
                instantReserveAccess, currentUserId, source));
        }

        public void UpdateUserNotificationToken(int userId, string token)
        {
            IQueryable<User> users = Repository.Query(q => q);
            var user = users.FirstOrDefault(x => x.Id == userId);
            if (token == "null")
            {
                token = null;
            }
            if (user != null && user.NotificationToken != token)
            {
                if (!string.IsNullOrEmpty(token))
                {
                    var usersWithSameToken = users.Where(x => x.NotificationToken == token);
                    foreach (var item in usersWithSameToken)
                    {
                        item.NotificationToken = null;
                        Repository.Update(item);
                    }
                }
                user.NotificationToken = token;
                Repository.Update(user);
                Repository.Save();
            }
        }

        public void UpdateFcmNotificationToken(int userId, string token)
        {
            IQueryable<User> users = Repository.Query(q => q);
            var user = users.FirstOrDefault(x => x.Id == userId);
            if (user.FcmAppNotificationToken != token)
            {
                if (!string.IsNullOrEmpty(token))
                {
                    var usersWithSameToken = users.Where(x => x.FcmAppNotificationToken == token);
                    foreach (var item in usersWithSameToken)
                    {
                        item.FcmAppNotificationToken = null;
                        Repository.Update(item);
                    }
                }
                user.FcmAppNotificationToken = token;
                Repository.Update(user);
                Repository.Save();
            }
        }

        public void UpdateAppNotificationToken(int userId, string token)
        {
            IQueryable<User> users = Repository.Query(q => q);
            var user = users.FirstOrDefault(x => x.Id == userId);
            if (user.AppNotificationToken != token)
            {
                if (!string.IsNullOrEmpty(token))
                {
                    var usersWithSameToken = users.Where(x => x.AppNotificationToken == token);
                    foreach (var item in usersWithSameToken)
                    {
                        item.AppNotificationToken = null;
                        Repository.Update(item);
                    }
                }
                user.AppNotificationToken = token;
                Repository.Update(user);
                Repository.Save();
            }
        }

        public void UpdateUserGeneralType(int userId, User.UserGeneralTypeEnum userGeneralType)
        {
            var user = Repository.Query(q => q.FirstOrDefault(f => f.Id == userId));
            user.UserGeneralType = (int)userGeneralType;
            Repository.Update(user);
            Repository.Save();
        }

        public void Delete(int userId, int currentUserId = 0,
            ActionLog.ActionSourceEnum source = ActionLog.ActionSourceEnum.AdminPanel)
        {
            var user = Repository.Query(q => q.FirstOrDefault(f => f.Id == userId));
            var shallowUser = user.ShallowCopy();
            user.IsDeleted = true;
            Repository.Delete(userId);
            Repository.Save();
            if (currentUserId > 0)
            {
                mediator.Publish(new UserUpdateEvent(shallowUser, user, source, currentUserId));
            }
        }

        public void AddFavorite(int userId, long advertiseId)
        {
            var user = Repository.Find(userId);
            if (user.Favorite.Any(x => x.AdvertiseID == advertiseId))
            {
                return;
            }
            var favorite = new UserFavorite();
            favorite.AdvertiseID = advertiseId;
            favorite.SetDate = DateTime.Now;
            user.Favorite.Add(favorite);
            Repository.Update(user);
            Repository.Save();
        }

        public bool DeleteFavorite(int userId, long advertiseId)
        {
            var user = Repository.Find(userId);
            var favorite = user.Favorite.FirstOrDefault(f => f.AdvertiseID == advertiseId);
            if (favorite == null)
            {
                return false;
            }
            user.Favorite.Remove(favorite);
            Repository.Update(user);
            Repository.Save();
            return true;
        }

        public void SendVerificationSms(string localNumber, string code)
        {
            mediator.Enqueue(new SendVerificationSmsCommand(localNumber, code));
        }

        public void SendNotificationApplication(string token, string title, string body, string targetAction, string targetId)
        {
            userContact.SendNotificationApplication(token, title, body, targetAction, targetId);
        }

        public void SendCustomSms(int delay, string mobile, string template)
        {
            mediator.Schedule(new ScheduleSendCustomSms(delay, mobile, template), new TimeSpan(0, 0, delay));
        }

        public void SendGroupNotification(List<string> tokens, string title, string body, string clickAction)
        {
            mediator.Enqueue(new ScheduleSendGroupNotificationCommand(tokens, title, body, clickAction));
        }

        public bool VerifyLogin(string mobile, out int user_id, string presentorCode, out string errorMsg)
        {
            var identityUser = userManager.FindByNameAsync(mobile).Result;
            if (identityUser != null)
            {
                var user = Repository.Query(q => q.OrderBy(u => u.Id).FirstOrDefault(u => u.MainMobile == mobile));
                if (identityUser.State == User.UserState.InActived)
                {
                    user.AmlakbashiScore = 1000;
                    if (!string.IsNullOrEmpty(presentorCode))
                    {
                        int prId = 0;
                        int.TryParse(presentorCode, out prId);
                        var prUser = Repository.Find(prId);
                        if (prUser == null)
                        {
                            errorMsg = "کد معرف اشتباه است";
                            user_id = user.Id;
                            return false;
                        }
                        user.PresentorUserID = prUser.Id;
                        mediator.Send(new AddDiscountCouponCommand(user.Id, user.PresentorUserID,
                            5, DiscountCoupon.DiscountCouponType.Present));
                        var contact = new UserContactDTO()
                        {
                            UserMainMobile = user.MainMobile,
                            UserAppNotificationToken = user.AppNotificationToken,
                            UserEmail = identityUser.Email,
                            EmailConfirmed = identityUser.EmailConfirmed,
                            UserFcmAppNotificationToken = user.FcmAppNotificationToken,
                            UserNotificationToken = user.NotificationToken,
                            Type = UserContactType.CouponPresent,
                            Extra1 = prUser.FullName,
                            Extra2 = "5%"
                        };
                        mediator.Enqueue(new SendMessageCommand(contact));
                    }
                }
                Repository.Update(user);
                Repository.Save();
                user_id = user.Id;
                errorMsg = "";
                return true;
            }
            user_id = 0;
            errorMsg = "خطا در کد فعالسازی، لطفا کد را مجددا بررسی کرده و دوباره وارد کنید";
            return false;
        }

        public void SendMessage(UserContactDTO userContact)
        {
            mediator.Enqueue(new SendMessageCommand(userContact));
        }

        public void SendSms(UserContactDTO userContact)
        {
            mediator.Enqueue(new SendSmsCommand(userContact));
        }

        public IList<string> GetAllIdentityUsernamesByState(User.UserState state = User.UserState.Acticved)
        {
            return userManager.Users.Where(w => w.State == state).Select(s => s.UserName).ToList();
        }

        public AppUser GetIdentityUser(string phrase, bool isEmail = false)
        {
            AppUser user;
            if (isEmail)
            {
                user = userManager.FindByEmailAsync(phrase).Result;
            }
            else
            {
                user = userManager.FindByNameAsync(phrase).Result;
            }
            return user;
        }

        public async Task<AppUser> GetIdentityUserByIdAsync(string id)
        {
            return await userManager.FindByIdAsync(id);
        }

        public void AddIdentityUser(AppUser user)
        {
            userManager.CreateAsync(user).Wait();
        }

        public void UpdateIdentityUser(AppUser user)
        {
            userManager.UpdateAsync(user).Wait();
        }

        public async Task<bool> UpdateIdentityUserAsync(AppUser user)
        {
            var result = await userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        public IdentityResult ChangeIdentityUserPassword(string username, string password)
        {
            var user = userManager.FindByNameAsync(username).Result;
            var result = passwordValidator.ValidateAsync(userManager, user, password).Result;
            if (result.Succeeded == false)
            {
                return result;
            }
            userManager.RemovePasswordAsync(user).Wait();
            return userManager.AddPasswordAsync(user, password).Result;
        }

        public IdentityResult ChangeIdentityUserPassword(string username, string currentPassword, string newPassword)
        {
            var user = userManager.FindByNameAsync(username).Result;
            return userManager.ChangePasswordAsync(user, currentPassword, newPassword).Result;
        }

        public IdentityResult ChangePassword(string username, string currentPassword, string newPassword)
        {
            var user = userManager.FindByNameAsync(username).Result;
            if (userManager.HasPasswordAsync(user).Result)
            {
                return userManager.ChangePasswordAsync(user, currentPassword, newPassword).Result;
            }
            return userManager.AddPasswordAsync(user, newPassword).Result;
        }

        public IList<AppRole> GetAllRoles()
        {
            var roles = roleManager.Roles;
            return roles.ToList();
        }

        public IList<string> GetAllRoleNames()
        {
            var roles = roleManager.Roles.Select(s => s.Name);
            return roles.ToList();
        }

        public IList<string> GetUserRoles(string username)
        {
            var user = userManager.FindByNameAsync(username).Result;
            var roles = userManager.GetRolesAsync(user).Result;
            return roles;
        }

        public bool AddClaimsToUser(string username, IList<Claim> claims)
        {
            var user = userManager.FindByNameAsync(username).Result;
            var userClaims = userManager.GetClaimsAsync(user).Result;
            if (userClaims != null && userClaims.Where(w => claims.Select(s => s.Type).Contains(w.Type)).Count() > 0)
            {
                var expireTime = DateTime.Parse(userClaims.FirstOrDefault(f => f.Type == "ImpersonateExpireTime").Value);
                if (expireTime > DateTime.Now)
                {
                    return false;
                }
                userManager.RemoveClaimsAsync(user, userClaims).Wait();
            }
            return userManager.AddClaimsAsync(user, claims).Result.Succeeded;
        }

        public void RemoveClaimsFromUser(string username, IList<Claim> claims)
        {
            var user = userManager.FindByNameAsync(username).Result;
            userManager.RemoveClaimsAsync(user, claims).Wait();
        }

        public void UpdateUserRoles(string username, IList<string> selectedRoles)
        {
            if (selectedRoles.Contains("SuperAdmin"))
            {
                return;
            }
            var user = userManager.FindByNameAsync(username).Result;
            var allUserRoles = userManager.GetRolesAsync(user).Result;
            var addedRoles = selectedRoles.Where(s => allUserRoles.Contains(s) == false);
            var removedRoles = allUserRoles.Where(w => selectedRoles.Contains(w) == false);
            userManager.AddToRolesAsync(user, addedRoles).Wait();
            userManager.RemoveFromRolesAsync(user, removedRoles).Wait();
        }

        public IList<User> GetRoleUserList(string roleName)
        {
            var identityUsers = userManager.GetUsersInRoleAsync(roleName).Result.Select(s => s.UserName);
            var userList = Repository.Query(q => q.Where(w => identityUsers.Contains(w.MainMobile)));
            return userList.ToList();
        }

        public bool SignInRegister(int user_id, string fname, string lname,
            out Dictionary<string, string> errors)
        {
            var user = Repository.Query(q => q.FirstOrDefault(f => f.Id == user_id));
            var identityUser = userManager.FindByNameAsync(user.MainMobile).Result;
            errors = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(fname))
            {
                if (StringUtility.ContainsNumber(fname))
                {
                    errors.Add(nameof(fname), "نام نمیتواند شامل عدد باشد");
                }
                else
                {
                    user.FName = fname;
                    Repository.Update(user);
                    Repository.Save();
                }
            }
            else
            {
                errors.Add(nameof(fname), "لطفا نام خود را وارد کنید");
            }
            if (!string.IsNullOrEmpty(lname))
            {
                if (StringUtility.ContainsNumber(lname))
                {
                    errors.Add(nameof(lname), "نام خانوادگی نمیتواند شامل عدد باشد");
                }
                else
                {
                    user.LName = lname;
                    Repository.Update(user);
                    Repository.Save();
                }
            }
            else
            {
                errors.Add(nameof(lname), "لطفا نام خانوادگی خود را وارد کنید");
            }
            return errors.Any() == false;
        }

        public bool SignInRegisterOld(int user_id, string fname, string lname,
            string password, string confirmPassword, out Dictionary<string, string> errors)
        {
            var user = Repository.Query(q => q.FirstOrDefault(f => f.Id == user_id));
            var identityUser = userManager.FindByNameAsync(user.MainMobile).Result;
            errors = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(fname))
            {
                if (StringUtility.ContainsNumber(fname))
                {
                    errors.Add(nameof(fname), "نام نمیتواند شامل عدد باشد");
                }
                else
                {
                    user.FName = fname;
                    Repository.Update(user);
                    Repository.Save();
                }
            }
            else
            {
                errors.Add(nameof(fname), "لطفا نام خود را وارد کنید");
            }
            if (!string.IsNullOrEmpty(lname))
            {
                if (StringUtility.ContainsNumber(lname))
                {
                    errors.Add(nameof(lname), "نام خانوادگی نمیتواند شامل عدد باشد");
                }
                else
                {
                    user.LName = lname;
                    Repository.Update(user);
                    Repository.Save();
                }
            }
            else
            {
                errors.Add(nameof(lname), "لطفا نام خانوادگی خود را وارد کنید");
            }
            if (string.IsNullOrEmpty(password) == false)
            {
                if (Regex.IsMatch(password, "[^\u0000-\u0080]+"))
                {
                    errors.Add(nameof(password) + 0, "رمز عبور نباید شامل حروف فارسی باشد");
                }
                else if (password != confirmPassword)
                {
                    errors.Add(nameof(confirmPassword), "رمز وارد شده و تاییدیه آن یکسان نمی باشند");
                }
                else
                {
                    userManager.RemovePasswordAsync(identityUser).Wait();
                    var addPasswordResult = userManager.AddPasswordAsync(identityUser, password).Result;
                    if (addPasswordResult.Succeeded == false)
                    {
                        int i = 0;
                        foreach (var addPasswordError in addPasswordResult.Errors)
                        {
                            i++;
                            errors.Add(nameof(password) + i, UserLocalization.GetIdentityPasswordErrorString(addPasswordError.Code,
                                addPasswordError.Description));
                        }
                    }
                }
            }
            else
            {
                errors.Add(nameof(password), "لطفا رمز عبور خود را وارد کنید");
            }
            return errors.Any() == false;
        }

        public JwtSecurityToken JwtSignIn(AppUser identityUser, byte[] key, int userGeneralType = 0)
        {
            var userRoles = userManager.GetRolesAsync(identityUser).Result;
            var authClaims = new List<Claim>();
            foreach (var role in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            }
            authClaims.Add(new Claim(ClaimTypes.NameIdentifier, identityUser.Id));
            authClaims.Add(new Claim(ClaimTypes.Name, identityUser.UserName));
            authClaims.Add(new Claim("AspNet.Identity.SecurityStamp", userManager.GetSecurityStampAsync(identityUser).Result));
            authClaims.Add(new Claim("type", userGeneralType == 0 ? "guest" : "host"));

            var authSigningKey = new SymmetricSecurityKey(key);
            var token = new JwtSecurityToken(
                    claims: authClaims,
                    expires: DateTime.Now.AddHours(1440),
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256));
            return token;
        }

        public async Task<string> GenerateJwtTokenAsync(string guid, string jwtSecret,
            User.UserGeneralTypeEnum? panel = null)
        {
            var identityUser = await userManager.FindByIdAsync(guid);
            var user = GetByMainMobile(identityUser.UserName);
            var userRoles = await userManager.GetRolesAsync(identityUser);
            panel = panel != null ? panel.Value : (User.UserGeneralTypeEnum)user.UserGeneralType;
            var claims = new List<Claim>();

            foreach (var role in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            claims.Add(new Claim(ClaimTypes.NameIdentifier, identityUser.Id));
            claims.Add(new Claim(ClaimTypes.Name, identityUser.UserName));
            claims.Add(new Claim("refreshToken", identityUser.SecurityStamp));
            claims.Add(new Claim("panel", panel == User.UserGeneralTypeEnum.Guest ? "guest" : "host"));

            var symmetricKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSecret));
            var token = new JwtSecurityToken(
                    issuer: GeneralData.WebsiteUrl,
                    audience: GeneralData.WebsiteUrl,
                    claims: claims,
                    expires: DateTime.Now.AddDays(30),
                    signingCredentials: new SigningCredentials(symmetricKey, SecurityAlgorithms.HmacSha256));
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public ClaimsPrincipal GetPrincipalFromJwtToken(string token, string jwtSecret)
        {
            try
            {
                var tokenValidationParameters = TokenUtility.GetTokenValidationParameters(jwtSecret, false);
                var tokenHandler = new JwtSecurityTokenHandler();
                SecurityToken securityToken;
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
                var jwtSecurityToken = securityToken as JwtSecurityToken;
                if (jwtSecurityToken == null ||
                    jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase) == false)
                {
                    return null;
                }
                return principal;
            }
            catch
            {
                return null;
            }
        }

        public IEnumerable<User> IdentityUsersToUsers(IEnumerable<AppUser> identityUsers)
        {
            var mainMobiles = identityUsers.Select(s => s.PhoneNumber).ToList();
            return Repository.Query(q => q.Where(w => mainMobiles.Contains(w.MainMobile)));
        }

        public IEnumerable<AppUser> GetAllSupportEmployees()
        {
            var supportRoles = Roles.SupportRoles;
            IEnumerable<AppUser> result = new List<AppUser>();
            foreach (var role in supportRoles)
            {
                result = result.Concat(userManager.GetUsersInRoleAsync(role).Result);
            }
            result = result.Distinct();
            return result;
        }

        public IEnumerable<AppUser> GetAllEmployees()
        {
            var employeeRoles = Roles.AllEmployeeRoles;
            IEnumerable<AppUser> result = new List<AppUser>();
            foreach (var role in employeeRoles)
            {
                result = result.Concat(userManager.GetUsersInRoleAsync(role).Result);
            }
            result = result.Distinct();
            return result;
        }

        public bool UserAllowPolicy(AppUser identityUser, string policy)
        {
            var roles = userManager.GetRolesAsync(identityUser).Result;
            var allowPolicy = PolicyData.AllPolicies[policy].ToList().Intersect(roles).Any();
            return allowPolicy;
        }

        public void SignOut()
        {
            signInManager.SignOutAsync().Wait();
            mediator.Send(new DeleteImpersonationCookiesCommand());
        }
    }
}
