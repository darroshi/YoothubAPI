using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YoothubAPI.Models
{
    public class Song
    {
        public int Id { get; set; }

        public string URL { get; set; }

        public string Title { get; set; }

        public int Votes { get; set; }

        public DateTime LastPlayed { get; set; }

        public DateTime Added { get; set; }

        public string AddedById { get; set; }

        public bool Broken { get; set; }

        public TimeSpan Duration { get; set; }
    }
}
