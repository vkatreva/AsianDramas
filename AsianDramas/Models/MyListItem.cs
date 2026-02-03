using AsianDramas.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace AsianDramas.Models
{
    public class MyListItem
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(450)]
        public string UserId { get; set; }
        public AppUser User { get; set; }

        [Required]
        public int DramaId { get; set; }
        public Drama Drama { get; set; }

        public DateTime AddedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public MyListStatus Status { get; set; } = MyListStatus.Planned;


        [MaxLength(500)]
        public string? Note { get; set; }
    }
}
