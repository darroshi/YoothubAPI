using Microsoft.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YoothubAPI.Models;

namespace YoothubAPI.Services
{
    public class PlaybackService : IPlaybackService
    {
        private int _currentSongId;
        private Queue<int> _wishQueue = new Queue<int>();

        public Song GetCurrentSong()
        {
            using (var _db = new ApplicationDbContext())
            {
                var songs = _db.Songs
                    .Include(s => s.AddedBy)
                    .Include(s => s.SongTags)
                    .ThenInclude(st => st.Tag)
                    .OrderBy(s => s.LastPlayed).ToList();

                var currentSong = songs.FirstOrDefault(s => s.Id == _currentSongId);
                if (currentSong == null || currentSong.LastPlayed.Add(currentSong.Duration) < DateTime.Now)
                {
                    _currentSongId = _wishQueue.Any() ? _wishQueue.Dequeue() : songs.First().Id;
                    currentSong = songs.FirstOrDefault(s => s.Id == _currentSongId);

                    currentSong.LastPlayed = DateTime.Now;
                    currentSong.TimesPlayed++;

                    _db.SaveChanges();
                }

                return currentSong;
            }    
        }

        public void Wish(int id)
        {
            _wishQueue.Enqueue(id);
        }
    }
}
