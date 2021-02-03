using MediatR;
using System.Threading;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using Amlakbashi.Core.Common.Utilities;
using Amlakbashi.Core.Common.Repository;
using Amlakbashi.Core.Entities;
using Hangfire;
using System.Collections.Generic;
using Amlakbashi.Mediator.Commands.FileCommands;
using Amlakbashi.Mediator.Commands.AdvertiseCommands;
using static Amlakbashi.Core.Entities.Advertise;
using System.Linq;
using Amlakbashi.Core.DTOs.FileDTOs;

namespace Amlakbashi.Application.Services.FileServices.CommandHandlers
{
    public class FileCommandHandler : IRequestHandler<MinifyImageCommand>,
        IRequestHandler<StopQueuedJobCommand>,
        IRequestHandler<GenerateThumbImageCommand, bool>
    {
        private static readonly object objlock = new object();
        private readonly IRepository<File, long> fileRepository;
        public FileCommandHandler(
            IRepository<File, long> fileRepository)
        {
            this.fileRepository = fileRepository;
        }

        public Task<Unit> Handle(MinifyImageCommand request, CancellationToken cancellationToken)
        {
            Image minifiedImage = null;
            try
            {
                using (var img = Image.FromFile(request.ImagePath))
                {
                    minifiedImage = ImageUtility.MinifyImage(img, 1024);
                    img.Dispose();
                }
            }
            catch
            {
                var excFile = fileRepository.Find(request.FileId);
                excFile.MinifyStatus = File.MinifyStatusEnum.Failed;
                fileRepository.Update(excFile);
                fileRepository.Save();
                return Task.FromResult(Unit.Value);
            }
            var extension = System.IO.Path.GetExtension(request.ImagePath);
            ImageCodecInfo format;
            EncoderParameters encoderParameters;
            switch (extension)
            {
                case ".png":
                    format = ImageUtility.GetEncoder(ImageFormat.Png);
                    encoderParameters = new EncoderParameters(1);
                    encoderParameters.Param[0] = new EncoderParameter(Encoder.Compression, (long)EncoderValue.CompressionLZW);
                    break;
                case ".gif":
                    format = ImageUtility.GetEncoder(ImageFormat.Gif);
                    encoderParameters = new EncoderParameters(0);
                    break;
                default:
                    format = ImageUtility.GetEncoder(ImageFormat.Jpeg);
                    encoderParameters = new EncoderParameters(1);
                    encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, request.QualityPercent);
                    break;
            }

            string file = System.IO.Path.GetFileNameWithoutExtension(request.ImagePath);
            string NewPath = request.ImagePath.Replace(file, file + "-min");

            lock (objlock)
            {
                try
                {
                    minifiedImage.Save(NewPath, format, encoderParameters);
                    minifiedImage.Dispose();
                }
                catch
                {
                    var excFile = fileRepository.Find(request.FileId);
                    excFile.MinifyStatus = File.MinifyStatusEnum.Failed;
                    fileRepository.Update(excFile);
                    fileRepository.Save();
                    if (System.IO.File.Exists(NewPath))
                        System.IO.File.Delete(NewPath);
                    return Task.FromResult(Unit.Value);
                }
                try
                {
                    System.IO.File.Delete(request.ImagePath);
                    System.IO.File.Move(NewPath, request.ImagePath);

                    var excFile = fileRepository.Find(request.FileId);
                    excFile.MinifyMaxWidth = request.MaxWidth;
                    excFile.MinifyQualityPercent = request.QualityPercent;
                    excFile.MinifyStatus = File.MinifyStatusEnum.Done;
                    fileRepository.Update(excFile);
                    fileRepository.Save();
                }
                catch
                {
                    var excFile = fileRepository.Find(request.FileId);
                    excFile.MinifyStatus = File.MinifyStatusEnum.Failed;
                    fileRepository.Update(excFile);
                    fileRepository.Save();
                    System.IO.File.Delete(NewPath);
                }
            }
            return Task.FromResult(Unit.Value);
        }

        public Task<Unit> Handle(StopQueuedJobCommand request, CancellationToken cancellationToken)
        {
            var monitoringApi = JobStorage.Current.GetMonitoringApi();
            var queues = monitoringApi.Queues();
            var toDelete = new List<string>();
            foreach (var queue in queues)
            {
                for (var i = 0; i < queue.Length; i++)
                {
                    foreach (var job in monitoringApi.EnqueuedJobs(queue.Name, 0, 100000000))
                    {
                        toDelete.Add(job.Key);
                    }
                }
            }
            foreach (var jobId in toDelete)
            {
                BackgroundJob.Delete(jobId);
            }
            return Task.FromResult(Unit.Value);
        }

