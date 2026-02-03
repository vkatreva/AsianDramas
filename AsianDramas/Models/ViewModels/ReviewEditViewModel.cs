using System.ComponentModel.DataAnnotations;

namespace AsianDramas.Models.ViewModels
{
    public class ReviewEditViewModel
    {
        public int Id { get; set; }

        public int DramaId { get; set; }   

        [Range(1, 10)]
        public int Rating { get; set; }

        [StringLength(1000)]
        public string Comment { get; set; }
    }
}
