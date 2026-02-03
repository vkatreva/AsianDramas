using System.ComponentModel.DataAnnotations;

namespace AsianDramas.Models.ViewModels
{
    public class MyListItemEditViewModel
    {
        public int Id { get; set; }

        [Required]
        public MyListStatus Status { get; set; }

        [MaxLength(500)]
        public string? Note { get; set; }

        
        public int DramaId { get; set; }
        public string? DramaTitle { get; set; }
        public string? PosterUrl { get; set; }
    }
}
