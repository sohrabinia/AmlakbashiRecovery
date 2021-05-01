using System;
using System.Collections.Generic;
using System.Linq;
using X.PagedList;
using System.Drawing;
using System.Drawing.Imaging;
using Amlakbashi.Application.Services.BlogPostServices.Interfaces;
using log4net;
using AutoMapper;
using Amlakbashi.Application.Services.PostServices.Interfaces;
using Amlakbashi.Core.DTOs;
using static Amlakbashi.Core.Entities.Post;
using Amlakbashi.Core.Entities;
using Amlakbashi.Application.Services.CommentServices.Interfaces;
using Amlakbashi.Application.Services.UserServices.Interfaces;
using Entities = Amlakbashi.Core.Entities;
using Amlakbashi.Core.DTOs.UserDTOs;
using Amlakbashi.Core.Common.Utilities;
using Amlakbashi.Core.Infrastructure.LocalizationHelpers;
using Amlakbashi.Application.Services.FileServices.Interfaces;
using Amlakbashi.Application.Services.AdvertiseServices.Interfaces;
using Amlakbashi.Application.Services.Category.Interfaces;
using static Amlakbashi.Core.Entities.Region;
using static Amlakbashi.Core.Entities.Advertise;
using Amlakbashi.Core.DTOs.AccommodationDTOs.AccDashboardDTOs;
using Amlakbashi.Core.DTOs.HomePageDTOs;
using Amlakbashi.Core.DTOs.AccommodationDTOs;
using Amlakbashi.Application.Services.ReserveServices.Interfaces;
using Amlakbashi.Core.DTOs.PostDTOs;
using Amlakbashi.Host.Controllers.Base;
using Amlakbashi.Host.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Amlakbashi.Host.Extensions;
using Microsoft.AspNetCore.Authorization;
using Amlakbashi.Core.Identity;

namespace Amlakbashi.Host.Controllers
{
    public class PostController : BaseController
    {
        public static List<int> most_view_city_category_ids = new List<int> { 55784, 85173, 55827, 55979, 55816, 55978, 55786 };
        public static List<string> most_view_city_names = new List<string> { "سوئیت و آپارتمان مبله تهران", "اجاره ویلا و سوئیت شمال", "سوئیت و آپارتمان مبله اصفهان", "سوئیت و آپارتمان مبله شیراز", "اجاره ویلا و سوئیت کردان", "اجاره ویلا و سوئیت رامسر", "سوئیت و آپارتمان مبله مشهد" };
        public static List<string> most_view_city_image_names = new List<string> { "tehran", "mazandaran", "esfahan", "shiraz", "kordan", "ramsar", "mashhad" };
        private readonly ILog logger;
        private readonly IMapper mapper;
        private readonly IPostAppService postService;
        private readonly IServiceAppService serviceService;
        private readonly IBlogPostAppService blogPostService;
        private readonly ICommentAppService commentService;
        private readonly IReportItemAppService reportItemService;
        private readonly IUserAppService userService;
        private readonly IBankCardAppService bankCardService;
        private readonly IFileAppService fileService;
        private readonly IAdvertiseAppService advertiseService;
        private readonly ICategoryAppService categoryService;
        private readonly IRegionAppService regionService;
        private readonly IDiscountTableAppService discountTableServie;
        private readonly IReserveAppService reserveService;
        private readonly IUserAccessor userAccessor;
        private readonly IWebHostEnvironment webHostEnvironment;
        public PostController(
            IPostAppService postService,
            IServiceAppService serviceService,
            IBlogPostAppService blogPostService,
            ICommentAppService commentService,
            IReportItemAppService reportItemService,
            IUserAppService userService,
            IBankCardAppService bankCardService,
            IFileAppService fileService,
            IAdvertiseAppService advertiseService,
            ICategoryAppService categoryService,
            IRegionAppService regionService,
            IDiscountTableAppService discountTableServie,
            IReserveAppService reserveService,
            IUserAccessor userAccessor,
            IWebHostEnvironment webHostEnvironment,
            ILog logger, IMapper mapper)
        {
            this.logger = logger;
            this.mapper = mapper;
            this.serviceService = serviceService;
            this.reserveService = reserveService;
            this.postService = postService;
            this.blogPostService = blogPostService;
            this.commentService = commentService;
            this.reportItemService = reportItemService;
            this.userService = userService;
            this.bankCardService = bankCardService;
            this.fileService = fileService;
            this.advertiseService = advertiseService;
            this.regionService = regionService;
            this.categoryService = categoryService;
            this.discountTableServie = discountTableServie;
            this.userAccessor = userAccessor;
            this.webHostEnvironment = webHostEnvironment;
        }

