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

namespace Amlakbashi.Application.Services.FileServices
{
    internal class FileAppService : AppServiceBase<File, long>, IFileAppService
    {
        private readonly IMediator mediator;
        private readonly IWebHostEnvironment webHostEnvironment;
        public FileAppService(IRepository<File, long> repository,
            IMediator mediator,
            IWebHostEnvironment webHostEnvironment) : base(repository)
        {
            this.mediator = mediator;
            this.webHostEnvironment = webHostEnvironment;
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

            CheckExistDirectory(File.AdvertiseImageDirectory);
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
                    MinifyMaxWidth = maxWidth
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
                    File.AdvertiseImageDirectory, fileName), format, encoderParameters);
                UpdateFilePath(file.Id, $"{File.AdvertiseImageDirectory}/{fileName}");
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
                DeleteFile(file.CorrectedFilePath);
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
            if (advertise != null && advertise.UserID != request.userId)
            {
                serviceResult.AddError("user is incorrect");
            }
            if (serviceResult.HasError())
            {
                return serviceResult;
            }

            string filepath = $"{File.AdvertiseLicenseImagesDirectory}/license_{request.advertiseId}.jpg";
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
                    AdvertiseLicense = advertise
                };
                Insert(newLicenseFile);
            }
            CheckExistDirectory(File.AdvertiseLicenseImagesDirectory);
            await SaveFile(request.image, filepath);
            return serviceResult;
        }

        public void Delete(int fileId, string serverPath)
        {
            var file = Repository.Query(q => q.FirstOrDefault(f => f.Id == fileId));
            DeleteFile(file.CorrectedFilePath);
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
            if (advertise.UserID != userId)
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
            System.IO.DirectoryInfo imageCacheDir = new System.IO.DirectoryInfo(
                System.IO.Path.Combine(webHostEnvironment.WebRootPath, File.ImageChacheDerectory));
            foreach (System.IO.FileInfo item in imageCacheDir.GetFiles())
            {
                item.Delete();
            }
        }

        private async Task SaveFile(IFormFile file, string path)
        {
            path = System.IO.Path.Combine(webHostEnvironment.WebRootPath, path);
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
            using (var stream = System.IO.File.Create(path))
            {
                await file.CopyToAsync(stream);
            }
        }

        private void DeleteFile(string path)
        {
            path = System.IO.Path.Combine(webHostEnvironment.WebRootPath, path);
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
        }

        private void CheckExistDirectory(string path)
        {
            path = System.IO.Path.Combine(webHostEnvironment.WebRootPath, path);
            if (System.IO.Directory.Exists(path) == false)
                System.IO.Directory.CreateDirectory(path);
        }
    }
}
