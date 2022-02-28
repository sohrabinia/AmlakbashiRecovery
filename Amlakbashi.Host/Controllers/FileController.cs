using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using Amlakbashi.Application.Services.PostServices.Interfaces;
using Amlakbashi.Application.Services.BlogPostServices.Interfaces;
using log4net;
using Amlakbashi.Application.Services.UserServices.Interfaces;
using Entities = Amlakbashi.Core.Entities;
using Amlakbashi.Application.Services.FileServices.Interfaces;
using Amlakbashi.Core.Common.Utilities;
using Amlakbashi.Application.Services.AdvertiseServices.Interfaces;
using Amlakbashi.Host.Controllers.Base;
using Amlakbashi.Host.Authentication;
using X.PagedList;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;
using Amlakbashi.Core.Identity;

namespace Portal.Controllers
{
    public class FileController : BaseController
    {
        private readonly IPostAppService postService;
        private readonly IBlogPostAppService blogPostService;
        private readonly IUserAppService userService;
        private readonly IFileAppService fileService;
        private readonly IAdvertiseAppService advertiseService;
        private readonly IUserAccessor userAccessor;
        private readonly IWebHostEnvironment host;
        private readonly ILog logger;
        private static readonly object objlock = new object();
        public FileController(IPostAppService postService,
            IBlogPostAppService blogPostService,
            IUserAppService userService,
            IFileAppService fileService,
            IAdvertiseAppService advertiseService,
            IUserAccessor userAccessor,
            IWebHostEnvironment host,
            ILog logger)
        {
            this.fileService = fileService;
            this.postService = postService;
            this.blogPostService = blogPostService;
            this.userService = userService;
            this.advertiseService = advertiseService;
            this.userAccessor = userAccessor;
            this.host = host;
            this.logger = logger;
        }

        [Authorize(Policy = Policies.Admin_General)]
        public ActionResult Index(int? page)
        {
            try
            {
                IEnumerable<Entities.File> model = fileService.GetAllDescendingByLastModifyDate();
                var PageNumber = page ?? 1;
                var onePageOfModel = model.ToPagedList(PageNumber, 20);
                ViewBag.RowIndexStart = (PageNumber * 20) - 20;
                return View(onePageOfModel);
            }
            catch (Exception exc)
            {
                logger.Error("File.Index", exc);
                return Redirect(Request.Headers["Referer"].ToString());
            }
        }

        [Authorize(Policy = Policies.Advertise_Edit)]
        public ActionResult Edit(int fid = -1)
        {
            try
            {
                ViewBag.msg = TempData["msg"];
                if (fid == -1)
                {
                    Entities.File model = new Entities.File();
                    model.Id = -1;
                    return View(model);
                }
                else
                {
                    Entities.File model = fileService.Find(fid);
                    return View(model);
                }
            }
            catch (Exception exc)
            {
                logger.Error("File.Edit(get)", exc);
                return Redirect(Request.Headers["Referer"].ToString());
            }
        }

