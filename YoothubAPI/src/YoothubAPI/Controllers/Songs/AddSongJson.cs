using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using YoothubAPI.Validators;

namespace YoothubAPI.Controllers.Songs
{
    public class AddSongJson
    {
        [Required]
        [YoutubeURL]
        public string URL { get; set; }

        public IEnumerable<string> Tags { get; set; }
    }
}
