using AsianDramas.Data;
using AsianDramas.Models;
using AsianDramas.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AsianDramas.Controllers
{
    public class DramasController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        private const int PageSize = 12;

        public DramasController(ApplicationDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        // Метод за dropdown-ите
        private void LoadDramaLookups()
        {
            ViewBag.Genres = Enum.GetNames(typeof(Genre))
                                 .Where(g => g != nameof(Genre.Unknown))
                                 .ToList();

            ViewBag.Statuses = Enum.GetNames(typeof(DramaStatus))
                                   .Where(s => s != nameof(DramaStatus.Unknown))
                                   .ToList();
        }

        

        // ALL
        [AllowAnonymous]
        public async Task<IActionResult> All(string? searchTitle, Drama.DramaRegion? region, string sortOrder = "title_asc", int page = 1, int pageSize = 12)

        {
            var isGuest = !User.Identity?.IsAuthenticated ?? true;

            if (isGuest)
            {
                
                searchTitle = null;
                region = null;
                sortOrder = "title_asc";
                page = 1;
                pageSize = 12;
            }

            var query = _context.Dramas.Where(d => !d.IsMovie).AsQueryable();


            // SEARCH 
            if (!string.IsNullOrWhiteSpace(searchTitle))
            {
                query = query.Where(d => d.Title.Contains(searchTitle));
            }

            if (region.HasValue)
            {
                query = query.Where(d => d.Region == region.Value);
            }


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

        // DETAILS 
        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var drama = await _context.Dramas
                .Include(d => d.Reviews)
                .Include(d => d.DramaActors)
                    .ThenInclude(da => da.Actor)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (drama == null)
                return NotFound();

            bool isInMyList = false;

            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                var userId = _userManager.GetUserId(User);

                isInMyList = await _context.MyListItems
                    .AnyAsync(x => x.UserId == userId && x.DramaId == id);
            }

            var vm = new DramaDetailsViewModel
            {
                Drama = drama,
                AverageRating = drama.AverageRating,
                Reviews = drama.Reviews
                               .OrderByDescending(r => r.CreatedAt)
                               .ToList(),
                Cast = drama.DramaActors.Select(da =>
                {
                    string roleLabel = "";
                    string character = "";

                    if (!string.IsNullOrEmpty(da.RoleName))
                    {
                        var parts = da.RoleName.Split(':', 2);
                        roleLabel = parts[0].Trim();
                        if (parts.Length > 1)
                            character = parts[1].Trim();
                    }

                    return new CastItemVM
                    {
                        DramaActorId = da.Id,
                        ActorId = da.ActorId,
                        Name = da.Actor.Name,
                        ImageUrl = da.Actor.ImageUrl,
                        RoleLabel = roleLabel,
                        CharacterName = character
                    };
                }).ToList(),


                IsInMyList = isInMyList
            };

            return View(vm);
        }


        // CREATE (GET) – Admin
        [Authorize(Roles = "Admin")]
        public IActionResult Create(bool isMovie = false)
        {
            LoadDramaLookups();

            var model = new Drama
            {
                IsMovie = isMovie,
                ReleaseDate = DateTime.Today
            };

            return View(model);
        }

        // CREATE (POST) – Admin
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Drama drama)
        {

            ModelState.Remove(nameof(Drama.Year));
            drama.Year = drama.ReleaseDate.Year;

            LoadDramaLookups();

            if (!ModelState.IsValid)
            {
                return View(drama);
            }

            _context.Dramas.Add(drama);
            await _context.SaveChangesAsync();

            if (drama.IsMovie)
                return RedirectToAction("Index", "Movies");
            else
                return RedirectToAction(nameof(All));
        }


        // EDIT (GET) – Admin
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var drama = await _context.Dramas.FindAsync(id);
            if (drama == null)
                return NotFound();

            LoadDramaLookups();
            return View(drama);
        }


        // EDIT (POST) – Admin
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Drama drama)
        {
            if (id != drama.Id)
                return BadRequest();

            ModelState.Remove(nameof(Drama.Year));
            drama.Year = drama.ReleaseDate.Year;

            LoadDramaLookups();

            if (!ModelState.IsValid)
            {
                return View(drama);
            }

            _context.Entry(drama).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            if (drama.IsMovie)
                return RedirectToAction("Index", "Movies");
            else
                return RedirectToAction(nameof(All));
        }

        // DELETE (GET) – Admin
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var drama = await _context.Dramas.FindAsync(id);
            if (drama == null)
                return NotFound();

            return View(drama);
        }


        // DELETE (POST) – Admin
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var drama = await _context.Dramas.FindAsync(id);
            if (drama == null)
                return NotFound();

            bool isMovie = drama.IsMovie;

            _context.Dramas.Remove(drama);
            await _context.SaveChangesAsync();

            if (isMovie)
                return RedirectToAction("Index", "Movies");
            else
                return RedirectToAction(nameof(All));
        }
    }
}
