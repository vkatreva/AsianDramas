using System.Collections.Generic;

namespace AsianDramas.Models.ViewModels
{
    public class DramaDetailsViewModel
    {
        public Drama Drama { get; set; }

        public List<CastItemVM> Cast { get; set; } = new List<CastItemVM>();

        public List<Review> Reviews { get; set; } = new List<Review>();

        public decimal AverageRating { get; set; }

        public bool IsInMyList { get; set; }
    }
}
