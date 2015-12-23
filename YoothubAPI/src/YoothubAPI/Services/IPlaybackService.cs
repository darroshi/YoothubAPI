using YoothubAPI.Models;

namespace YoothubAPI.Services
{
    public interface IPlaybackService
    {
        Song GetCurrentSong();
        void Wish(Song s);
    }
}