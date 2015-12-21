using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YoothubAPI.Controllers.Songs
{
    public class AddSongJson
    {
        [JsonProperty(Required = Required.Always)]
        public string URL { get; set; }

        public IEnumerable<string> Tags { get; set; }
    }
}
