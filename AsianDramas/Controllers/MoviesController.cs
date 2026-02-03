using AsianDramas.Data;
using AsianDramas.Models;
using AsianDramas.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace AsianDramas.Controllers
{
    public class MoviesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MoviesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index(string? searchTitle, Drama.DramaRegion? region,
    string sortOrder = "title_asc", int page = 1, int pageSize = 12)
        {

            var query = _context.Dramas
                .Where(d => d.IsMovie)   
                .AsQueryable();

            // SEARCH (2 criteria) 
            if (!string.IsNullOrWhiteSpace(searchTitle))
                query = query.Where(d => d.Title.Contains(searchTitle));

            if (region.HasValue)
                query = query.Where(d => d.Region == region.Value);

            // SORT 
            query = sortOrder switch
            {
                "title_desc" => query.OrderByDescending(d => d.Title),
                "year_asc" => query.OrderBy(d => d.Year),
                "year_desc" => query.OrderByDescending(d => d.Year),
                "rating_asc" => query.OrderBy(d => d.AverageRating),
                "rating_desc" => query.OrderByDescending(d => d.AverageRating),
                _ => query.OrderBy(d => d.Title)
            };

            // PAGING
            var totalItems = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var vm = new DramaAllViewModel
            {
                Items = items,
                SearchTitle = searchTitle,
                Region = region?.ToString(),
                SortOrder = sortOrder,
                Page = page,
                PageSize = pageSize,
                TotalItems = totalItems,
                RegionOptions = Enum.GetNames(typeof(Drama.DramaRegion)).ToList()
            };

            return View(vm);
        }



    }
}