        [Authorize(Policy = Policies.Post_View)]
        public ActionResult Index(int? page,
            PostStatus status = PostStatus.Suspend,
            int service = 43)
        {
            try
            {
                var servicesRaw = serviceService.GetRoots();
                var services = servicesRaw.Select(s => mapper.Map<ServiceDTO>(s)).ToList();
                foreach (var item in services)
                {
                    item.AddChildren(serviceService.GetChildren(item.Id)
                        .Select(s => mapper.Map<ServiceDTO>(s)).ToList());
                }
                var model = postService.Filter(status, service);
                ViewBag.Services = services;
                ViewBag.status = status;
                ViewBag.service = service;
                var PageNumber = page ?? 1;
                var onePageOfModel = model.ToPagedList(PageNumber, 20);
                ViewBag.RowIndexStart = (PageNumber * 20) - 20;

                List<PostIndexDTO> postDTOs = new List<PostIndexDTO>();
                foreach (var item in onePageOfModel)
                {
                    var dto = new PostIndexDTO()
                    {
                        Post = item,
                        UserPhoneNumber = userService.Find(item.UserID).GetPhoneNumber(Entities.User.PhoneType.MainMobile)
                    };
                    postDTOs.Add(dto);
                }
                ViewBag.dto = postDTOs;
                return View(onePageOfModel);
            }
            catch (Exception exc)
            {
                logger.Error("Post.Index", exc);
                return Redirect(Request.Headers["Referer"].ToString());
            }
        }

        [Authorize(Policy = Policies.Post_Edit)]
        [HttpGet]
        public ActionResult Edit(long id = -1)
        {
            try
            {
                ViewBag.msg = TempData["msg"];
                var servicesRaw = serviceService.GetRoots();
                var services = servicesRaw.Select(s => mapper.Map<ServiceDTO>(s)).ToList();
                foreach (var item in services)
                {
                    item.AddChildren(serviceService.GetChildren(item.Id)
                        .Select(s => mapper.Map<ServiceDTO>(s)).ToList());
                    foreach (var grandchild in item.children)
                    {
                        grandchild.AddChildren(serviceService.
                            GetChildren(grandchild.Id)
                            .Select(s => mapper.Map<ServiceDTO>(s)).ToList());
                    }
                }
                IEnumerable<File> Images = fileService.GetAllDescendingByLastModifyDate(50);
                var CurrentServices = postService.GetRelatedServiceIds(id);
                ViewBag.Services = services;
                ViewBag.Images = Images;
                ViewBag.CurrentServices = CurrentServices;
                if (id == -1)
                {
                    var objPost = new Post();
                    objPost.Id = -1;
                    return View(objPost);
                }
                else
                {
                    var model = postService.Find(id);
                    return View(model);
                }
            }
            catch (Exception exc)
            {
                logger.Error("Post.Edit", exc);
                return Redirect(Request.Headers["Referer"].ToString());
            }
        }

        [Authorize(Policy = Policies.Post_Edit)]
        [HttpPost]
        public ActionResult Edit(Post post, List<int> Services = null)
        {
            try
            {
                if (string.IsNullOrEmpty(post.Title))
                {
                    TempData["msg"] = "لطفا عنوان پست را وارد کنید .";
                    return RedirectToAction("Edit");
                }
                List<int> serviceIds = new List<int>();
                if (Services != null)
                {
                    serviceIds = Services;
                }
                if (post.Id == -1)
                {
                    postService.Insert(post, userAccessor.CurrentUser.Id, serviceIds);
                }
                else
                {
                    postService.Update(post, serviceIds);
                }
                return RedirectToAction("Index");
            }
            catch (Exception exc)
            {
                logger.Error("post insert/update failed", exc);
                return Redirect(Request.Headers["Referer"].ToString());
            }
        }

