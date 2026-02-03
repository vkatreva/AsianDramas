using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AsianDramas.Models
{
    public class Drama
    {
        public int Id { get; set; } 

        [Required]
        [StringLength(150)]
        public string Title { get; set; }

        [Required]
        [StringLength(1000)]
        public string Description { get; set; }

        [Required]
        public DateTime ReleaseDate { get; set; }

        [Range(1, 2000)]
        public int Episodes { get; set; }


        [Required]
        [Range(0, 10)]
        public decimal AverageRating { get; set; } 


        public enum DramaRegion
        {
            Korea,
            China,
            Japan,
            Thailand,
            Philippines,
            Taiwan,
            HongKong,
            Other
        }


        [Required]
        public DramaRegion Region { get; set; }

        [StringLength(100)]
        public string Genre { get; set; } // Romance, Thriller, Historical...

        [StringLength(50)]
        public string Status { get; set; } // Completed, Ongoing, Upcoming

        [Range(1900, 2100)]
        public int Year { get; set; }
        public bool IsMovie { get; set; }


        public ICollection<DramaActor> DramaActors { get; set; } = new List<DramaActor>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();


        [StringLength(500)]
        public string PosterUrl { get; set; }
    }
}
