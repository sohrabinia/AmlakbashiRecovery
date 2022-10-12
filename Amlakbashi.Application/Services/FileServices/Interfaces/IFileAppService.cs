using Amlakbashi.Application.DTOs;
using Amlakbashi.Core.Common.AppService;
using Amlakbashi.Core.DTOs.WebService.Requests.Files;
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
        Task<ServiceResult> AddAdvertiseImagesAsync(FilePostAdvertiseImagesRequest request);
        Task<ServiceResult> MoveResidenceVideoToMainDirectoryAsync(long videoFileId);
        void Update(File editedFile, string wwwrootPath);
        void UpdateFilePath(long fileId, string filePath);
        Task<ServiceResult<long>> UpdateUserProfileImageAsync(int userId, IFormFile newImage);
        Task<ServiceResult> UpdateAdvertiseLicenseImageAsync(FilePostAdvertiseLicenseImageRequest request);
        Task<ServiceResult> AddResidenceVideoAsync(int userId, long residenceId, IFormFile video);
        void Delete(int fileId, string serverPath);
        Task<ServiceResult> DeleteAdvertiseImage(long advertiseId, long fileId, int userId);
        void GenerateThumbImage(long accId, long fileId);
    }
}
