using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace YoothubAPI.Models
{
    public class Vote
    {
        public int Id { get; set; }

        [Required]
        public Song Song { get; set; }

        [Required]
        public ApplicationUser User { get; set; }

        [Required]
        public VoteType VoteType { get; set; }
    }

    public enum VoteType
    {
        Upvote, Downvote
    }
}
