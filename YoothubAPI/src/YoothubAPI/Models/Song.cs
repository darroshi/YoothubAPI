using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace YoothubAPI.Models
{
    public class Song
    {
        public int Id { get; set; }

        public string URL { get; set; }

        public string SongId { get; set; }

        public string Title { get; set; }

        public int Votes { get; set; }

        public DateTime LastPlayed { get; set; }

        public DateTime Added { get; set; }

        [JsonIgnore]
        public ApplicationUser AddedBy { get; set; }

        [NotMapped]
        public string AddedByName
        {
            get
            {
                return AddedBy?.UserName ?? string.Empty;
            }
        }

        public int TimesPlayed { get; set; }

        public bool Broken { get; set; }

        public TimeSpan Duration { get; set; }

        [NotMapped]
        public int DurationInSeconds
        {
            get
            {
                return (int)Math.Ceiling(Duration.TotalSeconds);
            }
        }

        [JsonIgnore]
        public List<SongTag> SongTags { get; set; }

        [NotMapped]
        public IEnumerable<string> Tags
        {
            get
            {
                return SongTags.Select(st => st.Tag.Name);
            }
        }
    }
}