        [Authorize(Policy = Policies.Post_Publish)]
        public JsonResult Delete(long id)
        {
            try
            {
                postService.Delete(id);
                return GenerateJsonResult(new { status = 1, val = "" });
            }
            catch (Exception exc)
            {
                logger.Error("post deletion failed", exc);
                return GenerateJsonResult(new { status = 0, val = "" });
            }
        }

        [Authorize(Policy = Policies.Post_Publish)]
        public JsonResult Suspend(long id)
        {
            try
            {
                postService.SetStatus(id, PostStatus.Suspend);
                return GenerateJsonResult(new { status = 1, val = "" });
            }
            catch (Exception exc)
            {
                logger.Error("error while suspending post", exc);
                return GenerateJsonResult(new { status = 0, val = "" });
            }
        }

        [Authorize(Policy = Policies.Post_Publish)]
        public JsonResult Publish(long id)
        {
            try
            {
                postService.SetStatus(id, PostStatus.Published);
                return GenerateJsonResult(new { status = 1, val = "" });
            }
            catch (Exception exc)
            {
                logger.Error("error while publishing post", exc);
                return GenerateJsonResult(new { status = 0, val = "" });
            }
        }

#if !DEBUG
        [ResponseCache(Duration = 60 * 60, VaryByQueryKeys = new string[] { "amp_version" })]
#endif
        public ActionResult Page(bool amp_version = false)
        {
            ViewBag.MessageShowOnReady = TempData["MessageShowOnReady"];
            if (amp_version && HttpContext.Request.Path.Value.Last() == '/')
            {
                Response.StatusCode = 404;
                return View("/views/errors/http404.cshtml");
            }
            ViewBag.home = true;
            var categories = new int[] { 55894, 55944, 55957, 55861, 55953 };
            var advertiseItemCount = 5;

            var user = userAccessor.CurrentUser;
            var userFavorites = user.Favorite == null ? new List<UserFavorite>() : user.Favorite.ToList();
            var home_categories = new List<HomePageCategoryDTO>();
            IList<Advertise> advertises;
            foreach (var id in categories)
            {
                var category = categoryService.Find(id);
                var home_cat = new HomePageCategoryDTO();
                home_cat.CategoryID = category.Id;
                home_cat.category = category;
                home_cat.categoryUrl = CategoryUrlLocalization.CategoryToUrl(category);
                home_cat.Title = category.Title;
                home_cat.URL = category.URL;
                home_cat.CountAdvertise = category.CountAdvertise;
                home_cat.Advertises = new List<HomePageAdvertiseDTO>();
                home_cat.AdvertiseItems = new List<AccommodationCardDTO>();

                // Initial Advertises
                advertises = category.Advertises.OrderByDescending(o => o.AdvertiseScore).Take(advertiseItemCount).ToList();
                foreach (var adv in advertises)
                {
                    var rate = adv.AverageUserRating;
                    var review_count = adv.ReportItems.GroupBy(g => g.UserID).Count();
                    home_cat.Advertises.Add(new HomePageAdvertiseDTO()
                    {
                        Id = adv.Id,
                        Title = adv.Title,
                        Description = adv.Description,
                        ImageSource = adv.PhotoID > 0 ? string.Format("/عکس-آگهی/{0}", adv.Slug) : string.Format("/عکس-یافت-نشد-{0}-{1}", 240, 144),
                        Rate = rate,
                        ReviewCount = review_count
                    });
                    AccommodationCardDTO advItem = adv;
                    advItem.Favourited = user.Id > 0 && userFavorites.Any(x => x.AdvertiseID == adv.Id);
                    home_cat.AdvertiseItems.Add(advItem);
                }

                home_cat.category = category;
                home_cat.categoryUrl = CategoryUrlLocalization.CategoryToUrl(category);
                var provinceString = category.Province == null ? "" : category.RegionProvince.PersianName;
                var cityString = category.City == null ? "" : category.RegionCity.PersianName;
                var areaString = category.Area == null ? "" : category.RegionArea.PersianName;
                var countryDirectionString = GetCountryDirectionString(category.CountryDirection);
                home_cat.categoryH1Title = AdvertiseSeoLocalization
                    .GetTitle(0, (int)category.Type, provinceString,
                    cityString, areaString, countryDirectionString);
                home_categories.Add(home_cat);
            }

            // Most View Regions
            var mostViewRegions = new List<MostViewRegionsDTO>();
            var mostViewCategoryList = categoryService.GetListByIds(most_view_city_category_ids);
            mostViewCategoryList = mostViewCategoryList.OrderBy(o => most_view_city_category_ids.IndexOf(o.Id)).ToList();
            var mostViewDic = regionService.GetRegionPersianNamesByCategoryList(mostViewCategoryList);
            int index = 0;
            foreach (var item in mostViewDic)
            {
                mostViewRegions.Add(new MostViewRegionsDTO()
                {
                    Title = item.Key.Title,
                    CityName = most_view_city_names[index],
                    ImageName = most_view_city_image_names[index],
                    Url = CategoryUrlLocalization.CategoryToUrl(item.Key),
                    MetaTitle = AdvertiseSeoLocalization.GetMetaTitle(0, 0, 0, (int)item.Key.Type,
                        item.Value[0], item.Value[1], item.Value[2], null)
                });
                index++;
            }

            // Most Discount Advertise
            var mostDiscountAccs = discountTableServie.GetMostDiscountAdvertises(5);
            List<AccommodationCardDTO> itemDTOs = new List<AccommodationCardDTO>();
            foreach (var item in mostDiscountAccs)
            {
                var dto = (AccommodationCardDTO)item;
                dto.Favourited = user.Id > 0 && userFavorites.Any(x => x.AdvertiseID == item.Id);
                itemDTOs.Add(dto);
            }
            ViewBag.mostDiscountAdvertise = itemDTOs;
            var norouzItemDTOs = new List<AccommodationCardDTO>();
            //var norouzAccs = advertiseService.GetNorouzAdvertises(5);
            var norouzAccs = new List<Advertise>();
            foreach (var item in norouzAccs)
            {
                var dto = (AccommodationCardDTO)item;
                dto.Favourited = user.Id > 0 && userFavorites.Any(x => x.AdvertiseID == item.Id);
                norouzItemDTOs.Add(dto);
            }
            ViewBag.norouzAdvertises = norouzItemDTOs;

            ViewBag.advertiseItemCount = advertiseItemCount;
            ViewBag.homePageCategories = home_categories;
            ViewBag.blogPostNews = blogPostService.GetNewItems(
                BlogPost.PlaceEnum.HomePage, 3);
            if (amp_version)
            {
                return View("../Amp/Home/Page.amp");
            }
            else
            {
                return View(mostViewRegions);
            }
        }

