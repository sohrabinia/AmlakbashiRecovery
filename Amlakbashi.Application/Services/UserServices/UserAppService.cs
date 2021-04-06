using Amlakbashi.Core.Common.AppService;
using Amlakbashi.Core.Common.Repository;
using Amlakbashi.Core.Common.Caching;
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
using log4net;
using Microsoft.AspNetCore.Identity;
using Amlakbashi.Data.Identity;
using System.Threading.Tasks;

namespace Amlakbashi.Application.Services.UserServices
{
    internal class UserAppService : AppServiceBase<User, int>, Interfaces.IUserAppService
    {
        private readonly IMediator mediator;
        private readonly IUserContactFacade userContact;
        private readonly UserManager<AppUser> userManager;
        private readonly ILog logger;
        public UserAppService(IRepository<User, int> repository, ICacheManager<User> cache,
            IUserContactFacade userContact,
            IMediator mediator,
            UserManager<AppUser> userManager,
            ILog logger) : base(repository, cache)
        {
            this.mediator = mediator;
            this.userContact = userContact;
            this.userManager = userManager;
            this.logger = logger;
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
                return Repository.Query(q => q.Where(u => u.CreateDate >= fromDate &&
                    u.CreateDate <= toDate).Count());
            }
            return Repository.Query(q => q.Where(u => u.CreateDate >= fromDate &&
                u.CreateDate <= toDate && userList.Contains(u.Id)).Count());
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

        public User GetByAdminLoginCode(string code)
        {
            return Repository.Query(q => q.FirstOrDefault(x => x.AdminLoginCode == code));
        }

        public User GetByMainMobile(string mainMobile)
        {
            if (PhoneUtility.ValidateLocalNumber(mainMobile))
                mainMobile = PhoneUtility.LocalNumberToInternational(mainMobile, 98);
            return Repository.Query(q => q.FirstOrDefault(f => f.MainMobile == mainMobile));
        }

        public User GetByEmail(string email)
        {
            return Repository.Query(q => q.FirstOrDefault(f => f.Email == email));
        }

        public User GetActivatedUserByMainMobile(string mainMobile, bool includeFavorite = false)
        {
            if (includeFavorite)
            {
                return Repository.Query(q => q.Include(i => i.Favorite).FirstOrDefault(
                    f => f.MainMobile == mainMobile &&
                    f.State == (int)User.UserState.Acticved));
            }
            return Repository.Query(q => q.FirstOrDefault(
                f => f.MainMobile == mainMobile &&
                f.State == (int)User.UserState.Acticved));
        }

