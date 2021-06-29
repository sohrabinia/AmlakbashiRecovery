using Amlakbashi.Core.Common.AppService;
using Amlakbashi.Core.Common.Repository;
using Amlakbashi.Application.Services.FileServices.Interfaces;
using Amlakbashi.Core.Common.Caching;
using Amlakbashi.Core.Entities;
using System.Collections.Generic;
using System.Linq;
using ServiceStack;
using MediatR;
using Amlakbashi.Core.Common.Extensions;
using Amlakbashi.Mediator.Commands.FileCommands;
using Amlakbashi.Mediator.Commands.AdvertiseCommands;

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

        public void Update(File editedFile, string wwwrootPath)
        {
            var data = Repository.Query(q => q.FirstOrDefault(f => f.Id == editedFile.Id));
            if (!editedFile.FilePath.IsNullOrEmpty() && editedFile.FilePath != data.FilePath)
            {
                if (System.IO.File.Exists(wwwrootPath + data.FilePath.Replace("~/", "")))
                    lock (objlock)
                    {
                        System.IO.File.Delete(wwwrootPath + data.FilePath.Replace("~/", ""));
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

        //public void SetWatermarkForAdvertisePhotos(long advertiseId, string serverPath)
        //{
        //    var advertise = Repository.Find<Advertise, long>(advertiseId);
        //    var advertisesToWatermark = new List<Advertise>();
        //    advertisesToWatermark.Add(advertise);
        //    if ((advertise.TypeID == Advertise.AdvertiseType.Complex ||
        //        advertise.TypeID == Advertise.AdvertiseType.HotelApartment) &&
        //        advertise.Childs != null)
        //    {
        //        advertisesToWatermark.AddRange(advertise.Childs);
        //    }
        //    foreach (var adv in advertisesToWatermark)
        //    {
        //        if (adv.Photos.Any())
        //        {
        //            foreach (var photo in adv.Photos)
        //            {
        //                if (!photo.FilePath.Contains("watermark_advertise"))
        //                {
        //                    mediator.Send(new SetWatermarkCommand(photo.Id, serverPath));
        //                }
        //            }
        //            mediator.Send(new GenerateThumbImageCommand(adv.Id, adv.PhotoID,
        //            adv.Photos.Select(s => s.Id).ToList(), serverPath));
        //        }
        //    }
        //    lock (objlock)
        //    {
        //        System.IO.DirectoryInfo IOdirectory = new System.IO.DirectoryInfo(System.IO.Path.Combine(serverPath, "content/imgcache"));
        //        foreach (System.IO.FileInfo IOfile in IOdirectory.GetFiles())
        //        {
        //            IOfile.Delete();
        //        }
        //    }
        //}

        public void GenerateThumbImage(long accId, string rootPath)
        {
            var acc = Repository.Find<Advertise, long>(accId);
            mediator.Send(new GenerateThumbImageCommand(acc.Id, acc.PhotoID,
                    acc.Photos.Select(s => s.Id).ToList(), rootPath));
        }
    }
}