        [Authorize(Policy = Policies.Advertise_Edit)]
        [HttpPost]
        public ActionResult Edit(Entities.File nfile)
        {
            try
            {
                string ext = "", filepath = "";
                int FileType = -1;
                if (Request.Form.Files.Count > 0 && Request.Form.Files[0].Length > 0)
                {
                    var uploadfile = Request.Form.Files[0];
                    ext = Path.GetExtension(uploadfile.FileName).ToLower();
                    if (ext == ".png" || ext == ".jpg" || ext == ".jpeg" || ext == ".gif")
                        FileType = (int)Entities.File.FileTypes.Image;
                    else if (ext == ".flv" || ext == ".mp4")
                        FileType = (int)Entities.File.FileTypes.Video;
                    else if (ext == ".mp3")
                        FileType = (int)Entities.File.FileTypes.Voice;
                    else if (ext == ".txt" || ext == ".pdf" || ext == ".doc" || ext == ".docx")
                        FileType = (int)Entities.File.FileTypes.File;
                    else if (ext == ".zip")
                        FileType = (int)Entities.File.FileTypes.zip;
                    else
                    {
                        TempData["msg"] = "فرمت فایل مورد قبول نمی باشد .";
                        return RedirectToAction("Edit", "File");
                    }

                    filepath = string.Format("~/content/files/file{0}{1}", Guid.NewGuid(), ext);
                    if (!Directory.Exists(Path.Combine(host.WebRootPath, "content/files")))
                        Directory.CreateDirectory(Path.Combine(host.WebRootPath, "content/files"));

                    using (Stream writer = new FileStream(Path.Combine(host.WebRootPath, filepath.Replace("~/", "")), FileMode.Create))
                    {
                        uploadfile.CopyTo(writer);
                    }

                    lock (objlock)
                    {
                        DirectoryInfo IOdirectory = new DirectoryInfo(Path.Combine(host.WebRootPath, "content/imgcache"));
                        foreach (System.IO.FileInfo IOfile in IOdirectory.GetFiles())
                        {
                            IOfile.Delete();
                        }
                    }
                }
                else if (nfile.Id == -1)
                {
                    TempData["msg"] = "لطفا یک فایل انتخاب کنید .";
                    return RedirectToAction("Edit", "File");
                }

                if (nfile.Id == -1)
                {
                    nfile.FilePath = filepath;
                    nfile.PostDate = DateTime.Now;
                    nfile.LastModifyDate = DateTime.Now;
                    nfile.UserID = userAccessor.CurrentUser.Id;
                    fileService.Insert(nfile);
                }
                else
                {
                    var file = fileService.Find(nfile.Id);
                    nfile.PostDate = file.PostDate;
                    nfile.LastModifyDate = DateTime.Now;
                    if (!string.IsNullOrEmpty(filepath))
                    {
                        nfile.FilePath = filepath;
                    }
                    fileService.Update(nfile, host.WebRootPath);
                }
                return RedirectToAction("Index");
            }
            catch (Exception exc)
            {
                logger.Error("File.Edit(post)", exc);
                return Redirect(Request.Headers["Referer"].ToString());
            }
        }

        [Authorize(Policy = Policies.Advertise_Edit)]
        public JsonResult Delete(int id)
        {
            try
            {
                fileService.Delete(id, host.WebRootPath);
                return GenerateJsonResult(new
                {
                    status = 1,
                    val = ""
                });
            }
            catch (Exception exc)
            {
                logger.Error("File.Delete", exc);
                return GenerateJsonResult(new
                {
                    status = 0,
                    val = ""
                });
            }
        }

        [ResponseCache(Duration = 60 * 60 * 24 * 365, Location = ResponseCacheLocation.Client, VaryByQueryKeys = new string[] { "*" })]
        public ActionResult GetImage(long FileID)
        {
            try
            {
                var objFile = fileService.Find(FileID);
                if (objFile == null)
                {
                    return File("/resource/img/img202_500_300.png", "image/png");
                }

                var extension = Path.GetExtension(objFile.FilePath).Replace(".", "");
                var strFormat = "image/" + (extension == "jpg" ? "jpeg" : extension);
                if (System.IO.File.Exists(host.WebRootPath + "/" + objFile.FilePathWithoutTildeAndSlash))
                {
                    return File(objFile.FilePath, strFormat);
                }
                return File("/resource/img/img202_500_300.png", "image/png");
            }
            catch (Exception exc)
            {
                logger.Error("File.GetImage", exc);
                return File("/resource/img/img202_500_300.png", "image/png");
            }
        }

        [ResponseCache(Duration = 60 * 60 * 24 * 365, Location = ResponseCacheLocation.Client, VaryByQueryKeys = new string[] { "*" })]
        public ActionResult imgThumb(long FileID, int w = 0, int h = 0)
        {
            try
            {
                var objFile = fileService.Find(FileID);
                if (objFile == null ||
                    System.IO.File.Exists(Path.Combine(host.WebRootPath, objFile.FilePathWithoutTildeAndSlash)) == false)
                {
                    return File("/resource/img/img202_500_300.png", "image/png");
                }

                string path = "", strFormat = "";
                using (Image tmpImage = Image.FromFile(Path.Combine(host.WebRootPath, objFile.FilePathWithoutTildeAndSlash)))
                {
                    if (tmpImage.RawFormat.Equals(ImageFormat.Png))
                    {
                        path = string.Format("content/imgcache/img{0}_{1}_{2}.png", FileID, w, h);
                        strFormat = "image/png";
                    }
                    else
                    {
                        path = string.Format("content/imgcache/img{0}_{1}_{2}.jpg", FileID, w, h);
                        strFormat = "image/jpeg";
                    }
                }

                if (System.IO.File.Exists(Path.Combine(host.WebRootPath, path)) == false)
                {
                    string OrginalPath = objFile.FilePathWithoutTildeAndSlash;
                    using (Image OriginalImage = Image.FromFile(Path.Combine(host.WebRootPath, OrginalPath)))
                    {
                        lock (objlock)
                        {
                            using (var result = (Bitmap)ImageUtility.ResizeImageKeepAspectRatio(OriginalImage, (int)w, (int)h))
                            {
                                result.Save(Path.Combine(host.WebRootPath, path), OriginalImage.RawFormat);
                            }
                        }
                    }
                }
                return File("/" + path, strFormat);
            }
            catch (Exception exc)
            {
                logger.Error("imgThumb", exc);
                if (FileID != 202)
                {
                    return RedirectToAction("imgThumb", new { FileID = 202, w = w, h = h });
                }
                else
                {
                    return File("/resource/img/img202_500_300.png", "image/png");
                }
            }
        }

