using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using Entities = Amlakbashi.Core.Entities;
using Amlakbashi.Core.Common.Utilities;
using Amlakbashi.Host.Controllers.Base;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

namespace Amlakbashi.Host.Controllers.API
{
    public partial class ApiController : Controller
    {
        private static readonly object objlock = new object();

        [ResponseCache(Duration = 604800, VaryByQueryKeys = new string[] { "*" })]
        public ActionResult GetImage(long id, int w, int h, string cid)
        {
            if (!ClientAuthenticate(cid))
            {
                return null;
            }
            try
            {
                Entities.File objFile;
                if (id == 202)
                {
                    objFile = new Entities.File()
                    {
                        FilePath = "/resource/img/img202_500_300.png"
                    };
                }
                else
                {
                    objFile = fileService.Find(id);
                }
                if (id != 202 && (objFile == null || System.IO.File.Exists(webHostEnvironment.WebRootPath + objFile.FilePath.Replace("~", "")) == false))
                {
                    return GetImage(202, w, h, cid);
                }
                var image_extension = Path.GetExtension(objFile.FilePath).Replace(".", "");
                var path = string.Format("~/content/imgcache/img{0}_{1}_{2}." + image_extension, id, w, h);
                var strFormat = "image/" + image_extension;

                if (System.IO.File.Exists(webHostEnvironment.WebRootPath + path.Replace("~", "")) == false)
                {
                    using (Image OriginalImage = Image.FromFile(webHostEnvironment.WebRootPath + objFile.FilePath.Replace("~", "")))
                    {
                        lock (objlock)
                        {
                            using (var result = (Bitmap)ImageUtility.ResizeImageKeepAspectRatio(OriginalImage, (int)w, (int)h))
                            {
                                result.Save(webHostEnvironment.WebRootPath + path.Replace("~", ""), OriginalImage.RawFormat);
                                return File(path.Replace("~", ""), strFormat);
                            }
                        }
                    }
                }
                else
                {
                    return File(path.Replace("~", ""), strFormat);
                }
            }
            catch (Exception exc)
            {
                logger.Error("FileApi.GetImage", exc);
                if (id != 202)
                {
                    return GetImage(202, w, h, cid);
                }
                return File("/resource/img/img202_500_300.png", "image/png");
            }
        }

        [ResponseCache(Duration = 604800, VaryByQueryKeys = new string[] { "*" })]
        public ActionResult GetImageRealSize(long id, string cid)
        {
            if (!ClientAuthenticate(cid))
            {
                return null;
            }
            try
            {
                if (id == 202)
                {
                    return File("/resource/img/img202_500_300.png", "image/png");
                }
                var objFile = fileService.Find(id);
                if (id != 202 && (objFile == null || !System.IO.File.Exists(webHostEnvironment.WebRootPath + objFile.FilePath.Replace("~", ""))))
                {
                    return GetImageRealSize(202, cid);
                }
                var image_extension = Path.GetExtension(objFile.FilePath).Replace(".", "");
                var strFormat = "image/" + image_extension;
                return File(objFile.FilePath, strFormat);
            }
            catch (Exception exc)
            {
                logger.Error("", exc);
                if (id != 202)
                {
                    return GetImageRealSize(202, cid);
                }
                return File("/resource/img/img202_500_300.png", "image/png");
            }
        }

        [ResponseCache(Duration = 86400, VaryByQueryKeys = new string[] { "*" })]
        public ActionResult GetAdvertiseItemImage(long id, string cid, long accid = 0)
        {
            if (accid > 0)
            {
                if (id == 0)
                {
                    return GetImage(202, 160, 114, cid);
                }
                return AccThumb(accid, id, "appcard");
            }
            return GetImage(id, 160, 114, cid);
        }

        [ResponseCache(Duration = 86400, VaryByQueryKeys = new string[] { "*" })]
        public ActionResult GetAdvertiseImage(long id, string cid, long accid = 0)
        {
            if (accid > 0)
            {
                return AccThumb(accid, id, "appcarousel");
            }
            return GetImage(id, 450, 300, cid);
        }

        [ResponseCache(Duration = 86400, VaryByQueryKeys = new string[] { "*" })]
        public ActionResult AccThumb(long accid, long fileid, string filename)
        {
            var path = "/content/accthumb/" + accid + "/" + fileid + "/" + filename + ".jpg";
            return File(path, "image/jpeg");
        }

        public ActionResult GetSquareMediumImage(long id, string cid)
        {
            return GetImage(id, 320, 320, cid);
        }

