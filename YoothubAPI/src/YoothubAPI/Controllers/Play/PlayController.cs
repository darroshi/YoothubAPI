using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;
using YoothubAPI.Services;

namespace YoothubAPI.Controllers.Play
{
    [Route("api/[controller]")]
    [Authorize]
    public class PlayController : Controller
    {
        private readonly IPlaybackService _playbackService;

        public PlayController(IPlaybackService playbackService)
        {
            _playbackService = playbackService;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Get()
        {
            var currentSong = _playbackService.GetCurrentSong();
            return Json(currentSong);
        }
    }
}
