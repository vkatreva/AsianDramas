using System;
using System.Collections.Generic;
using AsianDramas.Models;

namespace AsianDramas.Models.ViewModels
{
    public class DramaAllViewModel
    {
        public IEnumerable<Drama> Items { get; set; } = new List<Drama>();

        // Search
        public string? SearchTitle { get; set; }
        public string? Region { get; set; }   

        // Sort
        public string SortOrder { get; set; } = "title_asc";

        // Paging
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 12;
        public int TotalItems { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);

        // Options за dropdown
        public List<string> RegionOptions { get; set; } = new();
    }
}
