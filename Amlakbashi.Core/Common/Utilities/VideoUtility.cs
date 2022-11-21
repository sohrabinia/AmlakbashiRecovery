using Amlakbashi.Core.Common.StaticData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xabe.FFmpeg;

namespace Amlakbashi.Core.Common.Utilities
{
    public static class VideoUtility
    {
        public static string GetInfo(string path)
        {
            var mediaInfo = FFmpeg.GetMediaInfo(path).Result;
            var videoStream = mediaInfo.VideoStreams.FirstOrDefault();
            return $"Size:{mediaInfo.Size} - Duration:{mediaInfo.Duration} - Bitrate:{videoStream.Bitrate} - Codec:{videoStream.Codec} - Framerate:{videoStream.Framerate} - Resulation:{videoStream.Width}*{videoStream.Height} - Ratio:{videoStream.Ratio}";
        }

        public static async Task<(bool result, string errorMessage)> ConversionAsync(string inputPath, string outputPath)
        {
            try
            {
                var mediaInfo = await FFmpeg.GetMediaInfo(inputPath);
                var videoStream = mediaInfo.VideoStreams.FirstOrDefault();
                var audioStream = mediaInfo.AudioStreams.FirstOrDefault();

                var ratio = (double)videoStream.Width / (double)videoStream.Height;
                if (ratio > 1 && videoStream.Height > 480)
                {
                    videoStream.SetSize(VideoSize.Hd480);
                }
                videoStream.SetWatermark($"{GeneralData.WebHostEnvironment.WebRootPath}\\image\\logo\\logo-80x80.png", Position.UpperRight);

                await FFmpeg.Conversions.New()
                    .AddStream<IStream>(videoStream, audioStream)
                    .SetOverwriteOutput(true)
                    .AddParameter("-crf 28")
                    .SetOutput(outputPath).Start();
                return (true, string.Empty);
            }
            catch (Exception exc)
            {
                return (false, exc.Message);
            }
            
        }
    }
}
