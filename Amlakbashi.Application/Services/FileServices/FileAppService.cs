using Amlakbashi.Core.Common.AppService;
using Amlakbashi.Core.Common.Repository;
using Amlakbashi.Application.Services.FileServices.Interfaces;
using Amlakbashi.Core.Entities;
using System.Collections.Generic;
using System.Linq;
using MediatR;
using Amlakbashi.Mediator.Commands.AdvertiseCommands;
using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.Threading.Tasks;
using Amlakbashi.Core.DTOs.WebService.Requests.Files;
using Amlakbashi.Core.Common.Utilities;
using System.Drawing.Imaging;
using System.Drawing;
using Amlakbashi.Application.DTOs;
using Amlakbashi.Mediator.Commands.FileCommands;
using log4net;

namespace Amlakbashi.Application.Services.FileServices
{
    internal class FileAppService : AppServiceBase<File, long>, IFileAppService
    {
        private readonly IMediator mediator;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly ILog logger;
        public FileAppService(IRepository<File, long> repository,
            IMediator mediator,
            IWebHostEnvironment webHostEnvironment,
            ILog logger) : base(repository)
        {
            this.mediator = mediator;
            this.webHostEnvironment = webHostEnvironment;
            this.logger = logger;
        }

        public IList<File> GetAllDescendingByLastModifyDate(int count = 0)
        {
            if (count == 0)
            {
                return Repository.Query(q => q.OrderByDescending(o => o.LastModifyDate).ToList());
            }
            return Repository.Query(q => q.OrderByDescending(o => o.LastModifyDate).Take(count).ToList());
        }

        public File Find(long id)
        {
            return Repository.Query(q => q.FirstOrDefault(f => f.Id == id));
        }

        public long Insert(File newFile)
        {
            Repository.Insert(newFile);
            Repository.Save();
            return newFile.Id;
        }

        public async Task<ServiceResult> AddAdvertiseImagesAsync(FilePostAdvertiseImagesRequest request)
        {
            var serviceResult = new ServiceResult();
            var advertise = Repository.Find<Advertise, long>(request.advertiseId);
            if (advertise == null)
            {
                serviceResult.AddError("advertise Id is incorrect");
                return serviceResult;
            }

            CheckExistDirectory(File.ResidenceImagesDirectory);
            var quality = 80;
            var maxWidth = 1024;
            var newFileIds = new List<long>();
            foreach (var item in request.images.Files)
            {
                var file = new File()
                {
                    PostDate = DateTime.Now,
                    LastModifyDate = DateTime.Now,
                    UserID = request.userId,
                    MinifyStatus = File.MinifyStatusEnum.Done,
                    MinifyQualityPercent = quality,
                    MinifyMaxWidth = maxWidth,
                    Type = File.FileTypeEnum.ResidenceImage
                };
                file.Advertises = new List<Advertise>();
                file.Advertises.Add(advertise);
                Insert(file);

                var fileName = $"advertise_{request.advertiseId}_{file.Id}.jpg";
                ImageCodecInfo format = ImageUtility.GetEncoder(ImageFormat.Jpeg);
                EncoderParameters encoderParameters = new EncoderParameters(1);
                encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, quality);
                var image = Image.FromStream(item.OpenReadStream(), true, true);
                image = ImageUtility.MinifyImage(image, maxWidth);
                image.Save(System.IO.Path.Combine(webHostEnvironment.WebRootPath,
                    File.ResidenceImagesDirectory, fileName), format, encoderParameters);
                image.Dispose();
                UpdateFilePath(file.Id, $"{File.ResidenceImagesDirectory}/{fileName}");
                newFileIds.Add(file.Id);
            }
            await mediator.Send(new GenerateThumbImageCommand(advertise.Id, null, newFileIds));
            return serviceResult;
        }

        public void Update(File editedFile, string wwwrootPath)
        {
            var file = Repository.Find(editedFile.Id);
            if (string.IsNullOrEmpty(editedFile.FilePath) == false && editedFile.FilePath != file.FilePath)
            {
                DeleteFile(GetFullPath(file.CorrectedFilePath));
                file.FilePath = editedFile.FilePath;
            }
            file.LastModifyDate = editedFile.LastModifyDate;
            file.PostDate = editedFile.PostDate;
            file.UserID = editedFile.UserID;
            file.MinifyStatusInt = editedFile.MinifyStatusInt;
            file.MinifyMaxWidth = editedFile.MinifyMaxWidth;
            file.MinifyQualityPercent = editedFile.MinifyQualityPercent;
            Repository.Update(file);
            Repository.Save();
        }

        public void UpdateFilePath(long fileId, string filePath)
        {
            var data = Repository.Find(fileId);
            data.FilePath = filePath;
            data.LastModifyDate = DateTime.Now;
            Repository.Update(data);
            Repository.Save();
        }