        [ResponseCache(Duration = 60 * 60)]
        public ActionResult Search()
        {
            return View();
        }

        [ResponseCache(Duration = 60 * 60, VaryByQueryKeys = new string[] { "amp_version" })]
        public ActionResult SearchAccID()
        {
            return View();
        }

        public ActionResult News(bool amp_version = false)
        {
            if (HttpContext.Request.Path.Value.ToLower() == "/post/news")
            {
                return RedirectPermanent("/اخبار-و-مقالات");
            }
            var postList = new List<IList<Post>>();
            postList.Add(postService.Filter(PostStatus.Published, 23).Take(3).ToList());
            postList.Add(postService.Filter(PostStatus.Published, 10).Take(3).ToList());
            postList.Add(postService.Filter(PostStatus.Published, 13).Take(3).ToList());
            postList.Add(postService.Filter(PostStatus.Published, 14).Take(3).ToList());
            ViewBag.news = true;
            ViewBag.amp_version = amp_version;
            return View();
        }

        [Authorize]
        public ActionResult dashboard()
        {
            var userId = userAccessor.CurrentUser.Id;
            ViewBag.alert_msg = TempData["alert"];
            var model = advertiseService.GetAdvertisesByUserId(userId);
            return View(model);
        }

        public ActionResult Public(int sid)
        {
            if (HttpContext.Request.Path.Value == "/post/public?sid=" + sid)
            {
                switch (sid)
                {
                    case 6:
                        return RedirectPermanent("/contact");
                    case 4:
                        return RedirectPermanent("/درباره-ما");
                    case 8:
                        return RedirectPermanent("/help");
                    case 24:
                        return RedirectPermanent("/comment-complain");
                    case 25:
                        return RedirectPermanent("/rules");
                    default:
                        break;
                }
            }
            var model = serviceService.Find(sid);
            ViewBag.FirstPost = postService.Filter(PostStatus.Published,
                sid).FirstOrDefault();
            ViewBag.raw_url = HttpContext.Request.Path.Value.ToLower();
            return View(model);
        }

