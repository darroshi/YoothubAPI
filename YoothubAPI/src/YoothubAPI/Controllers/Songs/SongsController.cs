﻿using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YoothubAPI.Models;
using System.Security.Claims;
using YoothubAPI.Services;
using Microsoft.QueryStringDotNET;
using Swashbuckle.SwaggerGen.Annotations;
using System.Net;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Internal;
using Microsoft.Extensions.Logging;

namespace YoothubAPI.Controllers.Songs
{
    [Route("api/[controller]")]
    public class SongsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        private readonly IYoutubeService _youtubeService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger _logger;

        public SongsController(IYoutubeService youtubeService, UserManager<ApplicationUser> userManager, ILoggerFactory loggerFactory)
        {
            _youtubeService = youtubeService;
            _userManager = userManager;
            _logger = loggerFactory.CreateLogger<SongsController>();
        }

        // GET: api/songs?page=5&pageSize=20
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(SongsJson))]
        public async Task<IActionResult> Get([FromQuery]int? page, [FromQuery]int? pageSize = 20)
        {
            if (page < 0 || pageSize < 0) return new BadRequestResult();

            var count = db.Songs.Count();
            var currentUser = await _userManager.FindByIdAsync(User.GetUserId());
            var currentUserId = currentUser?.Id;

            var songs = db.Songs
                        .Include(s => s.AddedBy)
                        .Include(s => s.SongTags)
                        .ThenInclude(st => st.Tag);

            var votes = db.Votes
                .Include(v => v.User)
                .Where(v => v.User.Id == currentUserId).ToList();

            var result = songs.Select((s => new SongJson { Song = s, CurrentVote = (votes.Any(v => v.SongId == s.Id) ? (VoteType?)votes.FirstOrDefault(v => v.SongId == s.Id).VoteType : null) }));

            if (!page.HasValue)
                return Json(new SongsJson
                {
                    Count = count,
                    Results = result
                });

            return Json(new SongsJson
            {
                Count = count,
                Results = result
                    .Skip(page.Value * pageSize.Value).Take(pageSize.Value)
            });
        }

        // GET api/songs/5
        [HttpGet("{id}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(SongJson))]
        public async Task<IActionResult> Get(int id)
        {
            var song = await db.Songs
                        .Include(s => s.AddedBy)
                        .Include(s => s.SongTags)
                        .ThenInclude(st => st.Tag)
                        .FirstOrDefaultAsync(s => s.Id == id);

            var currentUser = await _userManager.FindByIdAsync(User.GetUserId());
            var currentUserId = currentUser?.Id;

            var vote = await db.Votes
                .Include(v => v.User)
                .FirstOrDefaultAsync(v => v.Id == id && v.User.Id == currentUserId);

            return Json(new SongJson { Song = song, CurrentVote = vote != null ? (VoteType?)vote.VoteType : null } );
        }

        // POST api/songs
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Post([FromBody]AddSongJson value)
        {
            var query = new Uri(value.URL).Query.TrimStart('?');

            var ytId = QueryString.Parse(query)["v"];
            var ytInfo = await _youtubeService.GetYoutubeInfo(ytId);
            var currentUser = await _userManager.FindByIdAsync(User.GetUserId());

            var song = new Song
            {
                Added = DateTime.Now,
                AddedBy = currentUser,
                Broken = false,
                Duration = ytInfo.Duration,
                LastPlayed = DateTime.MinValue,
                Title = ytInfo.Title,
                URL = value.URL,
                SongId = ytId,
                Votes = 0
            };

            foreach(var tag in value.Tags)
            {
                var dbTag = db.Tags.FirstOrDefault(t => t.Name == tag);
                if(dbTag == null)
                {
                    dbTag = new Tag
                    {
                        Name = tag
                    };
                    db.Tags.Add(dbTag);
                }

                db.SongTags.Add(
                    new SongTag()
                    {
                        Song = song,
                        Tag = dbTag
                    });
            }

            db.Add(song);
            db.SaveChanges();

            return new EmptyResult();
        }

        // DELETE api/songs/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var song = await db.Songs.Include(s => s.AddedBy).FirstOrDefaultAsync(s => s.Id == id);
            if (song == null) return new BadRequestObjectResult("Song with given id doesn't exist.");

            if (song.AddedBy.Id != User.GetUserId())
                return new HttpStatusCodeResult(403);

            db.Remove(song);
            db.SaveChanges();
            return new EmptyResult();
        }

        [HttpPost("Upvote/{id}")]
        [Authorize]
        public async Task<IActionResult> Upvote(int id)
        {
            var song = await db.Songs.FirstOrDefaultAsync(s => s.Id == id);
            if (song == null) return new BadRequestObjectResult("Song with given id doesn't exist.");
            var result = await VoteSong(song, VoteType.Upvote);
            return result;
        }

        [HttpPost("Downvote/{id}")]
        [Authorize]
        public async Task<IActionResult> Downvote(int id)
        {
            var song = await db.Songs.FirstOrDefaultAsync(s => s.Id == id);
            if (song == null) return new BadRequestObjectResult("Song with given id doesn't exist.");
            var result = await VoteSong(song, VoteType.Downvote);
            return result;
        }

        private async Task<IActionResult> VoteSong(Song song, VoteType voteType)
        {
            var currentUser = await _userManager.FindByIdAsync(User.GetUserId());

            var vote = db.Votes.FirstOrDefault(v => v.Song.Id == song.Id && v.User.Id == currentUser.Id);
            if(vote != null)
            {
                if (vote.VoteType == voteType)
                    return new ObjectResult("User already voted on that song.") { StatusCode = 406 };
                else
                {
                    vote.VoteType = voteType;
                    switch (voteType)
                    {
                        case VoteType.Upvote:
                            song.Votes++;
                            break;

                        case VoteType.Downvote:
                            song.Votes--;
                            break;
                    }
                }
                    
            }
            else
            {
                vote = new Vote()
                {
                    Song = song,
                    User = currentUser,
                    VoteType = voteType
                };

                db.Votes.Add(vote);
            }

            switch (voteType)
            {
                case VoteType.Upvote:
                    song.Votes++;
                    break;

                case VoteType.Downvote :
                    song.Votes--;
                    break;
            }

            db.SaveChanges();
            return new EmptyResult();
        }
    }
}