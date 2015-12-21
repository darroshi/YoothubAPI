using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YoothubAPI.Models
{
    public class Vote
    {
        public int Id { get; set; }

        public Song Song { get; set; }

        public ApplicationUser User { get; set; }

        public VoteType VoteType { get; set; }
    }

    public enum VoteType
    {
        Upvote, Downvote
    }
}
