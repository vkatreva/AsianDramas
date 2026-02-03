using System;
using System.ComponentModel.DataAnnotations;

namespace AsianDramas.Models
{
    public class ActorReview
    {
        public int Id { get; set; }

        [Required]
        public int ActorId { get; set; }
        public Actor Actor { get; set; } = null!;

        [Required]
        [MaxLength(450)]
        public string UserId { get; set; } = null!;
        public AppUser User { get; set; } = null!;

        [Range(1, 10)]
        public int Rating { get; set; }   // 1..10

        [MaxLength(1000)]
        public string? Comment { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