        public ActionResult NewsItem(long id, string title = "", bool amp_version = false)
        {
            try
            {
                ViewBag.news = true;
                ViewBag.raw_url = HttpContext.Request.Path.Value.Replace("/amp", "");
                ViewBag.amp_version = amp_version;
                var model = postService.Find(id);
                if (!string.IsNullOrEmpty(model.Link))
                    return Redirect(model.Link);
                var target_title = model.Title.Replace("+", "-").Replace(" ", "-");
                if (string.IsNullOrEmpty(title) && HttpContext.Request.Path.Value == "/post/newsitem?id=" + id)
                {
                    return RedirectPermanent(string.Format("/اخبار-و-مقالات/{0}-{1}", target_title, id));
                }
                if (HttpContext.Request.Path.Value == "/اخبار-و-مقالات/درباره-ما-5")
                {
                    return RedirectPermanent("/درباره-ما");
                }
                if (target_title != title)
                {
                    return StatusCode(404);
                }
                return View(model);
            }
            catch (Exception exc)
            {
                return StatusCode(404);
            }
        }

        [Authorize]
        public ActionResult Personal(int? page, int UserID = -1, string type = "all", long id = -1)
        {
            return RedirectPermanent("/post/accomodationmanager?page=" + page + "&userid=" + UserID + "&type=" + type + "&id=" + id);
        }

        [Authorize]
        public ActionResult AdvertiseManager(int? page, int UserID = -1, string type = "all", long id = -1)
        {
            return RedirectPermanent("/post/accomodationmanager?page=" + page + "&userid=" + UserID + "&type=" + type + "&id=" + id);
        }

        [Authorize]
        public ActionResult AccomodationManager(int? page,
            string type = "all", string id = "-1")
        {
            int UserID = userAccessor.CurrentUser.Id;
            var id_long = string.IsNullOrEmpty(id) ? 0 : long.Parse(StringUtility.PersianNumberToEnglish(id));
            var model = advertiseService.Filter(type, UserID, id_long);
            var result = model
                .OrderBy(x => x.Status == 0 ? 0 : (x.Status == AdvertiseStatus.FirstReady ?
                1 : (x.Status == AdvertiseStatus.Published ? 2 : 3)))
                .ThenByDescending(x => x.CreateDate);
            var PageNumber = page ?? 1;
            var onePageOfModel = result.ToPagedList(PageNumber, 5);
            var pageCount = (int)Math.Ceiling(result.Count() / 5f);
            if (pageCount > 1 && PageNumber > pageCount)
                return Redirect("/errors/Http404");
            var finalModel = AccommodationManagerDTO.Generate(userAccessor.CurrentUser, onePageOfModel.ToList());
            var insertCount = (PageNumber - 1) * 5;
            for (int i = 0; i < insertCount; i++)
            {
                (finalModel.accList as List<DashboardAccDTO>).Insert(0, null);
            }
            var addCount = result.Count() - finalModel.accList.Count();
            for (int i = 0; i < addCount; i++)
            {
                (finalModel.accList as List<DashboardAccDTO>).Add(null);
            }
            finalModel.accList = finalModel.accList.ToPagedList(PageNumber, 5);

            ViewBag.alert_success = TempData["alert_success"];
            ViewBag.alert_error = TempData["alert_error"];
            ViewBag.EditMode = true;
            ViewBag.UserID = UserID;
            ViewBag.type = type;
            ViewBag.id = id_long;
            return View(finalModel);
        }

        public ActionResult Http404()
        {
            return Redirect("/errors/Http404");
        }

