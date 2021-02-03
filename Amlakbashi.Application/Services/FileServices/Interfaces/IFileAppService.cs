using Amlakbashi.Core.Common.AppService;
using Amlakbashi.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amlakbashi.Application.Services.FileServices.Interfaces
{
    public interface IFileAppService : IAppService<File, long>
    {
        IList<File> GetAll();
        IQueryable<File> GetAllAsQueryable();
        IList<File> GetAllDescendingByLastModifyDate(int count = 0);
        IList<File> GetAllAdvertiseFile();
        List<string> GetAllFilePath();
        File Find(long id);
        long Insert(File newFile);
        void Update(File editedFile);
        void UpdateFilePath(long id, string newFilePath);
        void Delete(int fileId);
        void DeleteExtraFiles(List<long> existFile);
        void MinifyImage(long fileId, string filePath);
        void StopQueuedJob();
    }
}
