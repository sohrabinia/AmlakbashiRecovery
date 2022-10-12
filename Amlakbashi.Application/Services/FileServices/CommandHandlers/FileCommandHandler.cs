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
        IRequestHandler<RemovePhotosByPathsCommnd>,
        IRequestHandler<RenameAdvertisePhotosCommand>,
        IRequestHandler<UpdateAdvertiseLicenseFileCommand, long>
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
            var extension = Path.GetExtension(request.ImagePath);
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

            string file = Path.GetFileNameWithoutExtension(request.ImagePath);
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
                lock (objlock)
                {
                    var files = new List<Entities.File>();
                    if (request.PhotoAlbumIds.Count > 0)
                    {
                        files.AddRange(fileRepository.Query(q => q.Where(w => request.PhotoAlbumIds.Contains(w.Id))));
                    }
                    if (request.MainPhotoId != null && request.PhotoAlbumIds.Contains((long)request.MainPhotoId) == false)
                    {
                        files.Add(fileRepository.Find((long)request.MainPhotoId));
                    }
                    var watermarkedImageList = new List<string>();
                    var thumbs = new List<ImageThumbDTO>();
                    var accThumbPath = Path.Combine(host.WebRootPath, "Content", "accthumb", request.AdvertiseId.ToString());

                    foreach (var file in files)
                    {
                        var fileThumbPath = $"{accThumbPath}/{file.Id}";
                        if (File.Exists(Path.Combine(host.WebRootPath, file.CorrectedFilePath)) == false ||
                            (request.IsEdit == false && Directory.Exists(fileThumbPath)))
                        {
                            continue;
                        }
                        var waterPath = mediator.Send(new SetWatermarkCommand(file.Id)).Result;
                        var watermarkedImagePath = string.IsNullOrEmpty(waterPath) ?
                            Path.Combine(host.WebRootPath, file.CorrectedFilePath) :
                            host.WebRootPath + waterPath;
                        watermarkedImageList.Add(watermarkedImagePath);

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
                            using (Image thumbImage = ImageUtility.ResizeImageKeepAspectRatio(origImage, thumb.w, thumb.h))
                            {
                                ImageUtility.SaveThumb(thumbImage, thumb.thumbPath, thumb.OrigPath);
                            }
                        }
                    }

                    foreach (var item in watermarkedImageList)
                    {
                        if (File.Exists(item))
                        {
                            File.Delete(item);
                        }
                    }
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
                var filePath = Path.Combine(host.WebRootPath, file.CorrectedFilePath);
                string waterPath = string.Empty;

                double ratio = 4.5;
                using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (Image watermarkImage = Image.FromFile(host.WebRootPath + "/resource/img/water_logo.png"))
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
                            logoPath = "/content/advertise/temp/" + string.Format("logo_{0}.png", file.Id);
                            ImageCodecInfo format = ImageUtility.GetEncoder(ImageFormat.Png);
                            EncoderParameters encoderParameters = new EncoderParameters(1);
                            encoderParameters.Param[0] = new EncoderParameter(Encoder.Compression, (long)EncoderValue.CompressionLZW);
                            thumbnailBitmap.Save(host.WebRootPath + logoPath, format, encoderParameters);
                        }
                    }

                    using (Image NewWatermarkImage = Image.FromFile(host.WebRootPath + logoPath))
                    using (Graphics imageGraphics = Graphics.FromImage(image))
                    using (TextureBrush watermarkBrush = new TextureBrush(NewWatermarkImage))
                    {
                        int x = image.Width - Convert.ToInt16((double)waterWidth + ((double)waterWidth / 10));
                        int y = image.Height - Convert.ToInt16((double)waterHeight + ((double)waterHeight / 10));
                        watermarkBrush.TranslateTransform(x, y);
                        imageGraphics.FillRectangle(watermarkBrush, new Rectangle(new Point(x, y), new Size(waterWidth + 1, waterHeight)));
                        waterPath = "/content/advertise/temp/" + file.FilePath.Substring(file.FilePath.LastIndexOf('/') + 1);
                        var extension = Path.GetExtension(host.WebRootPath + waterPath);
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
                        image.Save(host.WebRootPath + waterPath, format, encoderParameters);
                    }
                    if (File.Exists(host.WebRootPath + logoPath))
                    {
                        File.Delete(host.WebRootPath + logoPath);
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
                var advertiseThumbPath = Path.Combine(host.WebRootPath, "Content", "accthumb", request.AdvertiseId.ToString());
                lock (objlock)
                {
                    foreach (var item in files)
                    {
                        if (item.Advertises.Count == 0)
                        {
                            fileRepository.Delete(item.Id);
                            if (File.Exists(Path.Combine(host.WebRootPath, item.CorrectedFilePath)))
                            {
                                File.Delete(Path.Combine(host.WebRootPath, item.CorrectedFilePath));
                            }
                            hasChange = true;
                        }
                        var fileThumbPath = Path.Combine(advertiseThumbPath, item.Id.ToString());
                        if (Directory.Exists(fileThumbPath))
                        {
                            Directory.Delete(fileThumbPath, true);
                        }
                    }
                }
                if (hasChange)
                {
                    fileRepository.Save();
                }
                if (Directory.EnumerateDirectories(advertiseThumbPath).Any() == false)
                {
                    Directory.Delete(advertiseThumbPath, true);
                }
            }
            catch (Exception exc)
            {
                logger.Error("FileCommandHandler.RemovePhotosByFileIdsCommand", exc);
            }
            return Task.FromResult(Unit.Value);
        }

        public Task<Unit> Handle(RemovePhotosByPathsCommnd request, CancellationToken cancellationToken)
        {
            try
            {
                lock (objlock)
                {
                    foreach (var item in request.PathList)
                    {
                        if (File.Exists(Path.Combine(host.WebRootPath, item)))
                        {
                            File.Delete(Path.Combine(host.WebRootPath, item));
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                logger.Error("FileCommandHandler.RemovePhotosByPathsCommnd", exc);
            }
            return Task.FromResult(Unit.Value);
        }

        public Task<Unit> Handle(RenameAdvertisePhotosCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var acc = fileRepository.Find<Entities.Advertise, long>(request.AdvertiseId);
                lock (objlock)
                {
                    foreach (var item in acc.Photos)
                    {
                        if (item.FilePath.Contains($"/advertise_{item.Id}.") ||
                            item.FilePath.Contains($"/advertise_0_") ||
                            item.FilePath.Contains($"/advertise_") == false)
                        {
                            var fileName = $"advertise_{acc.Id}_{item.Id}";
                            var oldFilePath = Path.Combine(host.WebRootPath, item.CorrectedFilePath);
                            var newDbFilePath = $"content/advertise/{fileName}.jpg";
                            var newFilePath = $"{host.WebRootPath}/content/advertise/{fileName}.jpg";
                            if (File.Exists(oldFilePath))
                            {
                                var file = fileRepository.Find(item.Id);
                                file.FilePath = newDbFilePath;
                                fileRepository.Update(file);
                                fileRepository.Save();
                                File.Move(oldFilePath, newFilePath, true);
                            }
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                logger.Error("FileCommandHandler.RenameAdvertisePhotosCommand", exc);
            }
            return Task.FromResult(Unit.Value);
        }

        public Task<long> Handle(UpdateAdvertiseLicenseFileCommand request, CancellationToken cancellationToken)
        {
            var contentType = request.NewLicenseFile.ContentType.ToLower();
            if ((contentType == "image/png" ||
                contentType == "image/jpg" ||
                contentType == "image/jpeg") == false)
            {
                return Task.FromResult((long)0);
            }

            string filepath = $"{Entities.File.ResidenceLicenseImagesDirectory}/license_{request.AdvertiseId}.jpg";
            if (request.LicenseFileId != null && request.LicenseFileId > 0)
            {
                var oldFile = fileRepository.Find(request.LicenseFileId.Value);
                oldFile.LastModifyDate = DateTime.Now;
                fileRepository.Update(oldFile);
                fileRepository.Save();
            }
            else
            {
                var newLicenseFile = new Entities.File()
                {
                    PostDate = DateTime.Now,
                    LastModifyDate = DateTime.Now,
                    UserID = request.UserId,
                    FilePath = filepath,
                    Type = Entities.File.FileTypeEnum.ResidenceLicense
                };
                fileRepository.Insert(newLicenseFile);
                fileRepository.Save();
                request.LicenseFileId = newLicenseFile.Id;
            }

            if (Directory.Exists(host.WebRootPath + "/content/licenses") == false)
            {
                Directory.CreateDirectory(host.WebRootPath + "/content/licenses");
            }

            using (var stream = File.Create(Path.Combine(host.WebRootPath, filepath)))
            {
                request.NewLicenseFile.CopyTo(stream);
            }
            return Task.FromResult((long)request.LicenseFileId);
        }
    }
}
