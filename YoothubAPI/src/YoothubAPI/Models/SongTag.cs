using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace YoothubAPI.Models
{
    public class SongTag
    {
        public int Id { get; set; }

        [Required]
        public Song Song { get; set; }

        [Required]
        public Tag Tag { get; set; }
    }
}
