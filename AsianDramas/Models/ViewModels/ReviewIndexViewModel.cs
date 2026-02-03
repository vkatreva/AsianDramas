using System;
using System.Collections.Generic;
using AsianDramas.Models;

namespace AsianDramas.Models.ViewModels
{
    public class ReviewIndexViewModel
    {
        public IEnumerable<Review> Items { get; set; } = new List<Review>();

        public string? Reviewer { get; set; }
        public int? MinRating { get; set; }

        public string SortOrder { get; set; } = "date_desc";

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalItems { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);
    }
}
