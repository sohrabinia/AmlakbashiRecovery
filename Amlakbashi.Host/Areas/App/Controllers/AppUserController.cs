using Amlakbashi.Application.Services.FileServices.Interfaces;
using Amlakbashi.Application.Services.ReserveServices.Interfaces;
using Amlakbashi.Application.Services.UserServices.Interfaces;
using Amlakbashi.Core.DTOs.UserDTOs;
using Amlakbashi.Core.Entities;
using Amlakbashi.Host.Area.App.Controllers.Base;
using Amlakbashi.Host.Authentication;
using Entities = Amlakbashi.Core.Entities;
using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Amlakbashi.Host.Areas.App.Controllers
{
    [Area("App")]
    [Route("app/user/[action]")]
    public class AppUserController : AppBaseController
    {
        private readonly IUserAccessor userAccessor;
        private readonly IUserAppService userService;
        private readonly IBankCardAppService bankCardService;
        private readonly IReserveAppService reserveService;
        private readonly IFileAppService fileService;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly ILog logger;
        public AppUserController(IUserAccessor userAccessor,
            IUserAppService userService,
            IBankCardAppService bankCardService,
            IReserveAppService reserveService,
            IFileAppService fileService,
            IWebHostEnvironment webHostEnvironment,
            ILog logger)
        {
            this.userAccessor = userAccessor;
            this.userService = userService;
            this.bankCardService = bankCardService;
            this.reserveService = reserveService;
            this.fileService = fileService;
            this.webHostEnvironment = webHostEnvironment;
            this.logger = logger;
        }

        public ActionResult Signout()
        {
            userService.SignOut();
            HttpContext.Session.Clear();
            return Redirect("/app/home/main");
        }

        [Authorize]
        [HttpGet]
        public ActionResult Profile()
        {
            try
            {
                var user = userAccessor.CurrentUser;
                var identityUser = userService.GetIdentityUser(user.MainMobile);
                var model = UserDTO.Generate(user, identityUser);
                var bankCard = bankCardService.GetByUserId(user.Id);
                if (bankCard != null)
                {
                    model.bankCardNumber = bankCard.BankCardNumber;
                    model.shabaNumber = bankCard.ShabaNumber;
                    model.bankFname = bankCard.FName;
                    model.bankLname = bankCard.LName;
                }
                ViewBag.msg = TempData["msg"];
                ViewBag.suc = TempData["suc"];
                ViewBag.photoId = user.PhotoID;
                ViewBag.photoStatus = user.PhotoStatus;
                ViewBag.hasCanselReserve = reserveService.UserHasRefundInProgress(user.Id);
                return View(model);
            }
            catch (Exception exc)
            {
                logger.Error("User.ProfileManager", exc);
                return Redirect(Request.Headers["Referer"].ToString());
            }
        }

        [Authorize]
        [HttpPost]
        public ActionResult Profile(UserDTO user)
        {
            try
            {
                if (user.id != userAccessor.CurrentUser.Id)
                    return Redirect("/errors/http404");

                if (Request.Form.Files.Count > 0 && Request.Form.Files[0].Length > 0)
                {
                    var uploadfile = Request.Form.Files[0];
                    var contentType = uploadfile.ContentType.ToLower();
                    if ((contentType == "image/png" ||
                        contentType == "image/gif" ||
                        contentType == "image/jpg" ||
                        contentType == "image/jpeg") == false)
                    {
                        TempData["msg"] = "فرمت عکس مورد قبول نمی باشد";
                        return RedirectToAction("profilemanager");
                    }

                    string filepath = $"~/content/users/user_{user.id}.jpg";
                    long PhotoID = 0;
                    if (userAccessor.CurrentUser.Photo != null)
                    {
                        PhotoID = userAccessor.CurrentUser.Photo.Id;
                        var oldFilePath = webHostEnvironment.WebRootPath + userAccessor.CurrentUser.Photo.FilePath.Replace("~", "");
                        fileService.UpdateFilePath(PhotoID, filepath);
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }
                    else
                    {
                        var newProfilePhoto = new File()
                        {
                            PostDate = DateTime.Now,
                            LastModifyDate = DateTime.Now,
                            UserID = user.id,
                            FilePath = filepath
                        };
                        PhotoID = fileService.Insert(newProfilePhoto);
                    }

                    if (!System.IO.Directory.Exists(webHostEnvironment.WebRootPath + "/content/users"))
                        System.IO.Directory.CreateDirectory(webHostEnvironment.WebRootPath + "/content/users");

                    using (var stream = System.IO.File.Create(webHostEnvironment.WebRootPath + filepath.Replace("~", "")))
                    {
                        uploadfile.CopyTo(stream);
                    }

                    userService.UpdateProfilePhoto(user.id, PhotoID, Entities.User.UserPhotoState.ready_publish);

                    System.IO.DirectoryInfo IOdirectory = new System.IO.DirectoryInfo(System.IO.Path.Combine(webHostEnvironment.WebRootPath, "content/imgcache"));
                    foreach (System.IO.FileInfo IOfile in IOdirectory.GetFiles())
                    {
                        IOfile.Delete();
                    }
                }
                List<string> errors;
                bool hasRefundInProgress = reserveService.UserHasRefundInProgress(user.id);
                var done = userService.Update(user, userAccessor.CurrentUser.Id, hasRefundInProgress, ActionLog.ActionSourceEnum.WebsiteDashboard, out errors);
                if (done)
                {
                    TempData["suc"] = "ویرایش پروفایل شما با موفقیت انجام شد";
                }
                else
                {
                    TempData["msg"] = errors.First();
                }

                return RedirectToAction(nameof(Profile));
            }
            catch (Exception exc)
            {
                logger.Error("User.Profile", exc);
                return RedirectToAction(nameof(Profile));
            }
        }
    }
}
