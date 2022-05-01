using Amlakbashi.Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Amlakbashi.Core.DTOs.WebService.Requests.Comments
{
    public class CommentPostGuestRequest
    {
        [Required]
        public long advertiseId { get; set; }

        [Required]
        public string text { get; set; }

        public List<CommentPostScoresRequest> scores { get; set; }
    }

    public class CommentPostScoresRequest
    {
        [Required]
        public ReportItem.ScoreType type { get; set; }

        [Required]
        [Range(1, 5)]
        public int score { get; set; }
    }
}
