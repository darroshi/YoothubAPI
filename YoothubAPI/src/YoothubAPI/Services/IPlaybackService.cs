using YoothubAPI.Models;

namespace YoothubAPI.Services
{
    public interface IPlaybackService
    {
        Song GetCurrentSong();
        void Wish(int s);
    }
}