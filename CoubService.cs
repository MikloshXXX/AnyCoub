using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AnyCoub
{
    internal static class CoubService
    {
        public static async Task<bool> DownloadCoub(string coubId)
        {
            using var progress = new ProgressBar();

            HttpClient client = new HttpClient();
            var response = await client.GetAsync($"https://coub.com/api/v2/coubs/{coubId}.json");

            if (!response.IsSuccessStatusCode)
            {
                return await Task.FromResult(false);
            }

            progress.Report(0.1);

            using var content = response.Content;
            var json = await content.ReadAsStringAsync();
            var coubJson = JsonConvert.DeserializeObject<JObject>(json);
            var title = coubJson?["title"]?.ToString();

            if (title != null)
            {
                var charsToRemove = "\\/:*?\"<>|".ToCharArray();
                foreach (var ch in charsToRemove)
                {
                    title = title.Replace(ch.ToString(), "");
                }
            }

            var mp3Link = coubJson?["file_versions"]?["html5"]?["audio"]?["high"]?["url"]?.ToString();
            var mp4Link = coubJson?["file_versions"]?["html5"]?["video"]?["higher"]?["url"]?.ToString();
            var duration = Convert.ToDouble(coubJson?["file_versions"]?["html5"]?["audio"]?["sample_duration"]?.ToString());

            progress.Report(0.2);

            await DownloadFiles(title, mp3Link, mp4Link, client, progress, duration);

            return await Task.FromResult(true);
        }

        private static async Task DownloadFiles(string title, string linkMp3, string linkMp4, HttpClient client, ProgressBar progress, double duration)
        {
            var basePath = Path.Combine(
                Directory.GetCurrentDirectory(), 
                "Downloads", 
                Directory.CreateDirectory($"Downloads/{title}-{DateTime.Now.Year}{DateTime.Now.Month}{DateTime.Now.Day}-{DateTime.Now.Hour}{DateTime.Now.Minute}").Name);

            var responseMp3 = await client.GetAsync(linkMp3);
            var mp3Path = Path.Combine(basePath, $"{title}.mp3");
            await using var fileStreamMp3 = new FileStream(
                mp3Path,
                FileMode.CreateNew);

            progress.Report(0.5);

            var responseMp4 = await client.GetAsync(linkMp4);
            var mp4Path = Path.Combine(basePath, $"{title}.mp4");
            await using var fileStreamMp4 = new FileStream(
                mp4Path,
                FileMode.CreateNew);

            await responseMp3.Content.CopyToAsync(fileStreamMp3);
            await responseMp4.Content.CopyToAsync(fileStreamMp4);

            progress.Report(0.7);

            await FFMpegService.MakeCoub(basePath, title, mp3Path, mp4Path, duration);

            progress.Report(1);

            await Task.Delay(200);
        }
    }
}