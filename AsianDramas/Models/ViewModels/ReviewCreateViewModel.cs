using System.ComponentModel.DataAnnotations;

namespace AsianDramas.Models.ViewModels
{
    public class ReviewCreateViewModel
    {
        [Required]
        public int DramaId { get; set; }

        [Required]
        [Range(1, 10)]
        public int Rating { get; set; }

        [StringLength(1000)]
        public string Comment { get; set; }
    }
}