        public async Task<ServiceResult<long>> UpdateUserProfileImageAsync(int userId, IFormFile newImage)
        {
            var serviceResult = new ServiceResult<long>();
            if (File.IsValidImageContentType(newImage.ContentType) == false)
            {
                serviceResult.AddError("incorrect image format");
                return serviceResult;
            }

            var user = Repository.Find<User, int>(userId);
            string filepath = $"{File.UserImagesDirectory}/user_{user.Id}.jpg";
            if (user.PhotoID != null)
            {
                serviceResult.Result = user.PhotoID.Value;
                var file = Repository.Find(user.PhotoID.Value);
                file.FilePath = filepath;
                file.LastModifyDate = DateTime.Now;
                Update(file, webHostEnvironment.WebRootPath);
            }
            else
            {
                serviceResult.Result = Insert(new File()
                {
                    PostDate = DateTime.Now,
                    LastModifyDate = DateTime.Now,
                    UserID = user.Id,
                    Type = File.FileTypeEnum.UserImage,
                    FilePath = filepath
                });
            }
            CheckExistDirectory(File.UserImagesDirectory);
            await SaveFile(newImage, filepath);
            ClearImagesCache();
            return serviceResult;
        }

        public async Task<ServiceResult> UpdateAdvertiseLicenseImageAsync(FilePostAdvertiseLicenseImageRequest request)
        {
            var serviceResult = new ServiceResult();
            var advertise = Repository.Find<Advertise, long>(request.advertiseId);
            if (File.IsValidImageContentType(request.image.ContentType) == false)
            {
                serviceResult.AddError("image content type is incorrect");
            }
            if (advertise == null)
            {
                serviceResult.AddError("advertise id is incorrect");
            }
            if (advertise != null && advertise.UserId != request.userId)
            {
                serviceResult.AddError("user is incorrect");
            }
            if (serviceResult.HasError())
            {
                return serviceResult;
            }

            string filepath = $"{File.ResidenceLicenseImagesDirectory}/license_{request.advertiseId}.jpg";
            if (advertise.LicenseFileId != null)
            {
                advertise.LicenseFile.FilePath = filepath;
                advertise.LicenseFile.LastModifyDate = DateTime.Now;
                Repository.Update(advertise.LicenseFile);
                Repository.Save();
            }
            else
            {
                var newLicenseFile = new File()
                {
                    PostDate = DateTime.Now,
                    LastModifyDate = DateTime.Now,
                    UserID = request.userId,
                    FilePath = filepath,
                    Type = File.FileTypeEnum.ResidenceLicense,
                    AdvertiseLicense = advertise
                };
                Insert(newLicenseFile);
            }
            CheckExistDirectory(File.ResidenceLicenseImagesDirectory);
            await SaveFile(request.image, filepath);
            return serviceResult;
        }

        public async Task<ServiceResult> UpdateResidenceVideoAsync(int userId, long residenceId, IFormFile video)
        {
            var serviceResult = new ServiceResult();
            var residence = Repository.Find<Advertise, long>(residenceId);
            if (video == null || video.Length == 0 || File.IsValidVideoContentType(video.ContentType) == false)
            {
                serviceResult.AddError("فایل انتخاب شده اشتباه است");
            }
            if (residence == null)
            {
                serviceResult.AddError("شناسه اقامتگاه اشتباه است");
            }
            if (residence != null && residence.UserId != userId)
            {
                serviceResult.AddError("شما مجوز افزودن ویدیو به این اقامتگاه را ندارید");
            }
            if (serviceResult.HasError())
            {
                return serviceResult;
            }

            string filepath = $"{File.PendingResidenceVideosDirectory}/pendingResidenceVideo_{residenceId}.mp4";
            if (residence.VideoId.HasValue)
            {
                residence.Video.FilePath = filepath;
                residence.Video.LastModifyDate = DateTime.Now;
                Repository.Update(residence.Video);
                Repository.Save();
            }
            else
            {
                var newVideoFile = new File()
                {
                    PostDate = DateTime.Now,
                    LastModifyDate = DateTime.Now,
                    UserID = userId,
                    FilePath = filepath,
                    Type = File.FileTypeEnum.ResidenceVideo,
                    ResidenceVideo = residence
                };
                Insert(newVideoFile);
            }
            CheckExistDirectory(File.PendingResidenceVideosDirectory);
            await SaveFile(video, filepath);
            return serviceResult;
        }

