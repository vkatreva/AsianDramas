using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AsianDramas.Data;
using AsianDramas.Models;
using AsianDramas.Models.ViewModels;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace AsianDramas.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Home/Index
        public async Task<IActionResult> Index(
            string? region, string? genre, string? status, string? search,
            int page = 1, int pageSize = 12)
        {
            IQueryable<Drama> dramasQuery = _context.Dramas.AsQueryable();

            // REGION FILTER
            Drama.DramaRegion parsedRegion = default;
            bool hasRegion = !string.IsNullOrEmpty(region)
                             && Enum.TryParse(region, true, out parsedRegion);

            if (hasRegion)
                dramasQuery = dramasQuery.Where(d => d.Region == parsedRegion);

            // GENRE FILTER
            if (!string.IsNullOrWhiteSpace(genre))
                dramasQuery = dramasQuery.Where(d => d.Genre == genre);


            // STATUS FILTER
            if (!string.IsNullOrWhiteSpace(status))
                dramasQuery = dramasQuery.Where(d => d.Status == status);


            // SEARCH
            if (!string.IsNullOrWhiteSpace(search))
                dramasQuery = dramasQuery.Where(d => d.Title.Contains(search));


            // OPTIONS FOR SELECT
            var genreOptions = Enum.GetNames(typeof(Genre))
                                   .Where(g => g != nameof(Genre.Unknown))
                                   .ToList();

            var statusOptions = Enum.GetNames(typeof(DramaStatus))
                                     .Where(s => s != nameof(DramaStatus.Unknown))
                                     .ToList();

            HomeIndexViewModel vm;


            // HOME PAGE (NO FILTERS) 
            if (!hasRegion && string.IsNullOrEmpty(genre) && string.IsNullOrEmpty(status) && string.IsNullOrEmpty(search))
            {
                var latest = await _context.Dramas
                    .OrderByDescending(d => d.ReleaseDate)
                    .Take(6)
                    .Select(d => new MovieVm
                    {
                        Id = d.Id,
                        Title = d.Title,
                        PosterUrl = string.IsNullOrEmpty(d.PosterUrl) ? "/images/fallback-poster.jpg" : d.PosterUrl,
                        Region = d.Region.ToString(),
                        Year = d.Year,
                        AverageRating = d.AverageRating,
                        Genre = d.Genre,
                        Status = d.Status,
                        ReleaseDate = d.ReleaseDate,
                        Episodes = d.Episodes,
                        Description = d.Description
                    })
                    .ToListAsync();

                var trending = await _context.Dramas
                    .OrderByDescending(d => d.AverageRating)
                    .ThenByDescending(d => d.Reviews.Count)
                    .Take(6)
                    .Select(d => new MovieVm
                    {
                        Id = d.Id,
                        Title = d.Title,
                        PosterUrl = string.IsNullOrEmpty(d.PosterUrl) ? "/images/fallback-poster.jpg" : d.PosterUrl,
                        Region = d.Region.ToString(),
                        Year = d.Year,
                        AverageRating = d.AverageRating,
                        Genre = d.Genre,
                        Status = d.Status,
                        ReleaseDate = d.ReleaseDate,
                        Episodes = d.Episodes
                    })
                    .ToListAsync();

                vm = new HomeIndexViewModel
                {
                    LatestReleases = latest,
                    Trending = trending,
                    FilteredDramas = null,

                    GenreOptions = genreOptions,
                    StatusOptions = statusOptions,

                    SelectedGenre = genre,
                    SelectedStatus = status,
                    SelectedRegion = region,
                    Search = search,

                    PageNumber = 1,
                    TotalPages = 1
                };

                return View(vm);
            }


            // FILTERED RESULTS (WITH PAGING)

            int totalDramas = await dramasQuery.CountAsync();

            int totalPages = (int)Math.Ceiling(totalDramas / (double)pageSize);

            var filtered = await dramasQuery
                .OrderByDescending(d => d.ReleaseDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(d => new MovieVm
                {
                    Id = d.Id,
                    Title = d.Title,
                    PosterUrl = string.IsNullOrEmpty(d.PosterUrl) ? "/images/fallback-poster.jpg" : d.PosterUrl,
                    Region = d.Region.ToString(),
                    Year = d.Year,
                    AverageRating = d.AverageRating,
                    Genre = d.Genre,
                    Status = d.Status,
                    ReleaseDate = d.ReleaseDate,
                    Episodes = d.Episodes
                })
                .ToListAsync();

            vm = new HomeIndexViewModel
            {
                
                FilteredDramas = filtered,

                GenreOptions = genreOptions,
                StatusOptions = statusOptions,

                SelectedGenre = genre,
                SelectedStatus = status,
                SelectedRegion = region,
                Search = search,

                PageNumber = page,
                TotalPages = totalPages
            };

            return View(vm);
        }

        public IActionResult Search(string query)
        {
            return RedirectToAction("Index", new { search = query });
        }
    }
}