        public ActionResult imgThumbOld(long FileID, int w = 0, int h = 0)
        {
            try
            {
                var objFile = fileService.Find(FileID);
                if (objFile == null ||
                    System.IO.File.Exists(Path.Combine(host.WebRootPath, objFile.FilePathWithoutTildeAndSlash)) == false)
                {
                    return File("/resource/img/img202_500_300.png", "image/png");
                }

                string path = "", strFormat = "";
                using (Image tmpImage = Image.FromFile(Path.Combine(host.WebRootPath, objFile.FilePathWithoutTildeAndSlash)))
                {
                    if (tmpImage.RawFormat.Equals(ImageFormat.Png))
                    {
                        path = string.Format("content/imgcache/img{0}_{1}_{2}.png", FileID, w, h);
                        strFormat = "image/png";
                    }
                    else
                    {
                        path = string.Format("content/imgcache/img{0}_{1}_{2}.jpg", FileID, w, h);
                        strFormat = "image/jpeg";
                    }
                }

                if (System.IO.File.Exists(Path.Combine(host.WebRootPath, path)) == false)
                {
                    string OrginalPath = objFile.FilePathWithoutTildeAndSlash;
                    using (Image OriginalImage = Image.FromFile(Path.Combine(host.WebRootPath, OrginalPath)))
                    {
                        double nw, nh, zz;
                        if (w == 0)
                        {
                            zz = (double)OriginalImage.Height / (double)h;
                            nh = h;
                            nw = (double)OriginalImage.Width / zz;
                        }
                        else if (h == 0)
                        {
                            zz = (double)OriginalImage.Width / (double)w;
                            nw = w;
                            nh = (double)OriginalImage.Height / zz;
                        }
                        else
                        {
                            nw = w;
                            nh = h;
                        }
                        lock (objlock)
                        {
                            using (Bitmap thumbnailBitmap = new Bitmap((int)nw, (int)nh))
                            {
                                thumbnailBitmap.SetResolution(OriginalImage.HorizontalResolution, OriginalImage.VerticalResolution);
                                using (Graphics thumbnailGraph = Graphics.FromImage(thumbnailBitmap))
                                {
                                    thumbnailGraph.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                                    thumbnailGraph.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                                    thumbnailGraph.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                                    Rectangle imageRectangle = new Rectangle(0, 0, (int)nw, (int)nh);
                                    thumbnailGraph.DrawImage(OriginalImage, imageRectangle);
                                    thumbnailBitmap.Save(Path.Combine(host.WebRootPath, path), OriginalImage.RawFormat);
                                }
                            }
                        }
                    }
                }
                return File("/" + path, strFormat);
            }
            catch (Exception exc)
            {
                logger.Error("File.imgThumbOld", exc);
                if (FileID != 202)
                {
                    return RedirectToAction("imgThumb", new { FileID = 202, w = w, h = h });
                }
                else
                {
                    return File("/resource/img/img202_500_300.png", "image/png");
                }
            }
        }

