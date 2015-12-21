using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YoothubAPI.Models;

namespace YoothubAPI.Controllers.Songs
{
    public class SongsJson
    {
        public int Count { get; set; }

        public IEnumerable<Song> Results { get; set; }
    }
}
