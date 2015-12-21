using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YoothubAPI.Models;

namespace YoothubAPI.Services
{
    public class PlaybackService : IPlaybackService
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public Song CurrentSong { get; private set; }
    }
}