        [ResponseCache(Duration = 60 * 60 * 24 * 365, Location = ResponseCacheLocation.Client, VaryByQueryKeys = new string[] { "*" })]
        public ActionResult UserImageThumb(long FileID, float w = 80, float h = 80)
        {
            try
            {
                var objFile = fileService.Find(FileID);
                if (objFile == null ||
                    System.IO.File.Exists(Path.Combine(host.WebRootPath, objFile.FilePathWithoutTildeAndSlash)) == false)
                {
                    return Redirect(HtmlUtility.EncodeUrlForRedirect(string.Format("/عکس-یافت-نشد-{0}-{1}", w, h)));
                }

                string path = "", strFormat = "";
                using (Image tmpImage = Image.FromFile(Path.Combine(host.WebRootPath, objFile.FilePathWithoutTildeAndSlash)))
                {
                    if (tmpImage.RawFormat.Equals(ImageFormat.Png))
                    {
                        path = string.Format("content/imgcache/img{0}_{1}_{2}.png", FileID, w, h);
                        strFormat = "image/png";
                    }
                    else
                    {
                        path = string.Format("content/imgcache/img{0}_{1}_{2}.jpg", FileID, w, h);
                        strFormat = "image/jpeg";
                    }
                }

                if (System.IO.File.Exists(Path.Combine(host.WebRootPath, path)) == false)
                {
                    using (Image OriginalImage = Image.FromFile(Path.Combine(host.WebRootPath, objFile.FilePathWithoutTildeAndSlash)))
                    {
                        double nw, nh, zz;
                        if (w == 0)
                        {
                            zz = (double)OriginalImage.Height / (double)h;
                            nh = h;
                            nw = (double)OriginalImage.Width / zz;
                        }
                        else if (h == 0)
                        {
                            zz = (double)OriginalImage.Width / (double)w;
                            nw = w;
                            nh = (double)OriginalImage.Height / zz;
                        }
                        else
                        {
                            nw = w;
                            nh = h;
                        }
                        lock (objlock)
                        {
                            using (Bitmap thumbnailBitmap = new Bitmap((int)nw, (int)nh))
                            {
                                thumbnailBitmap.SetResolution(OriginalImage.HorizontalResolution, OriginalImage.VerticalResolution);
                                using (Graphics thumbnailGraph = Graphics.FromImage(thumbnailBitmap))
                                {
                                    thumbnailGraph.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                                    thumbnailGraph.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                                    thumbnailGraph.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                                    Rectangle imageRectangle = new Rectangle(0, 0, (int)nw, (int)nh);
                                    thumbnailGraph.DrawImage(OriginalImage, imageRectangle);
                                    thumbnailBitmap.Save(Path.Combine(host.WebRootPath, path), OriginalImage.RawFormat);
                                }
                            }
                        }
                    }
                }
                return File(path.Replace("~", ""), strFormat);
            }
            catch (Exception exc)
            {
                logger.Error("File.UserImageThumb", exc);
                return Redirect(HtmlUtility.EncodeUrlForRedirect(string.Format("/عکس-یافت-نشد-{0}-{1}", w, h)));
            }
        }

        public ActionResult AdvertiseImageThumbLarge(string slug)
        {
            return AdvertiseImageThumb(slug, 1000, 300);
        }

        public ActionResult AdvertiseImageThumb(string slug, int w = 240, int h = 144)
        {
            try
            {
                if (string.IsNullOrEmpty(slug))
                {
                    return Redirect(HtmlUtility.EncodeUrlForRedirect(string.Format("/عکس-یافت-نشد-{0}-{1}", w, h)));
                }
                var advertise_id = long.Parse(slug.Split('-')[0]);
                var advertise = advertiseService.FindIncludingDeleted(advertise_id);
                if (advertise == null || advertise.Slug != slug)
                {
                    return Redirect(HtmlUtility.EncodeUrlForRedirect(string.Format("/عکس-یافت-نشد-{0}-{1}", w, h)));
                }
                var objFile = fileService.Find(advertise.PhotoID == null ? 0 : (long)advertise.PhotoID);
                if (objFile == null || !System.IO.File.Exists(Path.Combine(host.WebRootPath, objFile.FilePathWithoutTildeAndSlash)))
                {
                    return Redirect(HtmlUtility.EncodeUrlForRedirect(string.Format("/عکس-یافت-نشد-{0}-{1}", w, h)));
                }
                var image_extension = Path.GetExtension(objFile.FilePath).Replace(".", "");
                var path = string.Format("content/imgcache/img{0}_{1}_{2}." + image_extension, advertise.PhotoID, w, h);
                var strFormat = "image/" + image_extension;

                if (!System.IO.File.Exists(Path.Combine(host.WebRootPath, path)))
                {
                    using (Image OriginalImage = Image.FromFile(Path.Combine(host.WebRootPath, objFile.FilePathWithoutTildeAndSlash)))
                    {
                        lock (objlock)
                        {
                            using (var result = (Bitmap)ImageUtility.ResizeImageKeepAspectRatio(OriginalImage, (int)w, (int)h))
                            {
                                result.Save(Path.Combine(host.WebRootPath, path), OriginalImage.RawFormat);
                            }
                        }
                    }
                }
                return File(path.Replace("~", ""), strFormat);
            }
            catch (Exception exc)
            {
                logger.Error("File.AdvertiseImageThumb", exc);
                return Redirect(HtmlUtility.EncodeUrlForRedirect(string.Format("/عکس-یافت-نشد-{0}-{1}", w, h)));
            }
        }

