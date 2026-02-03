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
    public class ActorReviewsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public ActorReviewsController(ApplicationDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Create(int actorId)
        {
            var actor = await _context.Actors
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == actorId);

            if (actor == null) return NotFound();

            ViewBag.ActorName = actor.Name;

            var vm = new ActorReviewCreateViewModel
            {
                ActorId = actorId
            };

            return View(vm);
        }


        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ActorReviewCreateViewModel model)
        {
            var actor = await _context.Actors.FindAsync(model.ActorId);
            if (actor == null) return NotFound();

            ViewBag.ActorName = actor.Name;

            if (!ModelState.IsValid)
                return View(model);

            var userId = _userManager.GetUserId(User);

            var review = new ActorReview
            {
                ActorId = model.ActorId,
                UserId = userId!,
                Rating = model.Rating,
                Comment = model.Comment,
                CreatedAt = DateTime.UtcNow
            };

            _context.ActorReviews.Add(review);
            await _context.SaveChangesAsync();

            await UpdateActorPopularityAsync(model.ActorId);

            return RedirectToAction("Details", "Actors", new { id = model.ActorId });
        }


        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var review = await _context.ActorReviews
                .Include(r => r.Actor)
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (review == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            if (review.UserId != userId)
                return Forbid();


            return View(review);
        }



        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var review = await _context.ActorReviews.FindAsync(id);
            if (review == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            if (review.UserId != userId)
                return Forbid();


            int actorId = review.ActorId;

            _context.ActorReviews.Remove(review);
            await _context.SaveChangesAsync();
            await UpdateActorPopularityAsync(actorId);

            return RedirectToAction("Details", "Actors", new { id = actorId });
        }

        private async Task UpdateActorPopularityAsync(int actorId)
        {
            var stats = await _context.ActorReviews
                .Where(r => r.ActorId == actorId)
                .GroupBy(r => r.ActorId)
                .Select(g => new
                {
                    Count = g.Count(),
                    Avg = g.Average(x => x.Rating)
                })
                .FirstOrDefaultAsync();

            int popularity = 0;

            if (stats != null)
            {
                popularity = (stats.Count * 100) + (int)Math.Round(stats.Avg * 500);
                popularity = Math.Clamp(popularity, 0, 10000);
            }

            var actor = await _context.Actors.FindAsync(actorId);
            if (actor != null)
            {
                actor.PopularityScore = popularity;
                await _context.SaveChangesAsync();
            }
        }

        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            var review = await _context.ActorReviews
                .Include(r => r.Actor)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (review == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            if (review.UserId != userId)
                return Forbid();


            return View(review);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ActorReview form)
        {
            if (id != form.Id) return BadRequest();

            var review = await _context.ActorReviews.FindAsync(id);
            if (review == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            if (review.UserId != userId)
                return Forbid();

            review.Rating = form.Rating;
            review.Comment = form.Comment;

            await _context.SaveChangesAsync();
            await UpdateActorPopularityAsync(review.ActorId);

            return RedirectToAction("Details", "Actors", new { id = review.ActorId });
        }
    }
}
