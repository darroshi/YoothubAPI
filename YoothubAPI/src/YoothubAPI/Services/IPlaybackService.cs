using YoothubAPI.Models;

namespace YoothubAPI.Services
{
    public interface IPlaybackService
    {
        Song CurrentSong { get; }
    }
}