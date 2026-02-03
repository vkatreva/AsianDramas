using System.Collections.Generic;

namespace AsianDramas.Models.ViewModels
{
    public class HomeIndexViewModel
    {
        public List<MovieVm> LatestReleases { get; set; } = new();
        public List<MovieVm> Trending { get; set; } = new();
        public List<RegionGroupViewModel> Regions { get; set; } = new();
        public List<MovieVm>? FilteredDramas { get; set; }
        public IEnumerable<string> GenreOptions { get; set; } = new List<string>();
        public IEnumerable<string> StatusOptions { get; set; } = new List<string>();

        public string SelectedGenre { get; set; }
        public string SelectedStatus { get; set; }
        public string SelectedRegion { get; set; }
        public string Search { get; set; }
        public int PageNumber { get; set; }
        public int TotalPages { get; set; }

    }

    public class RegionGroupViewModel
    {
        public string Region { get; set; }
        public List<MovieVm> Dramas { get; set; } = new();
    }
}
