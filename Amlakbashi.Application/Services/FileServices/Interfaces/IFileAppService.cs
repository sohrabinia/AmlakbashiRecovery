using Amlakbashi.Core.Common.AppService;
using Amlakbashi.Core.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Application.Services.FileServices.Interfaces
{
    public interface IFileAppService : IAppService<File, long>
    {
        IList<File> GetAllDescendingByLastModifyDate(int count = 0);
        File Find(long id);
        long Insert(File newFile);
        void Update(File editedFile, string wwwrootPath);
        void UpdateFilePath(long fileId, string filePath);
        Task<long> UpdateUserProfileImageAsync(int userId, IFormFile newImage);
        void Delete(int fileId, string serverPath);
        void ClearImagesCache();
        void GenerateThumbImage(long accId, long fileId);
    }
}
