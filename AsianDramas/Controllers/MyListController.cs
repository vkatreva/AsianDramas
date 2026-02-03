using AsianDramas.Data;
using AsianDramas.Models;
using AsianDramas.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace AsianDramas.Controllers   
{
    [Authorize] 
    public class MyListController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public MyListController(ApplicationDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: /MyList
        [Authorize]
        public async Task<IActionResult> Index(string? searchTitle, MyListStatus? status, string sortOrder = "added_desc", int page = 1, int pageSize = 12)

        {
            var userId = _userManager.GetUserId(User);

            var query = _context.MyListItems
                .Include(x => x.Drama)
                .Where(x => x.UserId == userId);

            // SEARCH (2 criteria)
            if (!string.IsNullOrWhiteSpace(searchTitle))
                query = query.Where(x => x.Drama.Title.Contains(searchTitle));

            if (status.HasValue)
                query = query.Where(x => x.Status == status.Value);


            // SORT
            query = sortOrder switch
            {
                "title_asc" => query.OrderBy(x => x.Drama.Title),
                "title_desc" => query.OrderByDescending(x => x.Drama.Title),
                "added_asc" => query.OrderBy(x => x.AddedAt),
                _ => query.OrderByDescending(x => x.AddedAt)
            };

            // PAGING
            var totalItems = await query.CountAsync();

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(i => new MovieVm
                {
                    Id = i.Drama.Id,
                    Title = i.Drama.Title,
                    PosterUrl = string.IsNullOrEmpty(i.Drama.PosterUrl) ? "/images/fallback-poster.jpg" : i.Drama.PosterUrl,
                    Region = i.Drama.Region.ToString(),
                    Year = i.Drama.Year,
                    MyListItemId = i.Id,
                    MyListStatus = i.Status,
                    MyListNote = i.Note
                })
                .ToListAsync();

            var vm = new MyListIndexViewModel
            {
                Items = items,
                SearchTitle = searchTitle,
                Status = status?.ToString(),
                SortOrder = sortOrder,
                Page = page,
                PageSize = pageSize,
                TotalItems = totalItems
            };

            return View(vm);
        }


        // POST: /MyList/Add
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(int dramaId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }

            bool exists = await _context.MyListItems
                .AnyAsync(x => x.UserId == user.Id && x.DramaId == dramaId);

            if (!exists)
            {
                var item = new MyListItem
                {
                    UserId = user.Id,
                    DramaId = dramaId
                };
                _context.MyListItems.Add(item);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Details", "Dramas", new { id = dramaId });
        }

        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            var userId = _userManager.GetUserId(User);

            var item = await _context.MyListItems
                .Include(x => x.Drama)
                .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);

            if (item == null)
                return NotFound();

            var vm = new MyListItemEditViewModel
            {
                Id = item.Id,
                Status = item.Status,
                Note = item.Note,
                DramaId = item.DramaId,
                DramaTitle = item.Drama?.Title,
                PosterUrl = item.Drama?.PosterUrl
            };

            return View(vm);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(MyListItemEditViewModel model)
        {
            var userId = _userManager.GetUserId(User);

            var item = await _context.MyListItems
                .FirstOrDefaultAsync(x => x.Id == model.Id && x.UserId == userId);

            if (item == null)
                return NotFound();

            if (!ModelState.IsValid)
                return View(model);

            item.Status = model.Status;
            item.Note = model.Note;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Updated successfully.";
            return RedirectToAction(nameof(Index));

        }



        // POST: /MyList/Remove
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(int dramaId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var item = await _context.MyListItems
                .FirstOrDefaultAsync(x => x.UserId == user.Id && x.DramaId == dramaId);

            if (item != null)
            {
                _context.MyListItems.Remove(item);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }
    }
}
