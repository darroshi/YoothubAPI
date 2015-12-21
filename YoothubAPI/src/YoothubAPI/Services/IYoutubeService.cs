using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YoothubAPI.Services
{
    public interface IYoutubeService
    {
        Task<YoutubeInfo> GetYoutubeInfo(string videoId);
    }
}
