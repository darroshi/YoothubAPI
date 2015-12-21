using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace YoothubAPI.Services
{
    public class YoutubeService : IYoutubeService
    {
        private readonly ILogger _logger;

        public YoutubeService(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<YoutubeService>();
        }

        public async Task<YoutubeInfo> GetYoutubeInfo(string videoId)
        {
            string url = "https://www.googleapis.com/youtube/v3/videos?part=contentDetails%2Csnippet&id={0}&key={1}";
            string googleApiKey = "AIzaSyB22TTIMVwFTNuFpijo2tcs3RQreWDVTII";

            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage response = await client.GetAsync(string.Format(url, videoId, googleApiKey)))
            using (HttpContent content = response.Content)
            {
                string result = await content.ReadAsStringAsync();
                var json = JsonConvert.DeserializeObject<dynamic>(result);

                return new YoutubeInfo
                {
                    Title = json.items[0].snippet.title,
                    Duration = 15
                };
            }
        }
    }
}
