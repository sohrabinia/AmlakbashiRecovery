using Amlakbashi.Core.Common.AppService;
using Amlakbashi.Core.Common.Repository;
using Amlakbashi.Application.Services.FileServices.Interfaces;
using Amlakbashi.Core.Entities;
using System.Collections.Generic;
using System.Linq;
using ServiceStack;
using MediatR;
using Amlakbashi.Mediator.Commands.AdvertiseCommands;
using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.Threading.Tasks;

namespace Amlakbashi.Application.Services.FileServices
{
    internal class FileAppService : AppServiceBase<File, long>, IFileAppService
    {
        private readonly IMediator mediator;
        private readonly IWebHostEnvironment webHostEnvironment;
        private static readonly object objlock = new object();
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

        public void Update(File editedFile, string wwwrootPath)
        {
            var file = Repository.Find(editedFile.Id);
            if (editedFile.FilePath.IsNullOrEmpty() == false && editedFile.FilePath != file.FilePath)
            {
                if (System.IO.File.Exists(wwwrootPath + file.FilePath.Replace("~/", "")))
                {
                    lock (objlock)
                    {
                        System.IO.File.Delete(wwwrootPath + file.FilePath.Replace("~/", ""));
                    }
                }
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

        public async Task<long> UpdateUserProfileImageAsync(int userId, IFormFile newImage)
        {
            if (HasImageCorrectContentType(newImage.ContentType) == false)
            {
                return 0;
            }

            var user = Repository.Find<User, int>(userId);
            string filepath = $"~/content/users/user_{user.Id}.jpg";
            long PhotoID = 0;
            if (user.PhotoID != null)
            {
                PhotoID = user.Photo.Id;
                var file = Repository.Find(user.PhotoID.Value);
                file.FilePath = filepath;
                file.LastModifyDate = DateTime.Now;
                Update(file, webHostEnvironment.WebRootPath);
            }
            else
            {
                PhotoID = Insert(new File()
                {
                    PostDate = DateTime.Now,
                    LastModifyDate = DateTime.Now,
                    UserID = user.Id,
                    FilePath = filepath
                });
            }
            using (var stream = System.IO.File.Create(webHostEnvironment.WebRootPath + filepath.Replace("~", "")))
            {
                await newImage.CopyToAsync(stream);
            }
            ClearImagesCache();
            return PhotoID;
        }

        public void Delete(int fileId, string serverPath)
        {
            var file = Repository.Query(q => q.FirstOrDefault(f => f.Id == fileId));
            if (System.IO.File.Exists(serverPath + file.FilePath.Replace("~/", "")))
                lock (objlock)
                {
                    System.IO.File.Delete(serverPath + file.FilePath.Replace("~/", ""));
                }
            Repository.Delete(fileId);
            Repository.Save();
        }

        public void ClearImagesCache()
        {
            System.IO.DirectoryInfo imageCacheDir = new System.IO.DirectoryInfo(
                System.IO.Path.Combine(webHostEnvironment.WebRootPath, "content/imgcache"));
            foreach (System.IO.FileInfo item in imageCacheDir.GetFiles())
            {
                item.Delete();
            }
        }

        public bool HasImageCorrectContentType(string contentType)
        {
            contentType = contentType.ToLower();
            return contentType == "image/png" ||
                contentType == "image/gif" ||
                contentType == "image/jpg" ||
                contentType == "image/jpeg" 
                ? true : false;
        }

        public void GenerateThumbImage(long accId, long fileId)
        {
            mediator.Send(new GenerateThumbImageCommand(accId, null, new List<long>() { fileId }, true));
        }
    }
}