        public ActionResult FavoriteManager(int aid = -1)
        {
            try
            {
                var user = userAccessor.CurrentUser;
                var advertise_ids = user.Favorite.OrderByDescending(f => f.SetDate).
                    Select(f => f.AdvertiseID).Take(100).ToList();
                var model = advertiseService.GetAccListByIds(advertise_ids);
                List<AccommodationCardDTO> advertiseItemDTOs = new List<AccommodationCardDTO>();
                foreach (var item in model)
                {
                    var dto = (AccommodationCardDTO)item;
                    dto.Favourited = user.Id > 0 && user.Favorite.Any(x => x.AdvertiseID == item.Id);
                    advertiseItemDTOs.Add(dto);
                }
                return View(advertiseItemDTOs);
            }
            catch (Exception exc)
            {
                logger.Error("Post.FavoriteManager", exc);
                return Redirect("/errors/http404");
            }
        }

        public ActionResult ReplyComment(int commentID, int currentFilterType)
        {
            try
            {
                var model = commentService.Find(commentID);
                if (model?.RecieverUserID != userAccessor.CurrentUser.Id)
                    return Redirect("/errors/http404");
                ViewBag.CurrentFilterType = currentFilterType;
                var replyMessage = commentService.GetParent(commentID);
                if (replyMessage != null)
                {
                    ViewBag.ReplySent = true;
                    //ViewBag.ReplyTitle = replyMessage.Title;
                    //ViewBag.ReplyMessage = replyMessage.Description;
                }
                else
                {
                    ViewBag.ReplySent = false;
                }
                return View(model);
            }
            catch (Exception exc)
            {
                logger.Error("Post.ReplyComment", exc);
                return Redirect("/errors/http404");
            }
        }

        [Authorize]
        public JsonResult AddReplyComment(int commentID, string replyComment, int currentFilterType)
        {
            try
            {
                var commentToReply = commentService.Find(commentID);
                if (userAccessor.CurrentUser.Id != commentToReply.RecieverUserID)
                {
                    return GenerateJsonResult(new { status = 0, val = "" });
                }
                if (string.IsNullOrEmpty(replyComment))
                {
                    return GenerateJsonResult(new { status = 2, val = "" });
                }

                //var reply = CommentDepend.GenerateReplyComment(commentToReply, replyComment);
                Comment comment = new Comment()
                {
                    RecieverUserID = commentToReply.SenderUserID,
                    SenderUserID = (int)commentToReply.RecieverUserID,
                    Status = (int)Comment.CommentStatus.ready,
                    type = commentToReply.type,
                    Text = replyComment,
                    AdvertiseID = commentToReply.AdvertiseID,
                    PostID = commentToReply.PostID,
                    CreateDate = DateTime.Now,
                    LastModifyDate = DateTime.Now,
                    LastModifyDatetick = DateTime.Now.Ticks
                };

                commentService.Insert(comment);
                ViewBag.CurrentFilterType = currentFilterType;
                return GenerateJsonResult(new { } /*new { status = 1, val = "", replyTitle = reply.Title, replyMessage = reply.Description }*/);
            }
            catch (Exception exc)
            {
                logger.Error("Post.AddReplyComment", exc);
                return GenerateJsonResult(new { status = 0, val = "" });
            }
        }

        [Authorize]
        [HttpGet]
        public ActionResult SendComment()
        {
            try
            {
                return View();
            }
            catch (Exception exc)
            {
                logger.Error("Post.SendComment", exc);
                return Redirect(Request.Headers["Referer"].ToString());
            }
        }

        [Authorize]
        public JsonResult DeleteComment(int id)
        {
            try
            {
                var objComment = commentService.Find(id);
                if (objComment.RecieverUserID != userAccessor.CurrentUser.Id)
                {
                    return GenerateJsonResult(new { status = 0, val = "" });
                }
                commentService.Delete(id);
                return GenerateJsonResult(new { status = 1, val = "" });
            }
            catch (Exception exc)
            {
                logger.Error("Post.DeleteComment", exc);
                return GenerateJsonResult(new { status = 0, val = "" });
            }
        }

