using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity;
using Swashbuckle.SwaggerGen.Annotations;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using YoothubAPI.Models;
using YoothubAPI.Services;

namespace YoothubAPI.Controllers.Play
{
    [Route("api/[controller]")]
    [Authorize]
    public class PlayController : Controller
    {
        private readonly IPlaybackService _playbackService;
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public PlayController(IPlaybackService playbackService, ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
        {
            _playbackService = playbackService;
            _db = dbContext;
            _userManager = userManager;
        }

        [HttpGet]
        [AllowAnonymous]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CurrentSongJson))]
        public async Task<IActionResult> Get()
        {
            var currentSong = await Task.Run(() => _playbackService.GetCurrentSong());
            var currentUser = await _userManager.FindByIdAsync(User.GetUserId());
            var currentUserId = currentUser?.Id;

            var vote = await _db.Votes
                .Include(v => v.User)
                .FirstOrDefaultAsync(v => v.Id == currentSong.Id && v.User.Id == currentUserId);

            return Json(new CurrentSongJson { Song = currentSong, CurrentVote = vote != null ? (VoteType?)vote.VoteType : null });
        }
    }
}
