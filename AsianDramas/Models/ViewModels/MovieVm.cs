namespace AsianDramas.Models.ViewModels
{
    public class MovieVm
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string PosterUrl { get; set; }
        public string Region { get; set; }
        public int Year { get; set; }

        public decimal AverageRating { get; set; }
        public string Genre { get; set; }
        public string Status { get; set; }

        public DateTime ReleaseDate { get; set; }
        public int Episodes { get; set; }

        public string Description { get; set; }

        public int MyListItemId { get; set; }

        public MyListStatus MyListStatus { get; set; }
        public string? MyListNote { get; set; }
        
    }
}
