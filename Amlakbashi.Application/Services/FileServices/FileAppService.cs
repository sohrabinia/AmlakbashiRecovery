using Amlakbashi.Core.Common.AppService;
using Amlakbashi.Core.Common.Repository;
using Amlakbashi.Application.Services.FileServices.Interfaces;
using Amlakbashi.Core.Common.Caching;
using Amlakbashi.Core.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ServiceStack;
using MediatR;
using Amlakbashi.Core.Common.Extensions;
using Amlakbashi.Mediator.Commands.FileCommands;

namespace Amlakbashi.Application.Services.FileServices
{
    internal class FileAppService : AppServiceBase<File, long>, IFileAppService
    {
        private static readonly object objlock = new object();
        private readonly IMediator mediator;
        public FileAppService(IRepository<File, long> repository, ICacheManager<File> cache, IMediator mediator) : base(repository, cache)
        {
            this.mediator = mediator;
        }

        public IList<File> GetAll()
        {
            return Repository.Query(q => q).ToList();
        }

        public IQueryable<File> GetAllAsQueryable()
        {
            return Repository.Query(q => q);
        }

        public IList<File> GetAllDescendingByLastModifyDate(int count = 0)
        {
            if (count == 0)
            {
                return Repository.Query(q => q.OrderByDescending(o => o.LastModifyDate).ToList());
            }
            return Repository.Query(q => q.OrderByDescending(o => o.LastModifyDate).Take(count).ToList());
        }

        public IList<File> GetAllAdvertiseFile()
        {
            return Repository.Query(q => q.Where(w => w.FilePath.Contains("/content/advertise/") && w.MinifyStatusInt < 1).ToList());
        }

        public List<string> GetAllFilePath()
        {
            return Repository.Query(q => q.Select(s => s.FilePath).ToList());
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

        public void Update(File editedFile)
        {
            var data = Repository.Query(q => q.FirstOrDefault(f => f.Id == editedFile.Id));
            if (!editedFile.FilePath.IsNullOrEmpty() && editedFile.FilePath != data.FilePath)
            {
                if (System.IO.File.Exists(HttpContext.Current.Server.MapPath(data.FilePath)))
                    lock (objlock)
                    {
                        System.IO.File.Delete(HttpContext.Current.Server.MapPath(data.FilePath));
                    }
                data.FilePath = editedFile.FilePath;
            }
            data.LastModifyDate = editedFile.LastModifyDate;
            data.PostDate = editedFile.PostDate;
            data.UserID = editedFile.UserID;
            data.MinifyStatusInt = editedFile.MinifyStatusInt;
            data.MinifyMaxWidth = editedFile.MinifyMaxWidth;
            data.MinifyQualityPercent = editedFile.MinifyQualityPercent;
            Repository.Update(data);
            Repository.Save();
        }

        public void UpdateFilePath(long id, string newFilePath)
        {
            var data = Repository.Query(q => q.FirstOrDefault(f => f.Id == id));
            data.FilePath = newFilePath;
            Repository.Update(data);
            Repository.Save();
        }

        public void Delete(int fileId)
        {
            var file = Repository.Query(q => q.FirstOrDefault(f => f.Id == fileId));
            if (System.IO.File.Exists(HttpContext.Current.Server.MapPath(file.FilePath)))
                lock (objlock)
                {
                    System.IO.File.Delete(HttpContext.Current.Server.MapPath(file.FilePath));
                }
            Repository.Delete(fileId);
            Repository.Save();
        }

        public void DeleteExtraFiles(List<long> existFile)
        {
            var files = Repository.Query(q => q);
            foreach (var file in files)
            {
                if (!existFile.Contains(file.Id))
                {
                    Repository.Delete(file.Id);
                }
            }
            Repository.Save();
        }

        public void MinifyImage(long fileId, string filePath)
        {
            mediator.Enqueue(new MinifyImageCommand(filePath, 1024, 80, fileId));
        }

        public void StopQueuedJob()
        {
            mediator.Send(new StopQueuedJobCommand());
        }
    }
}
