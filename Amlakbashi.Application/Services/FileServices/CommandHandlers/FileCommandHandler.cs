using MediatR;
using System.Threading;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using Amlakbashi.Core.Common.Utilities;
using Amlakbashi.Core.Common.Repository;
using Hangfire;
using System.Collections.Generic;
using Amlakbashi.Mediator.Commands.FileCommands;
using Amlakbashi.Mediator.Commands.AdvertiseCommands;
using Entities = Amlakbashi.Core.Entities;
using System.Linq;
using Amlakbashi.Core.DTOs.FileDTOs;
using System.IO;
using System;
using log4net;
using Microsoft.AspNetCore.Hosting;

namespace Amlakbashi.Application.Services.FileServices.CommandHandlers
{
    public class FileCommandHandler : IRequestHandler<MinifyImageCommand>,
        IRequestHandler<StopQueuedJobCommand>,
        IRequestHandler<GenerateThumbImageCommand, bool>,
        IRequestHandler<SetWatermarkCommand, string>,
        IRequestHandler<RemovePhotosByFileIdsCommand>,
        IRequestHandler<RemovePhotosByPathsCommnd>
    {
        private static readonly object objlock = new object();
        private readonly IRepository<Entities.File, long> fileRepository;
        private readonly ILog logger;
        private readonly IWebHostEnvironment host;
        private readonly IMediator mediator;
        public FileCommandHandler(ILog logger,
            IRepository<Entities.File, long> fileRepository,
            IWebHostEnvironment host,
            IMediator mediator)
        {
            this.fileRepository = fileRepository;
            this.logger = logger;
            this.host = host;
            this.mediator = mediator;
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
                excFile.MinifyStatus = Entities.File.MinifyStatusEnum.Failed;
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
                    excFile.MinifyStatus = Entities.File.MinifyStatusEnum.Failed;
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
                    excFile.MinifyStatus = Entities.File.MinifyStatusEnum.Done;
                    fileRepository.Update(excFile);
                    fileRepository.Save();
                }
                catch
                {
                    var excFile = fileRepository.Find(request.FileId);
                    excFile.MinifyStatus = Entities.File.MinifyStatusEnum.Failed;
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
            try
            {
                var accThumbPath = request.Path + "/content/accthumb/" + request.AdvertiseId;
                var watermarkFolderPath = request.Path + "/content/imgcache/";
                if (System.IO.Directory.Exists(accThumbPath))
                {
                    for (int i = 1; i <= 3; ++i)
                    {
                        try
                        {
                            Directory.Delete(accThumbPath, true);
                            break;
                        }
                        catch (IOException exc) when (i <= 3)
                        {
                            logger.Error("FileCommandHandler.GenerateThumbImageCommand(delete old thumb directory)", exc);
                            Thread.Sleep(1000);
                        }
                    }
                }

                var files = new List<Entities.File>();
                if (request.PhotoAlbumIds.Count > 0)
                {
                    files.AddRange(fileRepository.Query(q => q.Where(w => request.PhotoAlbumIds.Contains(w.Id))));
                }
                if (request.MainPhotoId != null && !request.PhotoAlbumIds.Contains((long)request.MainPhotoId))
                {
                    files.Add(fileRepository.Find((long)request.MainPhotoId));
                }

                var watermarkedImageList = new List<string>();
                var thumbs = new List<ImageThumbDTO>();
                foreach (var file in files)
                {
                    if (File.Exists(request.Path + file.FilePath.Replace("~", "")) == false)
                    {
                        continue;
                    }
                    var waterPath = mediator.Send(new SetWatermarkCommand(file.Id, host.WebRootPath)).Result;
                    //var origFilePath = request.Path + "/" + file.FilePathWithoutTildeAndSlash;
                    var watermarkedImagePath = request.Path + waterPath;
                    watermarkedImageList.Add(watermarkedImagePath);
                    var fileThumbPath = accThumbPath + "/" + file.Id;

                    thumbs.Add(new ImageThumbDTO()
                    {
                        directoryPath = fileThumbPath,
                        OrigPath = watermarkedImagePath,
                        thumbPath = fileThumbPath + "/appcard.jpg",
                        w = 160,
                        h = 114
                    });
                    thumbs.Add(new ImageThumbDTO()
                    {
                        directoryPath = fileThumbPath,
                        OrigPath = watermarkedImagePath,
                        thumbPath = fileThumbPath + "/appcarousel.jpg",
                        w = 450,
                        h = 300
                    });
                    thumbs.Add(new ImageThumbDTO()
                    {
                        directoryPath = fileThumbPath,
                        OrigPath = watermarkedImagePath,
                        thumbPath = fileThumbPath + "/card.jpg",
                        w = 240,
                        h = 144
                    });
                    thumbs.Add(new ImageThumbDTO()
                    {
                        directoryPath = fileThumbPath,
                        OrigPath = watermarkedImagePath,
                        thumbPath = fileThumbPath + "/xxxlarge.jpg",
                        w = 700,
                        h = 394
                    });
                    thumbs.Add(new ImageThumbDTO()
                    {
                        directoryPath = fileThumbPath,
                        OrigPath = watermarkedImagePath,
                        thumbPath = fileThumbPath + "/xxlarge.jpg",
                        w = 600,
                        h = 338
                    });
                    thumbs.Add(new ImageThumbDTO()
                    {
                        directoryPath = fileThumbPath,
                        OrigPath = watermarkedImagePath,
                        thumbPath = fileThumbPath + "/xlarge.jpg",
                        w = 400,
                        h = 253
                    });
                    thumbs.Add(new ImageThumbDTO()
                    {
                        directoryPath = fileThumbPath,
                        OrigPath = watermarkedImagePath,
                        thumbPath = fileThumbPath + "/large.jpg",
                        w = 331,
                        h = 186
                    });
                    thumbs.Add(new ImageThumbDTO()
                    {
                        directoryPath = fileThumbPath,
                        OrigPath = watermarkedImagePath,
                        thumbPath = fileThumbPath + "/medium.jpg",
                        w = 249,
                        h = 140
                    });
                    thumbs.Add(new ImageThumbDTO()
                    {
                        directoryPath = fileThumbPath,
                        OrigPath = watermarkedImagePath,
                        thumbPath = fileThumbPath + "/small.jpg",
                        w = 213,
                        h = 120
                    });
                    thumbs.Add(new ImageThumbDTO()
                    {
                        directoryPath = fileThumbPath,
                        OrigPath = watermarkedImagePath,
                        thumbPath = fileThumbPath + "/xsmall.jpg",
                        w = 146,
                        h = 82
                    });
                }

                try
                {
                    foreach (var thumb in thumbs)
                    {
                        if (File.Exists(thumb.OrigPath))
                        {
                            if (Directory.Exists(thumb.directoryPath) == false)
                            {
                                Directory.CreateDirectory(thumb.directoryPath);
                            }
                            using (FileStream stream = new FileStream(thumb.OrigPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                            using (Image origImage = Image.FromStream(stream))
                            {
                                var thumbImage = ImageUtility.ResizeImageKeepAspectRatio(origImage, thumb.w, thumb.h);
                                //ImageUtility.SaveThumb(thumbImage, thumb.thumbPath, thumb.OrigPath);

                                var format = ImageUtility.GetEncoder(ImageFormat.Jpeg);
                                var encoderParameters = new EncoderParameters(1);
                                encoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 80L);
                                thumbImage.Save(thumb.thumbPath, format, encoderParameters);

                                thumbImage.Dispose();
                            }
                        }
                    }
                }
                catch (Exception exc)
                {
                    logger.Error("FileCommandHandler.GenerateThumbImageCommand(generate thumbs)", exc);
                }

                try
                {
                    foreach (var item in watermarkedImageList)
                    {
                        if (File.Exists(item))
                        {
                            File.Delete(item);
                        }
                    }
                }
                catch (Exception exc)
                {
                    logger.Error("FileCommandHandler.GenerateThumbImageCommand(delete watermarked images)", exc);
                }
            }
            catch (Exception exc)
            {
                logger.Error("FileCommandHandler.GenerateThumbImageCommand", exc);
            }
            return Task.FromResult(true);
        }

        public Task<string> Handle(SetWatermarkCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var file = fileRepository.Find(request.FileId);
                var filePath = request.ServerPath + file.FilePath.Replace("~", "");
                string waterPath = string.Empty;

                if (file == null || File.Exists(filePath) == false)
                    return Task.FromResult(waterPath);

                lock (objlock)
                {
                    double ratio = 4.5;
                    using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                    using (Image watermarkImage = Image.FromFile(request.ServerPath + "/resource/img/water_logo.png"))
                    using (Image image = Image.FromStream(stream))
                    {
                        int waterWidth = Convert.ToInt16((double)image.Width / ratio);
                        double waterRate = (double)watermarkImage.Width / (double)waterWidth;
                        int waterHeight = Convert.ToInt16((double)watermarkImage.Height / waterRate);
                        string logoPath = "";

                        using (Bitmap thumbnailBitmap = new Bitmap(waterWidth, waterHeight))
                        {
                            thumbnailBitmap.SetResolution(watermarkImage.HorizontalResolution, watermarkImage.VerticalResolution);
                            using (Graphics new_watermark = Graphics.FromImage(thumbnailBitmap))
                            {
                                new_watermark.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                                new_watermark.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                                new_watermark.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                                Rectangle imageRectangle = new Rectangle(0, 0, waterWidth, waterHeight);
                                new_watermark.DrawImage(watermarkImage, imageRectangle);
                                logoPath = "/content/logocache/" + string.Format("logo_{0}.png", file.Id);
                                ImageCodecInfo format = ImageUtility.GetEncoder(ImageFormat.Png);
                                EncoderParameters encoderParameters = new EncoderParameters(1);
                                encoderParameters.Param[0] = new EncoderParameter(Encoder.Compression, (long)EncoderValue.CompressionLZW);
                                thumbnailBitmap.Save(request.ServerPath + logoPath, format, encoderParameters);
                            }
                        }

                        using (Image NewWatermarkImage = Image.FromFile(request.ServerPath + logoPath))
                        using (Graphics imageGraphics = Graphics.FromImage(image))
                        using (TextureBrush watermarkBrush = new TextureBrush(NewWatermarkImage))
                        {
                            int x = image.Width - Convert.ToInt16((double)waterWidth + ((double)waterWidth / 10));
                            int y = image.Height - Convert.ToInt16((double)waterHeight + ((double)waterHeight / 10));
                            watermarkBrush.TranslateTransform(x, y);
                            imageGraphics.FillRectangle(watermarkBrush, new Rectangle(new Point(x, y), new Size(waterWidth + 1, waterHeight)));
                            waterPath = "/content/advertisecache/" + file.FilePath.Substring(file.FilePath.LastIndexOf('/') + 1);
                            var extension = System.IO.Path.GetExtension(request.ServerPath + waterPath);
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
                                    encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, 80L);
                                    break;
                            }
                            image.Save(request.ServerPath + waterPath, format, encoderParameters);
                        }
                        if (File.Exists(request.ServerPath + logoPath))
                        {
                            File.Delete(request.ServerPath + logoPath);
                        }
                    }
                }
                return Task.FromResult(waterPath);
            }
            catch (Exception exc)
            {
                logger.Error("FileCommandHandler.SetWatermarkCommand", exc);
                return Task.FromResult(string.Empty);
            }
        }

        public Task<Unit> Handle(RemovePhotosByFileIdsCommand request, CancellationToken cancellationToken)
        {
            try
            {
                bool hasChange = false;
                var files = fileRepository.Query(q => q.Where(w => request.PhotoIds.Contains(w.Id)));
                foreach (var item in files)
                {
                    if (item.Advertises.Count == 0)
                    {
                        fileRepository.Delete(item.Id);
                        if (File.Exists(Path.Combine(host.WebRootPath, item.FilePathWithoutTildeAndSlash)))
                        {
                            File.Delete(Path.Combine(host.WebRootPath, item.FilePathWithoutTildeAndSlash));
                        }
                        hasChange = true;
                    }
                }
                if (hasChange)
                {
                    fileRepository.Save();
                }
                return Task.FromResult(Unit.Value);
            }
            catch (Exception exc)
            {
                logger.Error("FileCommandHandler.RemovePhotosByFileIdsCommand", exc);
                return Task.FromResult(Unit.Value);
            }
        }

        public Task<Unit> Handle(RemovePhotosByPathsCommnd request, CancellationToken cancellationToken)
        {
            try
            {
                foreach (var item in request.PathList)
                {
                    if (File.Exists(Path.Combine(host.WebRootPath, item)))
                    {
                        File.Delete(Path.Combine(host.WebRootPath, item));
                    }
                }
                return Task.FromResult(Unit.Value);
            }
            catch (Exception exc)
            {
                logger.Error("FileCommandHandler.RemovePhotosByPathsCommnd", exc);
                return Task.FromResult(Unit.Value);
            }
        }
    }
}
