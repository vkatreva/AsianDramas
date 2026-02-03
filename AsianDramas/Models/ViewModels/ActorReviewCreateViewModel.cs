using System.ComponentModel.DataAnnotations;

namespace AsianDramas.Models.ViewModels
{
    public class ActorReviewCreateViewModel
    {
        public int ActorId { get; set; }

        [Required]
        [Range(1, 10)]
        public int Rating { get; set; }

        [MaxLength(500)]
        public string? Comment { get; set; }
    }

}
