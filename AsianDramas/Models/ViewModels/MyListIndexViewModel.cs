using System;
using System.Collections.Generic;

namespace AsianDramas.Models.ViewModels
{
    public class MyListIndexViewModel
    {
        public IEnumerable<MovieVm> Items { get; set; } = new List<MovieVm>();

        public string? SearchTitle { get; set; }
        public string? Status { get; set; }

        public string SortOrder { get; set; } = "added_desc";

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 12;
        public int TotalItems { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);
    }
}