        [Authorize]
        [HttpGet]
        public ActionResult ProfileManager()
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
                logger.Error("Post.ProfileManager", exc);
                return Redirect(Request.Headers["Referer"].ToString());
            }
        }

        [Authorize]
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult ProfileManager(UserDTO user)
        {
            try
            {
                var test = User;
                if (user.id != userAccessor.CurrentUser.Id)
                    return Redirect("/errors/http404");

                string ext = "", filepath = "";
                int FileType = -1;
                if (Request.Form.Files.Count > 0 && Request.Form.Files[0].Length > 0)
                {
                    var uploadfile = Request.Form.Files[0];
                    ext = System.IO.Path.GetExtension(uploadfile.FileName).ToLower();
                    if (ext == ".png" || ext == ".jpg" || ext == ".gif")
                        FileType = (int)Entities.File.FileTypes.Image;
                    else
                    {
                        TempData["msg"] = "فرمت عکس مورد قبول نمی باشد .";
                        return Redirect(Request.Headers["Referer"].ToString());
                    }

                    filepath = string.Format("{0}/content/users/user{1}{2}", webHostEnvironment.WebRootPath, Guid.NewGuid(), ext);
                    if (!System.IO.Directory.Exists(webHostEnvironment.WebRootPath + "/content/users"))
                        System.IO.Directory.CreateDirectory(webHostEnvironment.WebRootPath + "/content/users");
                    using (var stream = System.IO.File.Create(filepath))
                    {
                        uploadfile.CopyTo(stream);
                    }

                    var nfile = new File();
                    nfile.FilePath = filepath;
                    nfile.PostDate = DateTime.Now;
                    nfile.LastModifyDate = DateTime.Now;
                    nfile.UserID = userAccessor.CurrentUser.Id;
                    long PhotoID = fileService.Insert(nfile);
                    userService.UpdateProfilePhoto(user.id, PhotoID, Entities.User.UserPhotoState.ready_publish);
                }
                string msg;
                List<string> errors;
                bool hasRefundInProgress = reserveService.UserHasRefundInProgress(user.id);
                var done = userService.Update(user, userAccessor.CurrentUser.Id, hasRefundInProgress, ActionLog.ActionSourceEnum.WebsiteDashboard, out errors);
                if (done)
                {
                    if (HttpContext.Session.GetObjectFromJson<User>("impersonateUser") != null)
                    {
                        HttpContext.Session.SetObjectAsJson("impersonateUser", userService.Find(user.id));
                    }
                    msg = "ویرایش پروفایل شما با موفقیت انجام شد";
                    TempData["suc"] = msg;
                }
                else
                {
                    msg = errors.First();
                    TempData["msg"] = msg;
                }

                return Redirect(Request.Headers["Referer"].ToString());
            }
            catch (Exception exc)
            {
                logger.Error("Post.ProfileManager", exc);
                return Redirect(Request.Headers["Referer"].ToString());
            }
        }

        [HttpGet]
        public ActionResult FrequentlyQuestions(bool amp_version = false)
        {
            try
            {
                if (HttpContext.Request.Path.Value.ToLower() == "/post/frequentlyquestions")
                {
                    return RedirectPermanent("/سوالات-متداول");
                }
                ViewBag.amp_version = amp_version;
                return View();
            }
            catch (Exception exc)
            {
                logger.Error("Post.FrequentlyQuestions", exc);
                return Redirect(Request.Headers["Referer"].ToString());
            }
        }

        [HttpGet]
        public ActionResult Contact()
        {
            try
            {
                return View();
            }
            catch (Exception exc)
            {
                logger.Error("", exc);
                return Redirect(Request.Headers["Referer"].ToString());
            }
        }

        [HttpPost]
        public JsonResult SaveUploadedFile()
        {
            try
            {
                var quality = 80;
                var maxWidth = 1024;
                long photoID = -1;
                if (Request.Form.Files.Count > 0 && Request.Form.Files[0].Length > 0)
                {
                    string extension = "", filepath = "";
                    var uploadfile = Request.Form.Files[0];
                    extension = System.IO.Path.GetExtension(uploadfile.FileName).ToLower();

                    string filename = string.Format("advertise{0}{1}", Guid.NewGuid(), ".jpg");

                    filepath = "~/content/advertise/" + filename;
                    if (!System.IO.Directory.Exists(webHostEnvironment.WebRootPath + "/content/advertise"))
                        System.IO.Directory.CreateDirectory(webHostEnvironment.WebRootPath + "/content/advertise");
                    switch (extension)
                    {
                        case ".png":
                        case ".gif":
                        case ".jpg":
                        case ".jpeg":
                            ImageCodecInfo format = ImageUtility.GetEncoder(ImageFormat.Jpeg);
                            EncoderParameters encoderParameters = new EncoderParameters(1);
                            encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, quality);
                            var image = Image.FromStream(uploadfile.OpenReadStream(), true, true);
                            image = ImageUtility.MinifyImage(image, maxWidth);
                            image.Save(webHostEnvironment.WebRootPath + filepath.Replace("~", ""), format, encoderParameters);
                            break;
                        default:
                            //return Json(new { Status = 0, Message = "فرمت عکس مورد قبول نمی باشد ." });
                            return GenerateJsonResult(new { Status = 0, Message = "فرمت عکس مورد قبول نمی باشد ." });
                    }

                    File ObjFile = new File();
                    ObjFile.FilePath = filepath;
                    ObjFile.PostDate = DateTime.Now;
                    if (User.Identity.IsAuthenticated)
                    {
                        ObjFile.UserID = userAccessor.CurrentUser.Id;
                    }
                    else
                    {
                        ObjFile.UserID = 0;
                    }
                    ObjFile.LastModifyDate = DateTime.Now;
                    ObjFile.MinifyStatus = Entities.File.MinifyStatusEnum.Done;
                    ObjFile.MinifyQualityPercent = quality;
                    ObjFile.MinifyMaxWidth = maxWidth;
                    photoID = fileService.Insert(ObjFile);
                }

                if (photoID < 1)
                {
                    return Json(new { Status = 0, Message = "خطا در دریافت فایل، لطفا دوباره امتحان کنید" });
                }

                //return Json(new { Status = 1, id = photoID });
                return GenerateJsonResult(new { Status = 1, id = photoID });
            }
            catch (Exception exc)
            {
                logger.Error("Post.SaveUploadedFile", exc);
                //return Json(new { Status = 0, Message = "فرمت عکس مورد قبول نمی باشد ." });
                return GenerateJsonResult(new { Status = 0, Message = "فرمت عکس مورد قبول نمی باشد ." });
            }

        }

        public ActionResult DownloadApp(bool fromApp = false, bool amp_version = false)
        {
            ViewBag.fromApp = fromApp;
            if (amp_version)
            {
                return View("../Amp/Home/DownloadApp.amp");
            }
            return View();
        }

        [ResponseCache(Duration = 24 * 60 * 60, VaryByQueryKeys = new string[] {"*"})]
        public ActionResult DownloadAppPopup(bool ios = false)
        {
            ViewBag.ios = ios;
            return PartialView("_DownloadAppPopup");
        }

        //[Authorize]
        public ActionResult PresentAndPrize(int userId = 0)
        {
            var user = userId > 0 ? userService.Find(userId) : userAccessor.CurrentUser;
            var presentorCode = user.Id;
            ViewBag.presentorCode = presentorCode == 0 ? "" : presentorCode.ToString();
            ViewBag.refreshOnLogin = presentorCode == 0;
            return View();
        }

        public ActionResult EnamadRedirect()
        {
            return View();
        }

        public ActionResult SamandehiRedirect()
        {
            return View();
        }

        [ResponseCache(Duration = 24 * 60 * 60, VaryByQueryKeys = new string[] { "*" })]
        public ActionResult GetMessagePopup(bool fullScreen = false)
        {
            ViewBag.fullScreen = fullScreen;
            return PartialView("_MessagePopup");
        }

        public ActionResult GetPresentAndPrizePopup(bool containOnLogin = false)
        {
            var currentUser = userAccessor.CurrentUser;
            ViewBag.presentorCode = currentUser.Id == 0 ? "" : currentUser.Id.ToString();
            ViewBag.containOnLogin = containOnLogin;
            return PartialView("_PresentAndPrize");
        }
    }
}

