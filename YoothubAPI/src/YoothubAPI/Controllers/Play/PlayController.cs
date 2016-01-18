using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        public IActionResult Get()
        {
            var currentSong = _playbackService.GetCurrentSong();
            return Json(currentSong);
        }
    }
}