        [ResponseCache(Duration = 60 * 60 * 24 * 365, Location = ResponseCacheLocation.Client, VaryByQueryKeys = new string[] { "*" })]
        public ActionResult PostImageThumb(string slug, long PostID, int w = 0, int h = 0)
        {
            try
            {
                var post = postService.Find(PostID);
                if (post.Title.Replace("+", "-").Replace(" ", "-") != slug)
                {
                    return Redirect(HtmlUtility.EncodeUrlForRedirect(string.Format("/عکس-یافت-نشد-{0}-{1}", w, h)));
                }
                return imgThumb(post.PhotoID, w, h);
            }
            catch (Exception exc)
            {
                logger.Error("File.PostImageThumb", exc);
                return Redirect(HtmlUtility.EncodeUrlForRedirect(string.Format("/عکس-یافت-نشد-{0}-{1}", w, h)));
            }
        }

        [ResponseCache(Duration = 60 * 60 * 24 * 365, Location = ResponseCacheLocation.Client, VaryByQueryKeys = new string[] { "*" })]
        public ActionResult ImageNotFound(int w, int h)
        {
            if (w == 1000 && h == 300)
            {
                return File("/resource/img/image-not-found-wide.png", "image/png");
            }
            else if (w == h)
            {
                return File("/resource/img/image-not-found-square.png", "image/png");
            }
            return File("/resource/img/img202_500_300.png", "image/png");
        }

        public ActionResult PhotoCropper(long id, long accId = 0)
        {
            try
            {
                var file = fileService.Find(id);
                if (file == null)
                {
                    return Redirect(Request.Headers["Referer"].ToString());
                }

                using (Image image = Image.FromFile(Path.Combine(host.WebRootPath, file.FilePathWithoutTildeAndSlash)))
                {
                    if (image.Width > 1500)
                        ViewBag.BigImage = true;
                }
                ViewBag.accId = accId;
                return View(file);
            }
            catch (Exception exc)
            {
                logger.Error("File.PhotoCropper", exc);
                return Redirect(Request.Headers["Referer"].ToString());
            }
        }

        public JsonResult SavePhotoCropper(int id, long accId)
        {
            try
            {
                if (Request.Form.Files.Count > 0 && Request.Form.Files[0].Length > 0)
                {
                    lock (objlock)
                    {
                        Entities.File ObjFile = fileService.Find(id).Clone();
                        var uploadfile = Request.Form.Files[0];

                        string directoryPath = ObjFile.FilePathWithoutTildeAndSlash.Substring(0,
                            ObjFile.FilePathWithoutTildeAndSlash.LastIndexOf('/') + 1);
                        var filepath = Path.Combine(host.WebRootPath, ObjFile.FilePathWithoutTildeAndSlash);

                        if (Directory.Exists(Path.Combine(host.WebRootPath, directoryPath)) == false)
                            Directory.CreateDirectory(Path.Combine(host.WebRootPath, directoryPath));

                        var format = ImageUtility.GetEncoder(ImageFormat.Jpeg);
                        var encoderParameters = new EncoderParameters(1);
                        encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, 100L);

                        using (var imageToSave = Image.FromStream(uploadfile.OpenReadStream(), true, true))
                        {
                            imageToSave.Save(Path.Combine(host.WebRootPath, filepath), format, encoderParameters);
                        }

                        ObjFile.LastModifyDate = DateTime.Now;
                        fileService.Update(ObjFile, host.WebRootPath);

                        if (accId > 0)
                        {
                            fileService.GenerateThumbImage(accId, id);
                        }

                        DirectoryInfo IOdirectory = new DirectoryInfo(Path.Combine(host.WebRootPath, "content/imgcache"));
                        foreach (FileInfo IOfile in IOdirectory.GetFiles())
                        {
                            IOfile.Delete();
                        }
                    }
                }
                return GenerateJsonResult(new { status = 1 });
            }
            catch (Exception exc)
            {
                logger.Error("File.SavePhontoCropper", exc);
                return GenerateJsonResult(new { status = 0, message = "فرمت عکس مورد قبول نمی باشد ." });
            }
        }

