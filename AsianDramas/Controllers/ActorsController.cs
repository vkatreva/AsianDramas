using AsianDramas.Data;
using AsianDramas.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace AsianDramas.Controllers
{
    public class ActorsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const int PageSize = 10;

        public ActorsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // LIST + SEARCH + SORT + PAGING
        [AllowAnonymous]
        public async Task<IActionResult> Index(string name, string country, string sortOrder, int page = 1)
        {
            var query = _context.Actors.AsQueryable();

            // FILTERING
            if (!string.IsNullOrWhiteSpace(name))
                query = query.Where(a => a.Name.Contains(name));

            if (!string.IsNullOrWhiteSpace(country))
                query = query.Where(a => a.Nationality.Contains(country));

            // SORTING
            query = sortOrder switch
            {
                "name_desc" => query.OrderByDescending(a => a.Name),
                "birth" => query.OrderBy(a => a.BirthDate),
                "birth_desc" => query.OrderByDescending(a => a.BirthDate),
                _ => query.OrderBy(a => a.Name)
            };

            // PAGINATION
            var totalItems = await query.CountAsync();
            var actors = await query
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            ViewBag.NameFilter = name;
            ViewBag.CountryFilter = country;
            ViewBag.SortOrder = sortOrder;
            ViewBag.Page = page;
            ViewBag.PageSize = PageSize;
            ViewBag.TotalItems = totalItems;

            return View(actors);
        }

        // DETAILS
        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var actor = await _context.Actors
                .Include(a => a.DramaActors)
                    .ThenInclude(da => da.Drama)
                .Include(a => a.Reviews)              
                    .ThenInclude(r => r.User)         
                .FirstOrDefaultAsync(a => a.Id == id);

            if (actor == null)
                return NotFound();

            return View(actor);
        }


        // CREATE
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(Actor actor)
        {
            if (ModelState.IsValid)
            {

                actor.PopularityScore = 0;

                _context.Add(actor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                Console.WriteLine(error.ErrorMessage);
            }


            return View(actor);
        }

        // EDIT
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var actor = await _context.Actors.FindAsync(id);
            if (actor == null)
                return NotFound();

            return View(actor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, Actor actor)
        {
            if (id != actor.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(actor);

            var dbActor = await _context.Actors.FindAsync(id);
            if (dbActor == null)
                return NotFound();

            
            dbActor.Name = actor.Name;
            dbActor.Biography = actor.Biography;
            dbActor.BirthDate = actor.BirthDate;
            dbActor.Nationality = actor.Nationality;
            dbActor.ImageUrl = actor.ImageUrl;

            

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        // DELETE
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var actor = await _context.Actors
                .FirstOrDefaultAsync(a => a.Id == id);

            if (actor == null)
                return NotFound();

            return View(actor);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var actor = await _context.Actors.FindAsync(id);
            if (actor == null)
                return NotFound();

            _context.Actors.Remove(actor);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


    }
}
