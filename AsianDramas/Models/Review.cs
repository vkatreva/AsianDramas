using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.ComponentModel.DataAnnotations;

namespace AsianDramas.Models
{
    public class Review
    {
        public int Id { get; set; }

        [Required]
        public int DramaId { get; set; }
        public Drama Drama { get; set; }

        [ValidateNever]
        [StringLength(50)]
        public string ReviewerName { get; set; } 

        [Required]
        [Range(1, 10)]
        public int Rating { get; set; } 

        [Required]
        [StringLength(1000)]
        public string Comment { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        
        public long Likes { get; set; }
    }
}