        [ResponseCache(Duration = 60 * 60 * 24 * 365, Location = ResponseCacheLocation.Client, VaryByQueryKeys = new string[] { "*" })]
        public ActionResult AccThumbCard(long accid, long fileid)
        {
            return AccThumb(accid, fileid, "card");
        }

        [ResponseCache(Duration = 60 * 60 * 24 * 365, Location = ResponseCacheLocation.Client, VaryByQueryKeys = new string[] { "*" })]
        public ActionResult AccThumbXSmall(long accid, long fileid)
        {
            return AccThumb(accid, fileid, "xsmall");
        }

        [ResponseCache(Duration = 60 * 60 * 24 * 365, Location = ResponseCacheLocation.Client, VaryByQueryKeys = new string[] { "*" })]
        public ActionResult AccThumbSmall(long accid, long fileid)
        {
            return AccThumb(accid, fileid, "small");
        }

        [ResponseCache(Duration = 60 * 60 * 24 * 365, Location = ResponseCacheLocation.Client, VaryByQueryKeys = new string[] { "*" })]
        public ActionResult AccThumbMedium(long accid, long fileid)
        {
            return AccThumb(accid, fileid, "medium");
        }

        [ResponseCache(Duration = 60 * 60 * 24 * 365, Location = ResponseCacheLocation.Client, VaryByQueryKeys = new string[] { "*" })]
        public ActionResult AccThumbLarge(long accid, long fileid)
        {
            return AccThumb(accid, fileid, "large");
        }

        [ResponseCache(Duration = 60 * 60 * 24 * 365, Location = ResponseCacheLocation.Client, VaryByQueryKeys = new string[] { "*" })]
        public ActionResult AccThumbXLarge(long accid, long fileid)
        {
            return AccThumb(accid, fileid, "xlarge");
        }

        [ResponseCache(Duration = 60 * 60 * 24 * 365, Location = ResponseCacheLocation.Client, VaryByQueryKeys = new string[] { "*" })]
        public ActionResult AccThumbXXLarge(long accid, long fileid)
        {
            return AccThumb(accid, fileid, "xxlarge");
        }

        [ResponseCache(Duration = 60 * 60 * 24 * 365, Location = ResponseCacheLocation.Client, VaryByQueryKeys = new string[] { "*" })]
        public ActionResult AccThumbXXXLarge(long accid, long fileid)
        {
            return AccThumb(accid, fileid, "xxxlarge");
        }

        [ResponseCache(Duration = 60 * 60 * 24 * 365, Location = ResponseCacheLocation.Client, VaryByQueryKeys = new string[] { "*" })]
        public ActionResult AccThumb(long accid, long fileid, string filename)
        {
            var path = "content/accthumb/" + accid + "/" + fileid + "/" + filename + ".jpg";
            if (!System.IO.File.Exists(Path.Combine(host.WebRootPath, path)))
            {
                return File("/resource/img/img202_500_300.png", "image/png");
            }
            return File("/" + path, "image/jpeg");
        }

        [ResponseCache(Duration = 60 * 60 * 24 * 365, Location = ResponseCacheLocation.Client)]
        public ActionResult Logo()
        {
            return File("/resource/img/logo.gif", "image/gif");
        }

        [ResponseCache(Duration = 60 * 60 * 24 * 365, Location = ResponseCacheLocation.Client, VaryByQueryKeys = new string[] { "*" })]
        public ActionResult HomePageSlider(int number, string format = "webp")
        {
            var userAgent = Request.Headers["User-Agent"].ToString();
            if (userAgent.Contains("Chrome") == true && userAgent.Contains("Edge") == false)
            {
                return File("/resource/img/home_page_slider_" + number + "." + format, "image/" + format);
            }
            return File("/resource/img/home_page_slider_" + number + ".jpg", "image/jpeg");
        }

