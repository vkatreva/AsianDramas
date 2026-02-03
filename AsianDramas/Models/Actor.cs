using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;


namespace AsianDramas.Models
{
    public class Actor
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; }

        [StringLength(2000)]
        public string Biography { get; set; }

        public DateTime? BirthDate { get; set; } 

        [StringLength(100)]
        public string Nationality { get; set; }

        [StringLength(200)]
        public string ImageUrl { get; set; }

        
        public int PopularityScore { get; set; }

        [ValidateNever]
        public ICollection<DramaActor> DramaActors { get; set; }
        public ICollection<ActorReview> Reviews { get; set; } = new List<ActorReview>();
    }
}
