using AsianDramas.Data;
using AsianDramas.Models;
using AsianDramas.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AsianDramas.Controllers
{
    [Authorize] 
    public class ReviewsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public ReviewsController(ApplicationDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // LIST ПО ДРАМА
        [AllowAnonymous]
        public async Task<IActionResult> Index(string? reviewer, int? minRating, string sortOrder = "date_desc", int page = 1, int pageSize = 10)
        {

            var isGuest = !User.Identity?.IsAuthenticated ?? true;

            if (isGuest)
            {
                reviewer = null;
                minRating = null;
                sortOrder = "date_desc";
                page = 1;
                pageSize = 10;
            }

            var query = _context.Reviews.Include(r => r.Drama).AsQueryable();


            // SEARCH (2 criteria)
            if (!string.IsNullOrWhiteSpace(reviewer))
                query = query.Where(r => r.ReviewerName.Contains(reviewer));

            if (minRating.HasValue)
                query = query.Where(r => r.Rating >= minRating.Value);

            // SORT
            query = sortOrder switch
            {
                "date_asc" => query.OrderBy(r => r.CreatedAt),
                "rating_desc" => query.OrderByDescending(r => r.Rating),
                "rating_asc" => query.OrderBy(r => r.Rating),
                _ => query.OrderByDescending(r => r.CreatedAt)
            };

            // PAGING
            var totalItems = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var vm = new ReviewIndexViewModel
            {
                Items = items,
                Reviewer = reviewer,
                MinRating = minRating,
                SortOrder = sortOrder,
                Page = page,
                PageSize = pageSize,
                TotalItems = totalItems
            };

            return View(vm);
        }


        // CREATE (GET) 
        [HttpGet]
        public async Task<IActionResult> Create(int dramaId)
        {
            var drama = await _context.Dramas.FindAsync(dramaId);
            if (drama == null) return NotFound();

            ViewBag.DramaTitle = drama.Title;

            var vm = new ReviewCreateViewModel
            {
                DramaId = dramaId
            };

            return View(vm);
        }

        // CREATE (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ReviewCreateViewModel model)
        {
            var drama = await _context.Dramas.FirstOrDefaultAsync(d => d.Id == model.DramaId);


            if (drama == null) return NotFound();

            if (!ModelState.IsValid)
            {
                ViewBag.DramaTitle = drama.Title;
                return View(model);
            }

            var review = new Review
            {
                DramaId = model.DramaId,
                Rating = model.Rating,
                Comment = model.Comment,
                ReviewerName = User.FindFirstValue(ClaimTypes.Email) ?? User.Identity?.Name ?? "Anonymous",
                CreatedAt = DateTime.UtcNow
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            await RecalculateDramaAverageRatingAsync(model.DramaId);

            return RedirectToAction("Details", "Dramas", new { id = model.DramaId });
        }


        // EDIT (GET) 
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review == null) return NotFound();

            var myName = User.Identity?.Name;
            var myEmail = User.FindFirstValue(ClaimTypes.Email);

            if (!(string.Equals(review.ReviewerName, myName, StringComparison.OrdinalIgnoreCase) ||
                  string.Equals(review.ReviewerName, myEmail, StringComparison.OrdinalIgnoreCase)))
            {
                return Forbid();
            }


            var vm = new AsianDramas.Models.ViewModels.ReviewEditViewModel
            {
                Id = review.Id,
                DramaId = review.DramaId,
                Rating = review.Rating,
                Comment = review.Comment
            };

            return View(vm);
        }

        // EDIT (POST) 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AsianDramas.Models.ViewModels.ReviewEditViewModel model)
        {
            var review = await _context.Reviews
                .Include(r => r.Drama)
                .FirstOrDefaultAsync(r => r.Id == model.Id);

            if (review == null) return NotFound();

            var myName = User.Identity?.Name;
            var myEmail = User.FindFirstValue(ClaimTypes.Email);

            if (!(string.Equals(review.ReviewerName, myName, StringComparison.OrdinalIgnoreCase) ||
                  string.Equals(review.ReviewerName, myEmail, StringComparison.OrdinalIgnoreCase)))
            {
                return Forbid();
            }


            if (!ModelState.IsValid)
                return View(model);

            
            review.Rating = model.Rating;
            review.Comment = model.Comment;
            review.CreatedAt = DateTime.UtcNow; 

            await _context.SaveChangesAsync();


            await RecalculateDramaAverageRatingAsync(review.DramaId);

            return RedirectToAction("Details", "Dramas", new { id = model.DramaId });

        }


        // DELETE (GET – потвърждение) 
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var review = await _context.Reviews
                .Include(r => r.Drama)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (review == null) return NotFound();

            var myName = User.Identity?.Name;
            var myEmail = User.FindFirstValue(ClaimTypes.Email);

            if (!(string.Equals(review.ReviewerName, myName, StringComparison.OrdinalIgnoreCase) ||
                  string.Equals(review.ReviewerName, myEmail, StringComparison.OrdinalIgnoreCase)))
            {
                return Forbid();
            }


            return View(review);  
        }

        // DELETE (POST – реално триене) 
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var review = await _context.Reviews
                .Include(r => r.Drama)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (review == null) return NotFound();


            var myName = User.Identity?.Name;
            var myEmail = User.FindFirstValue(ClaimTypes.Email);

            if (!(string.Equals(review.ReviewerName, myName, StringComparison.OrdinalIgnoreCase) ||
                  string.Equals(review.ReviewerName, myEmail, StringComparison.OrdinalIgnoreCase)))
            {
                return Forbid();
            }

            var dramaId = review.DramaId;
            

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();

            await RecalculateDramaAverageRatingAsync(dramaId);

            return RedirectToAction("Details", "Dramas", new { id = dramaId });
        }

        private async Task RecalculateDramaAverageRatingAsync(int dramaId)
        {
            var avg = await _context.Reviews
                .Where(r => r.DramaId == dramaId)
                .Select(r => (decimal?)r.Rating)
                .AverageAsync();

            var drama = await _context.Dramas.FindAsync(dramaId);
            if (drama == null) return;

            drama.AverageRating = Math.Round(avg ?? 0m, 2);
            await _context.SaveChangesAsync();
        }

    }
}