        public Task<bool> Handle(GenerateThumbImageCommand request, CancellationToken cancellationToken)
        {
            var accThumbPath = request.Path + "content/accthumb/" + request.AdvertiseId;
            if (System.IO.Directory.Exists(accThumbPath))
            {
                lock (objlock)
                {
                    System.IO.Directory.Delete(accThumbPath, true);
                }
            }

            var files = new List<File>();
            if (request.PhotoAlbumIds.Count > 0)
            {
                files.AddRange(fileRepository.Query(q => q.Where(w => request.PhotoAlbumIds.Contains(w.Id))));
            }
            if (request.MainPhotoId != null && !request.PhotoAlbumIds.Contains((long)request.MainPhotoId))
            {
                files.Add(fileRepository.Find((long)request.MainPhotoId));
            }

            var thumbs = new List<ImageThumbDTO>();
            foreach (var file in files)
            {
                try
                {
                    var fileThumbPath = accThumbPath + "/" + file.Id;
                    var origFilePath = request.Path + file.FilePath.Replace("~", "").Substring(1);
                    thumbs.Add(new ImageThumbDTO()
                    {
                        directoryPath = fileThumbPath,
                        OrigPath = origFilePath,
                        thumbPath = accThumbPath + "/" + file.Id + "/appcard.jpg",
                        w = 160,
                        h = 114
                    });
                    thumbs.Add(new ImageThumbDTO()
                    {
                        directoryPath = fileThumbPath,
                        OrigPath = origFilePath,
                        thumbPath = accThumbPath + "/" + file.Id + "/appcarousel.jpg",
                        w = 450,
                        h = 300
                    });
                    thumbs.Add(new ImageThumbDTO()
                    {
                        directoryPath = fileThumbPath,
                        OrigPath = origFilePath,
                        thumbPath = accThumbPath + "/" + file.Id + "/card.jpg",
                        w = 240,
                        h = 144
                    });
                    thumbs.Add(new ImageThumbDTO()
                    {
                        directoryPath = fileThumbPath,
                        OrigPath = origFilePath,
                        thumbPath = accThumbPath + "/" + file.Id + "/xxxlarge.jpg",
                        w = 700,
                        h = 394
                    });
                    thumbs.Add(new ImageThumbDTO()
                    {
                        directoryPath = fileThumbPath,
                        OrigPath = origFilePath,
                        thumbPath = accThumbPath + "/" + file.Id + "/xxlarge.jpg",
                        w = 600,
                        h = 338
                    });
                    thumbs.Add(new ImageThumbDTO()
                    {
                        directoryPath = fileThumbPath,
                        OrigPath = origFilePath,
                        thumbPath = accThumbPath + "/" + file.Id + "/xlarge.jpg",
                        w = 400,
                        h = 253
                    });
                    thumbs.Add(new ImageThumbDTO()
                    {
                        directoryPath = fileThumbPath,
                        OrigPath = origFilePath,
                        thumbPath = accThumbPath + "/" + file.Id + "/large.jpg",
                        w = 331,
                        h = 186
                    });
                    thumbs.Add(new ImageThumbDTO()
                    {
                        directoryPath = fileThumbPath,
                        OrigPath = origFilePath,
                        thumbPath = accThumbPath + "/" + file.Id + "/medium.jpg",
                        w = 249,
                        h = 140
                    });
                    thumbs.Add(new ImageThumbDTO()
                    {
                        directoryPath = fileThumbPath,
                        OrigPath = origFilePath,
                        thumbPath = accThumbPath + "/" + file.Id + "/small.jpg",
                        w = 213,
                        h = 120
                    });
                    thumbs.Add(new ImageThumbDTO()
                    {
                        directoryPath = fileThumbPath,
                        OrigPath = origFilePath,
                        thumbPath = accThumbPath + "/" + file.Id + "/xsmall.jpg",
                        w = 146,
                        h = 82
                    });

                }
                catch { }
            }

            foreach (var thumb in thumbs)
            {
                if (System.IO.File.Exists(thumb.OrigPath))
                {
                    new System.IO.FileInfo(thumb.thumbPath).Directory.Create();
                    var origImage = Image.FromFile(thumb.OrigPath);
                    var thumbImage = ImageUtility.ResizeImageKeepAspectRatio(origImage, thumb.w, thumb.h);
                    ImageUtility.SaveThumb(thumbImage, thumb.thumbPath, thumb.OrigPath);
                }
            }

            return Task.FromResult(true);
        }
    }
}