        public ActionResult GetProfileSmallImage(long id, string cid)
        {
            return GetImage(id, 40, 40, cid);
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = bearerScheme)]
        public JsonResult SetProfileImage(string cid, IFormFile image)
        {
            if (!ClientAuthenticate(cid))
            {
                return null;
            }
            try
            {
                var user = GetUser();
                if (image == null)
                {
                    return GenerateJsonResult(new
                    {
                        done = false,
                        msg = "خطا در دریافت عکس"
                    });
                }

                var contentType = image.ContentType.ToLower();
                if ((contentType == "image/png" ||
                     contentType == "image/gif" ||
                     contentType == "image/jpg" ||
                     contentType == "image/jpeg") == false)
                {
                    return GenerateJsonResult(new
                    {
                        done = false,
                        msg = "فرمت عکس مورد قبول نمی باشد"
                    });
                }

                var filePath = $"~/content/users/user_{user.Id}.jpg";
                long photoID = 0;
                if (user.Photo != null)
                {
                    photoID = user.Photo.Id;
                    var oldFilePath = webHostEnvironment.WebRootPath + user.Photo.FilePath.Replace("~", "");
                    fileService.UpdateFilePath(photoID, filePath);
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }
                else
                {
                    var newProfilePhoto = new Entities.File()
                    {
                        PostDate = DateTime.Now,
                        LastModifyDate = DateTime.Now,
                        UserID = user.Id,
                        FilePath = filePath
                    };
                    photoID = fileService.Insert(newProfilePhoto);
                }

                if (!Directory.Exists(webHostEnvironment.WebRootPath + "/content/users"))
                    Directory.CreateDirectory(webHostEnvironment.WebRootPath + "/content/users");

                using (var stream = System.IO.File.Create(
                    webHostEnvironment.WebRootPath + filePath.Replace("~", "")))
                {
                    image.CopyTo(stream);
                }

                userService.UpdateProfilePhoto(user.Id, photoID, Entities.User.UserPhotoState.ready_publish);
                return GenerateJsonResult(new
                {
                    done = true,
                    id = photoID
                });
            }
            catch (Exception exc)
            {
                logger.Error("FileApi.SetProfileImage", exc);
                return GenerateJsonResult(new
                {
                    done = false,
                    msg = "متاسفانه عملیات با خطا مواجه شد"
                });
            }
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = bearerScheme)]
        public JsonResult UploadAdvertiseImage(string cid, IFormFile image)
        {
            if (!ClientAuthenticate(cid))
            {
                return null;
            }
            try
            {
                var user = GetUser();
                var files = Request.Form.Files;
                if (image != null)
                {
                    var contentType = image.ContentType.ToLower();
                    if ((contentType == "image/png" ||
                        contentType == "image/gif" ||
                        contentType == "image/jpg" ||
                        contentType == "image/jpeg") == false)
                    {
                        return GenerateJsonResult(new
                        {
                            done = false,
                            msg = "فرمت عکس مورد قبول نمی باشد"
                        });
                    }

                    var quality = 80;
                    var maxWidth = 1024;

                    Entities.File ObjFile = new Entities.File();
                    ObjFile.PostDate = DateTime.Now;
                    ObjFile.UserID = user.Id;
                    ObjFile.LastModifyDate = DateTime.Now;
                    ObjFile.MinifyStatus = Entities.File.MinifyStatusEnum.Done;
                    ObjFile.MinifyQualityPercent = quality;
                    ObjFile.MinifyMaxWidth = maxWidth;
                    long photoID = fileService.Insert(ObjFile);

                    var filepath = $"~/content/advertise/advertise_{photoID}.jpg";

                    if (Directory.Exists(webHostEnvironment.WebRootPath + "/content/advertise") == false)
                        Directory.CreateDirectory(webHostEnvironment.WebRootPath + "/content/advertise");

                    ImageCodecInfo format = ImageUtility.GetEncoder(ImageFormat.Jpeg);
                    EncoderParameters encoderParameters = new EncoderParameters(1);
                    encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, quality);
                    var imageToSave = Image.FromStream(image.OpenReadStream(), true, true);
                    imageToSave = ImageUtility.MinifyImage(imageToSave, maxWidth);
                    imageToSave.Save(webHostEnvironment.WebRootPath + filepath.Replace("~", ""), format, encoderParameters);

                    fileService.UpdateFilePath(photoID, filepath);

                    return GenerateJsonResult(new
                    {
                        done = true,
                        id = photoID
                    });
                }

                return GenerateJsonResult(new
                {
                    done = false,
                    msg = "خطا در دریافت فایل، لطفا دوباره امتحان کنید"
                });
            }
            catch (Exception exc)
            {
                logger.Error("FileApi.UploadAdvertiseImage", exc);
                return GenerateJsonResult(new
                {
                    done = false,
                    msg = "متاسفانه عملیات با خطا مواجه شد"
                });
            }
        }
    }
}