        [ResponseCache(Duration = 60 * 60 * 24 * 365, Location = ResponseCacheLocation.Client, VaryByQueryKeys = new string[] { "file_name" })]
        public ActionResult ResourceImage(string file_name)
        {
            return File("/resource/img/" + file_name + ".jpg", "image/jpeg");
        }

        [ResponseCache(Duration = 60 * 60 * 24 * 365, Location = ResponseCacheLocation.Client, VaryByQueryKeys = new string[] { "file_name" })]
        public ActionResult ResourceImageWebp(string file_name)
        {
            var userAgent = Request.Headers["User-Agent"].ToString();
            if (userAgent.Contains("Chrome") == true && userAgent.Contains("Edge") == false)
            {
                return File("/resource/img/" + file_name + ".webp", "image/webp");
            }
            return ResourceImage(file_name);
        }

        [ResponseCache(Duration = 60 * 60 * 24 * 365, Location = ResponseCacheLocation.Client, VaryByQueryKeys = new string[] { "file_name" })]
        public ActionResult ResourceImagePNG(string file_name)
        {
            return File("/resource/img/" + file_name + ".png", "image/png");
        }

        [ResponseCache(Duration = 60 * 60 * 24 * 365, Location = ResponseCacheLocation.Client, VaryByQueryKeys = new string[] { "file_name" })]
        public ActionResult ResourceImageGIF(string file_name)
        {
            return File("/resource/img/" + file_name + ".gif", "image/gif");
        }

        [ResponseCache(Duration = 60 * 60 * 24 * 365, Location = ResponseCacheLocation.Client, VaryByQueryKeys = new string[] { "file_name" })]
        public ActionResult Loading()
        {
            return File("/resource/img/indicator.white.gif", "image/gif");
        }

        [ResponseCache(Duration = 60 * 60 * 24 * 365, Location = ResponseCacheLocation.Client, VaryByQueryKeys = new string[] { "*" })]
        public ActionResult GetFile(string file_name, string type)
        {
            return File(file_name, type);
        }

        [ResponseCache(Duration = 60 * 60 * 24 * 365, Location = ResponseCacheLocation.Client, VaryByQueryKeys = new string[] { "*" })]
        public ActionResult GetPngFile(string file_name)
        {
            return File(file_name, "image/png");
        }

        [ResponseCache(Duration = 60 * 60 * 24 * 365, Location = ResponseCacheLocation.Client, VaryByQueryKeys = new string[] { "*" })]
        public ActionResult GetWoff2File(string file_name)
        {
            return File(file_name, "font/woff2");
        }

        [ResponseCache(Duration = 60 * 60 * 24 * 365, Location = ResponseCacheLocation.Client, VaryByQueryKeys = new string[] { "*" })]
        public ActionResult GetGifFile(string file_name)
        {
            return File(file_name, "image/gif");
        }

        [ResponseCache(Duration = 60 * 60 * 24 * 365, Location = ResponseCacheLocation.Client, VaryByQueryKeys = new string[] { "*" })]
        public ActionResult GetCss(string src, int v = 0)
        {
            return File(src, "text/css");
        }

        [ResponseCache(Duration = 60 * 60 * 24 * 365, Location = ResponseCacheLocation.Client, VaryByQueryKeys = new string[] { "*" })]
        public ActionResult GetJS(string src, int v = 0)
        {
            return File(src, "text/javascript");
        }

