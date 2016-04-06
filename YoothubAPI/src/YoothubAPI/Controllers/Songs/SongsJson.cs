using System.Collections.Generic;

namespace YoothubAPI.Controllers.Songs
{
    public class SongsJson
    {
        public int Count { get; set; }

        public IEnumerable<SongJson> Results { get; set; }
    }
}