        //public async Task<ServiceResult> MoveResidenceVideoToMainDirectoryAsync(long videoFileId)
        //{
        //    ServiceResult serviceResult = new ServiceResult();
        //    var videoFile = await Repository.FindAsync(videoFileId);
        //    string filepath = $"{File.ResidenceVideosDirectory}/residenceVideo_{videoFile.ResidenceVideo.Id}.mp4";
        //    if (MoveFile(videoFile.FilePath, filepath) == false)
        //    {
        //        serviceResult.AddError("در حال حاضر امکان دسترسی به فایل وجود ندارد. لطفا بعدا امتحان کنید.");
        //        return serviceResult;
        //    }
        //    videoFile.FilePath = filepath;
        //    Repository.Update(videoFile);
        //    Repository.Save();
        //    return serviceResult;
        //}

        public async Task<ServiceResult> ConversionResidenceVideoAsync(long videoFileId)
        {
            ServiceResult serviceResult = new ServiceResult();
            var videoFile = await Repository.FindAsync(videoFileId);

            if (IsLockedFile(videoFile.FilePath))
            {
                serviceResult.AddError("در حال حاضر امکان دسترسی به فایل وجود ندارد. لطفا بعدا امتحان کنید.");
                return serviceResult;
            }

            string newFilePath = $"{File.ResidenceVideosDirectory}/residenceVideo_{videoFile.ResidenceVideo.Id}.mp4";
            var conversionResult = await VideoUtility.ConversionAsync(videoFile.FilePath, newFilePath);
            if (conversionResult.result == false)
            {
                serviceResult.AddError("عملیات پردازش ویدیو با خطا مواجه شد.");
                logger.Error(conversionResult.errorMessage);
                return serviceResult;
            }
            DeleteFile(videoFile.FilePath);
            videoFile.FilePath = newFilePath;
            Repository.Update(videoFile);
            Repository.Save();
            return serviceResult;
        }

        public void Delete(int fileId, string serverPath)
        {
            var file = Repository.Find(fileId);
            DeleteFile(GetFullPath(file.CorrectedFilePath));
            Repository.Delete(fileId);
            Repository.Save();
        }

        public async Task<ServiceResult> DeleteAdvertiseImage(long advertiseId, long fileId, int userId)
        {
            var serviceResult = new ServiceResult();
            var advertise = Repository.Find<Advertise, long>(advertiseId);
            if (advertise == null)
            {
                serviceResult.AddError("advertise Id is incorrect");
            }
            if (advertise.UserId != userId)
            {
                serviceResult.AddError("user is incorrect");
            }
            if (advertise.Photos.Any(x => x.Id == fileId) == false)
            {
                serviceResult.AddError("file Id is incorrect");
            }
            if (serviceResult.HasError())
            {
                return serviceResult;
            }
            var file = Repository.Find(fileId);
            file.Advertises.Remove(advertise);
            Repository.Update(file);
            Repository.Save();
            await mediator.Send(new RemovePhotosByFileIdsCommand(advertiseId, new List<long>() { fileId }));
            return serviceResult;
        }

        public void GenerateThumbImage(long accId, long fileId)
        {
            mediator.Send(new GenerateThumbImageCommand(accId, null, new List<long>() { fileId }, true));
        }

        private void ClearImagesCache()
        {
            System.IO.DirectoryInfo imageCacheDir = new System.IO.DirectoryInfo(GetFullPath(File.ImageCacheDirectory));
            foreach (System.IO.FileInfo item in imageCacheDir.GetFiles())
            {
                item.Delete();
            }
        }

        private async Task SaveFile(IFormFile file, string path)
        {
            DeleteFile(GetFullPath(path));
            using (var stream = System.IO.File.Create(GetFullPath(path)))
            {
                await file.CopyToAsync(stream);
                stream.Close();
            }
        }

        private bool MoveFile(string sourceFullPath, string destinationFullPath)
        {
            if (IsLockedFile(sourceFullPath) || IsLockedFile(destinationFullPath))
            {
                return false;
            }
            System.IO.File.Move(sourceFullPath, destinationFullPath, true);
            return true;
        }

        private void DeleteFile(string fullPath)
        {
            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
            }
        }

        private void CheckExistDirectory(string path)
        {
            path = GetFullPath(path);
            if (System.IO.Directory.Exists(path) == false)
                System.IO.Directory.CreateDirectory(path);
        }

        private string GetFullPath(string path)
        {
            return System.IO.Path.Combine(webHostEnvironment.WebRootPath, path);
        }

        private bool IsLockedFile(string fullPath)
        {
            try
            {
                System.IO.FileInfo fileInfo = new System.IO.FileInfo(fullPath);
                if (fileInfo.Exists)
                {
                    using (System.IO.FileStream stream = fileInfo.Open(System.IO.FileMode.Open,
                    System.IO.FileAccess.Read, System.IO.FileShare.None))
                    {
                        stream.Close();
                    }
                }
                return false;
            }
            catch
            {
                return true;
            }
        }
    }
}
