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

namespace Amlakbashi.Application.Services.FileServices.CommandHandlers
{
    public class FileCommandHandler : IRequestHandler<MinifyImageCommand>,
        IRequestHandler<StopQueuedJobCommand>,
        IRequestHandler<GenerateThumbImageCommand, bool>,
        IRequestHandler<SetWatermarkCommand>
    {
        private static readonly object objlock = new object();
        private readonly IRepository<Entities.File, long> fileRepository;
        private readonly ILog logger;
        public FileCommandHandler(ILog logger,
            IRepository<Entities.File, long> fileRepository)
        {
            this.fileRepository = fileRepository;
            this.logger = logger;
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
            var accThumbPath = request.Path + "content/accthumb/" + request.AdvertiseId;
            if (System.IO.Directory.Exists(accThumbPath))
            {
                lock (objlock)
                {
                    System.IO.Directory.Delete(accThumbPath, true);
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
                    using (FileStream stream = new FileStream(thumb.OrigPath, FileMode.Open, FileAccess.Read))
                    {
                        var origImage = Image.FromStream(stream);
                        var thumbImage = ImageUtility.ResizeImageKeepAspectRatio(origImage, thumb.w, thumb.h);
                        ImageUtility.SaveThumb(thumbImage, thumb.thumbPath, thumb.OrigPath);
                        stream.Close();
                        stream.Dispose();
                    }
                }
            }
            return Task.FromResult(true);
        }

        public Task<Unit> Handle(SetWatermarkCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var file = fileRepository.Find(request.FileId);
                var filePath = request.ServerPath + file.FilePath.Replace("~", "").Substring(1);
                if (file != null)
                {
                    lock (objlock)
                    {
                        double ratio = 4.5;
                        if (!File.Exists(filePath))
                            return Task.FromResult(Unit.Value);
                        using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                        using (Image watermarkImage = Image.FromFile(request.ServerPath + "resource/img/water_logo.png"))
                        {
                            Image image = Image.FromStream(stream);
                            int water_width = Convert.ToInt16((double)image.Width / ratio);
                            double water_rate = (double)watermarkImage.Width / (double)water_width;
                            int water_height = Convert.ToInt16((double)watermarkImage.Height / water_rate);
                            string logo_path = "";

                            using (Bitmap thumbnailBitmap = new Bitmap(water_width, water_height))
                            {
                                thumbnailBitmap.SetResolution(watermarkImage.HorizontalResolution, watermarkImage.VerticalResolution);
                                using (Graphics new_watermark = Graphics.FromImage(thumbnailBitmap))
                                {
                                    new_watermark.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                                    new_watermark.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                                    new_watermark.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                                    Rectangle imageRectangle = new Rectangle(0, 0, water_width, water_height);
                                    new_watermark.DrawImage(watermarkImage, imageRectangle);
                                    logo_path = "content/logo/" + string.Format("logo_{0}.png", Guid.NewGuid());
                                    var extension = System.IO.Path.GetExtension(request.ServerPath + logo_path);
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
                                    thumbnailBitmap.Save(request.ServerPath + logo_path, format, encoderParameters);
                                }
                            }

                            using (Image NewWatermarkImage = Image.FromFile(request.ServerPath + logo_path))
                            using (Graphics imageGraphics = Graphics.FromImage(image))
                            using (TextureBrush watermarkBrush = new TextureBrush(NewWatermarkImage))
                            {
                                int x = image.Width - Convert.ToInt16((double)water_width + ((double)water_width / 10));
                                int y = image.Height - Convert.ToInt16((double)water_height + ((double)water_height / 10));
                                watermarkBrush.TranslateTransform(x, y);
                                imageGraphics.FillRectangle(watermarkBrush, new Rectangle(new Point(x, y), new Size(water_width + 1, water_height)));
                                string water_path = "~/content/advertise/" + "watermark_" +
                                    file.FilePath.Substring(file.FilePath.LastIndexOf('/') + 1);
                                var extension = System.IO.Path.GetExtension(request.ServerPath + water_path.Replace("~/", ""));
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
                                image.Save(request.ServerPath + water_path.Replace("~/", ""), format, encoderParameters);
                                file.FilePath = water_path;
                                fileRepository.Update(file);
                                fileRepository.Save();
                            }
                            stream.Close();
                            stream.Dispose();
                        }
                    }
                }
                if (File.Exists(filePath))
                    lock (objlock)
                    {
                        File.Delete(filePath);
                    }
            }
            catch (Exception exc)
            {
                logger.Error("File.SetWatermarkForFile", exc);
            }
            return Task.FromResult(Unit.Value);
        }
    }
}