        [HttpPost]
        public JsonResult UploadPhoto()
        {
            if (Request.Form.Files.Count < 1)
            {
                return GenerateJsonResult(new
                {
                    success = false,
                    error = "خطای بارگذاری فایل"
                });
            }
            var image = Request.Form.Files[0];
            try
            {
                string msg;
                string file_path = "~/content/blogpost/blogpost";
                if (image == null)
                {
                    msg = "خطا در دریافت عکس";
                    return GenerateJsonResult(new
                    {
                        success = false,
                        error = msg
                    });
                }

                string ext;
                ext = Path.GetExtension(image.FileName).ToLower();
                if (!(ext == ".png" || ext == ".jpg" || ext == ".gif"))
                {
                    msg = "فرمت عکس مورد قبول نمی باشد";
                    return GenerateJsonResult(new
                    {
                        success = false,
                        error = msg
                    });
                }
                file_path += (Guid.NewGuid() + ext);
                if (!Directory.Exists(Path.Combine(host.WebRootPath, "content/blogpost")))
                    Directory.CreateDirectory(Path.Combine(host.WebRootPath, "content/blogpost"));

                using (Stream stream = new FileStream(Path.Combine(host.WebRootPath, file_path.Replace("~/", "")), FileMode.Create))
                {
                    image.CopyTo(stream);
                }

                var nfile = new Entities.File();
                nfile.FilePath = file_path;
                nfile.PostDate = DateTime.Now;
                nfile.LastModifyDate = DateTime.Now;
                nfile.UserID = userAccessor.CurrentUser.Id;
                var fileId = fileService.Insert(nfile);
                msg = null;

                if (fileId < 1)
                {
                    return GenerateJsonResult(new
                    {
                        success = false,
                        error = msg
                    });
                }
                return GenerateJsonResult(new
                {
                    success = true,
                    id = fileId
                });
            }
            catch (Exception exc)
            {
                logger.Error("File.UploadPhoto", exc);
                return GenerateJsonResult(new
                {
                    success = false,
                    error = "خطای بارگذاری فایل"
                });
            }
        }

        private static System.Threading.Thread minifyThread;

        public JsonResult GetUploadedPhoto(long id)
        {
            var file = fileService.Find(id);
            return GenerateJsonResult(new
            {
                name = "",
                uuid = 0,
                thumbnailUrl = "/file/getphoto?id=" + id
            });
        }

        public ActionResult GetPhoto(long id)
        {
            var objFile = fileService.Find(id);
            var extension = Path.GetExtension(objFile.FilePath).Replace(".", "");
            var strFormat = "image/" + (extension == "jpg" ? "jpeg" : extension);
            return File(objFile.FilePath, strFormat);
        }

        [Authorize]
        [HttpPost]
        public JsonResult SaveUploadedFile(long accId)
        {
            try
            {
                if (Request.Form.Files.Count > 0 && Request.Form.Files[0].Length > 0)
                {
                    var uploadfile = Request.Form.Files[0];
                    var contentType = uploadfile.ContentType.ToLower();
                    if ((contentType == "image/png" ||
                        contentType == "image/gif" ||
                        contentType == "image/jpg" ||
                        contentType == "image/jpeg") == false)
                    {
                        return GenerateJsonResult(new { Status = 0, Message = "فرمت عکس مورد قبول نمی باشد" });
                    }

                    var quality = 80;
                    var maxWidth = 1024;

                    var ObjFile = new Entities.File();
                    ObjFile.PostDate = DateTime.Now;
                    ObjFile.UserID = userAccessor.CurrentUser.Id;
                    ObjFile.LastModifyDate = DateTime.Now;
                    ObjFile.MinifyStatus = Entities.File.MinifyStatusEnum.Done;
                    ObjFile.MinifyQualityPercent = quality;
                    ObjFile.MinifyMaxWidth = maxWidth;
                    var photoID = fileService.Insert(ObjFile);

                    if (Directory.Exists(host.WebRootPath + "/content/advertise") == false)
                        Directory.CreateDirectory(host.WebRootPath + "/content/advertise");

                    var filepath = $"~/content/advertise/advertise_{accId}_{photoID}.jpg";
                    ImageCodecInfo format = ImageUtility.GetEncoder(ImageFormat.Jpeg);
                    EncoderParameters encoderParameters = new EncoderParameters(1);
                    encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, quality);
                    var image = Image.FromStream(uploadfile.OpenReadStream(), true, true);
                    image = ImageUtility.MinifyImage(image, maxWidth);
                    image.Save(host.WebRootPath + filepath.Replace("~", ""), format, encoderParameters);

                    fileService.UpdateFilePath(photoID, filepath);
                    return GenerateJsonResult(new { Status = 1, id = photoID });
                }
                return Json(new { Status = 0, Message = "خطا در دریافت فایل، لطفا دوباره امتحان کنید" });
            }
            catch (Exception exc)
            {
                logger.Error("Post.SaveUploadedFile", exc);
                return GenerateJsonResult(new { Status = 0, Message = "عملیات با خطا مواجه شد" });
            }
        }
    }
}


