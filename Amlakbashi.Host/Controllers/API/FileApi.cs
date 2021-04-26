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

        //[ResponseCache(Duration = 604800, VaryByQueryKeys = new string[] { "*" })]
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
                if (id != 202 && (objFile == null || !System.IO.File.Exists(webHostEnvironment.WebRootPath + objFile.FilePath.Replace("~", ""))))
                {
                    return GetImage(202, w, h, cid);
                }
                var image_extension = Path.GetExtension(objFile.FilePath).Replace(".", "");
                var path = string.Format("~/content/imgcache/img{0}_{1}_{2}." + image_extension, id, w, h);
                var strFormat = "image/" + image_extension;

                if (!System.IO.File.Exists(webHostEnvironment.WebRootPath + path.Replace("~", "")))
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
                logger.Error("", exc);
                if (id != 202)
                {
                    return GetImage(202, w, h, cid);
                }
                return File("/resource/img/img202_500_300.png", "image/png");
            }
        }

        //[ResponseCache(Duration = 604800, VaryByQueryKeys = new string[] { "*" })]
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

        //[ResponseCache(Duration = 86400, VaryByQueryKeys = new string[] { "*" })]
        public ActionResult GetAdvertiseItemImage(long id, string cid, long accid = 0)
        {
            if (accid > 0)
            {
                return AccThumb(accid, id, "appcard");
            }
            return GetImage(id, 160, 114, cid);
        }
        //[ResponseCache(Duration = 86400, VaryByQueryKeys = new string[] { "*" })]
        public ActionResult GetAdvertiseImage(long id, string cid, long accid = 0)
        {
            if (accid > 0)
            {
                return AccThumb(accid, id, "appcarousel");
            }
            return GetImage(id, 450, 300, cid);
        }
        //[ResponseCache(Duration = 86400, VaryByQueryKeys = new string[] { "*" })]
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
                string msg;
                string file_path = "~/content/users/user";
                if (image == null)
                {
                    msg = "خطا در دریافت عکس";
                    return GenerateJsonResult(new
                    {
                        done = false,
                        msg = msg
                    });
                }
                string ext;
                ext = Path.GetExtension(image.FileName).ToLower();
                if (!(ext == ".png" || ext == ".jpg" || ext == ".gif"))
                {
                    msg = "فرمت عکس مورد قبول نمی باشد";
                    return GenerateJsonResult(new
                    {
                        done = false,
                        msg = msg
                    });
                }
                file_path += (Guid.NewGuid() + ext);
                if (!Directory.Exists(webHostEnvironment.WebRootPath + "/content/users"))
                    Directory.CreateDirectory(webHostEnvironment.WebRootPath + "/content/users");
                using (var stream = System.IO.File.Create(
                    webHostEnvironment.WebRootPath + file_path.Replace("~", "")))
                {
                    image.CopyTo(stream);
                }

                var nfile = new Entities.File();
                nfile.FilePath = file_path;
                nfile.PostDate = DateTime.Now;
                nfile.LastModifyDate = DateTime.Now;
                nfile.UserID = user.Id;
                var file_id = fileService.Insert(nfile);
                msg = null;

                if (file_id < 1)
                {
                    return GenerateJsonResult(new
                    {
                        done = false,
                        msg = msg
                    });
                }
                user.PhotoID = file_id;
                userService.UpdateProfilePhoto(user.Id, user.PhotoID == null ? 0 : (long)user.PhotoID, Entities.User.UserPhotoState.ready_publish);
                return GenerateJsonResult(new
                {
                    done = true,
                    id = file_id
                });
            }
            catch (Exception exc)
            {
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
                var quality = 80;
                var maxWidth = 1024;
                long photoID = -1;
                if (image != null)
                {
                    string extension = "", filepath = "";
                    image = Request.Form.Files[0];
                    extension = System.IO.Path.GetExtension(image.FileName).ToLower();

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
                            encoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);
                            var imageToSave = Image.FromStream(image.OpenReadStream(), true, true);
                            imageToSave = ImageUtility.MinifyImage(imageToSave, maxWidth);
                            imageToSave.Save(webHostEnvironment.WebRootPath + filepath.Replace("~", ""), format, encoderParameters);
                            break;
                        default:
                            return GenerateJsonResult(new
                            {
                                done = false,
                                msg = "فرمت عکس مورد قبول نمی باشد"
                            });
                    }

                    Entities.File ObjFile = new Entities.File();
                    ObjFile.FilePath = filepath;
                    ObjFile.PostDate = DateTime.Now;
                    ObjFile.UserID = user.Id;
                    ObjFile.LastModifyDate = DateTime.Now;
                    ObjFile.MinifyStatus = Entities.File.MinifyStatusEnum.Done;
                    ObjFile.MinifyQualityPercent = quality;
                    ObjFile.MinifyMaxWidth = maxWidth;
                    photoID = fileService.Insert(ObjFile);
                }

                if (photoID < 1)
                {
                    return GenerateJsonResult(new
                    {
                        done = false,
                        msg = "خطا در دریافت فایل، لطفا دوباره امتحان کنید"
                    });
                }

                return GenerateJsonResult(new
                {
                    done = true,
                    id = photoID
                });
            }
            catch (Exception exc)
            {
                return GenerateJsonResult(new
                {
                    done = false,
                    msg = "متاسفانه عملیات با خطا مواجه شد"
                });
            }
        }
    }
}