        public User GetActivatedUserByEmail(string email, bool includeFavorite = false)
        {
            if (includeFavorite)
            {
                return Repository.Query(q => q.Include(i => i.Favorite)
                .FirstOrDefault(f => f.Email == email &&
                f.State == (int)User.UserState.Acticved));
            }
            return Repository.Query(q => q.FirstOrDefault(f => f.Email == email &&
                f.State == (int)User.UserState.Acticved));
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

        public bool Update(UserDTO dto, int currentUserId, bool userHasRefunedInProgress,
            ActionLog.ActionSourceEnum source, out List<string> errors, int? cancelInstantReserveLimit = null)
        {
            if (dto.Validate(out errors) == false)
                return false;
            var user = Repository.Find(dto.id);
            var shallowUser = user.ShallowCopy();
            user.ResponseFrom = dto.responseFrom;
            user.ResponseTo = dto.responseTo;
            user.FName = dto.fname;
            user.LName = dto.lname;
            user.AccessType = dto.accessType;
            user.OwnerShip = dto.OwnerShip;
            user.CancelInstantReserveLimit = dto.CancelInstantReserveLimit;
            user.ContactPhone = dto.ContactPhone;
            user.AmlakbashiScore = dto.AmlakbashiScore;
            user.Address = dto.Address;
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
            if (user.GetLoginProperty() != User.LoginPriorites.Email && string.IsNullOrEmpty(user.Email) || !string.IsNullOrEmpty(dto.email))
                user.Email = dto.email;
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
            var bankCards = user.BankCards;
            var bankCardObj = bankCards == null || bankCards.Count == 0 ? null : bankCards.FirstOrDefault();
            var hasChange = ((bankCardObj == null && dto.shabaNumber == null &&
                dto.bankCardNumber == null &&
                dto.bankFname == null &&
                dto.bankLname == null) ||
                (bankCardObj != null && dto.shabaNumber == bankCardObj.ShabaNumber &&
                dto.bankCardNumber == bankCardObj.BankCardNumber &&
                dto.bankFname == bankCardObj.FName &&
                dto.bankLname == bankCardObj.LName)) == false;

            if (hasChange &&
                (dto.userGeneralType > (int)User.UserGeneralTypeEnum.Guest ||
                !string.IsNullOrEmpty(dto.bankCardNumber) ||
                !string.IsNullOrEmpty(dto.shabaNumber) ||
                userHasRefunedInProgress))
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
            if (cancelInstantReserveLimit != null &&
                cancelInstantReserveLimit != user.CancelInstantReserveLimit)
            {
                var hostAccs = user.Advertises;
                if (hostAccs.Sum(x => x.InstantReserveCancels) > user.CancelInstantReserveLimit)
                {
                    user.InstantReserveAccess = User.InstantReserveAccessEnum.Banned;
                    foreach (var item in hostAccs)
                    {
                        item.InstantReserveStatus = Advertise.InstantReserveStatusEnum.None;
                    }
                }
                else
                {
                    user.InstantReserveAccess = User.InstantReserveAccessEnum.Verified;
                }
            }
            Repository.Update(user);
            Repository.Save();
            mediator.Publish(new UserUpdateEvent(shallowUser, user, source, currentUserId));
            return true;
        }

        public void UpdateState(int userId, bool state, int currentUserId = 0,
            ActionLog.ActionSourceEnum source = ActionLog.ActionSourceEnum.AdminPanel)
        {
            var user = Repository.Query(q => q.FirstOrDefault(f => f.Id == userId));
            var shallowUser = user.ShallowCopy();
            if (state)
            {
                user.State = (int)User.UserState.Acticved;
            }
            else
            {
                user.State = (int)User.UserState.InActived;
            }
            Repository.Update(user);
            Repository.Save();
            if (currentUserId > 0)
            {
                mediator.Publish(new UserUpdateEvent(shallowUser, user, source, currentUserId));
            }
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
                user.ContactPhone = "0";
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
            user.CreateDate = time;
            Repository.Update(user);
            Repository.Save();

            
        }

        public void UpdateAdminLoginCode(int userId, string code)
        {
            var user = Repository.Query(q => q.FirstOrDefault(f => f.Id == userId));
            user.AdminLoginCode = code;
            Repository.Update(user);
            Repository.Save();
        }

        public void UpdateForgetCode(int userId, string code)
        {
            var user = Repository.Query(q => q.FirstOrDefault(f => f.Id == userId));
            user.ForgetCode = code;
            Repository.Update(user);
            Repository.Save();
        }

        public void UpdateSendVerification(int userId, DateTime time, string code = null)
        {
            var user = Repository.Query(q => q.FirstOrDefault(f => f.Id == userId));
            user.SendVerification = time;
            if (code != null)
            {
                user.Code = code;
            }
            Repository.Update(user);
            Repository.Save();
        }

        public void UpdatePresentorUser(int userId, int pid)
        {
            var user = Repository.Query(q => q.FirstOrDefault(f => f.Id == userId));
            user.PresentorUserID = pid;
            Repository.Update(user);
            Repository.Save();
        }

        public void UpdateLoginPriority(int userId, User.LoginPriorites loginPriorites)
        {
            var user = Repository.Query(q => q.FirstOrDefault(f => f.Id == userId));
            user.SetLoginPriority(loginPriorites);
            Repository.Update(user);
            Repository.Save();
        }

        public void UpdateFName(int userId, string newFName)
        {
            var user = Repository.Query(q => q.FirstOrDefault(f => f.Id == userId));
            user.FName = newFName;
            Repository.Update(user);
            Repository.Save();
        }

        public void UpdateLName(int userId, string newLName)
        {
            var user = Repository.Query(q => q.FirstOrDefault(f => f.Id == userId));
            user.LName = newLName;
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

        public void UpdateLastNotifPermetionTicks(int userId, long ticks)
        {
            var user = Repository.Query(q => q.FirstOrDefault(f => f.Id == userId));
            user.LastNotifPermitionTicks = ticks;
            Repository.Update(user);
            Repository.Save();
        }

        public void UpdateLoginCode(int userId, string token)
        {
            var user = Repository.Query(q => q.FirstOrDefault(f => f.Id == userId));
            user.AdminLoginCode = token;
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
            if (user.NotificationToken != token)
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
            var user = Repository.Query(q => q.FirstOrDefault(f => f.Id == userId));
            var favorite = new UserFavorite();
            favorite.AdvertiseID = advertiseId;
            favorite.SetDate = DateTime.Now;
            user.Favorite.Add(favorite);
            Repository.Update(user);
            Repository.Save();
        }

        public void DeleteFavorite(int userId, long advertiseId)
        {
            var user = Repository.Query(q => q.FirstOrDefault(f => f.Id == userId));
            var favorite = user.Favorite.FirstOrDefault(f => f.AdvertiseID == advertiseId);
            if (favorite != null)
            {
                user.Favorite.Remove(favorite);
                Repository.Update(user);
                Repository.Save();
            }
        }

        public void SendVerificationSms(string localNumber, string code)
        {
            userContact.SendVerificationSms(localNumber, code);
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

        public bool VerifyLogin(string mobile, string code, out int user_id, string presentorCode, out string errorMsg)
        {
            var user = Repository.Query(q => q.FirstOrDefault(u => u.MainMobile == mobile && u.State == (int)User.UserState.Acticved));
            if (user == null)
                user = Repository.Query(q => q.OrderBy(u => u.Id).FirstOrDefault(u => u.MainMobile == mobile));

            if (user != null && code == user.Code)
            {
                if (user.State != (int)User.UserState.Acticved)
                {
                    user.AmlakbashiScore = 1000;
                    if (!string.IsNullOrEmpty(presentorCode))
                    {
                        try
                        {
                            var prId = int.Parse(presentorCode);
                            var prUser = Repository.Find(prId);
                            user.PresentorUserID = prUser.Id;
                            if (user.PresentorUserID > 0)
                            {
                                mediator.Send(new AddDiscountCouponCommand(user.Id, user.PresentorUserID,
                                    5, DiscountCoupon.DiscountCouponType.Present));
                                var contact = new UserContactDTO()
                                {
                                    UserLoginPriority = user.LoginPriority,
                                    UserMainMobile = user.MainMobile,
                                    UserAppNotificationToken = user.AppNotificationToken,
                                    UserEmail = user.Email,
                                    UserFcmAppNotificationToken = user.FcmAppNotificationToken,
                                    UserNotificationToken = user.NotificationToken,
                                    Type = UserContactType.CouponPresent,
                                    Extra1 = prUser.FullName,
                                    Extra2 = "5%"
                                };
                                mediator.Enqueue(new SendMessageCommand(contact));
                            }
                        }
                        catch
                        {
                            errorMsg = "کد معرف اشتباه است. لطفا بررسی کنید";
                            user_id = 0;
                            return false;
                        }
                    }
                }
                user.State = (int)User.UserState.Acticved;
                user.MainMobile = mobile;
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

        public async Task<AppUser> GetActivatedUserIdentity(string phrase, bool isEmail = false)
        {
            AppUser user;
            if (isEmail)
            {
                user = await userManager.FindByEmailAsync(phrase);
            }
            else
            {
                user = await userManager.FindByNameAsync(phrase);
            }
            if (user != null && user.State == User.UserState.Acticved)
            {
                return user;
            }
            return null;
        }

        public async Task<AppUser> GetUserIdentity(string phrase, bool isEmail = false)
        {
            AppUser user;
            if (isEmail)
            {
                user = await userManager.FindByEmailAsync(phrase);
            }
            else
            {
                user = await userManager.FindByNameAsync(phrase);
            }
            return user;
        }

        public async Task AddIdentityUser(AppUser user)
        {
            await userManager.CreateAsync(user);
        }

        public void UpdateIdentityUser(AppUser user)
        {
            var result = userManager.UpdateAsync(user).Result;
        }

        public bool VerifyLoginCode(string mobileInternational, string code)
        {
            var user = userManager.FindByNameAsync(mobileInternational).Result;
            if (user.Code == code)
            {
                return true;
            }
            return false;
        }
    }
}
