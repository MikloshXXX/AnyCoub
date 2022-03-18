using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xabe.FFmpeg;

namespace AnyCoub
{
    internal static class FFMpegService
    {
        public static async Task MakeCoub(string basePath, string title, string mp3Path, string mp4path, double duration)
        {
            IMediaInfo mp4Info = await FFmpeg.GetMediaInfo(mp4path);
            IMediaInfo mp3Info = await FFmpeg.GetMediaInfo(mp3Path);

            IVideoStream videoStream = mp4Info.VideoStreams.FirstOrDefault()?.SetCodec(VideoCodec.h264);
            IAudioStream audioStream = mp3Info.AudioStreams.FirstOrDefault()?.SetCodec(AudioCodec.aac);

            if (duration == 0)
            {
                videoStream?.SetStreamLoop((int)(audioStream.Duration.TotalSeconds / videoStream.Duration.TotalSeconds));
            } 
            else
            {
                string[] parts = Convert.ToString(duration).Split('.',',');
                audioStream?.Split(new TimeSpan(0), new TimeSpan(0, 0, 0, Convert.ToInt32(parts[0]), Convert.ToInt32(parts[1])));
            }

            string output = Path.Combine(basePath, $"AnyCoub-{title}.mp4");

            var conv = FFmpeg.Conversions.New()
                .AddStream(videoStream)
                .AddStream(audioStream)
                .SetOutput(output)
                .UseHardwareAcceleration(HardwareAccelerator.auto, VideoCodec.h264, VideoCodec.h264);
            await conv.Start();
        }
    }
}
