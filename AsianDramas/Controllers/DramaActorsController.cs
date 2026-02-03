using System.Linq;
using System.Threading.Tasks;
using AsianDramas.Data;
using AsianDramas.Models;
using AsianDramas.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AsianDramas.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DramaActorsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DramaActorsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ADD GET 
        [HttpGet]
        public async Task<IActionResult> Add(int dramaId)
        {
            var drama = await _context.Dramas.FindAsync(dramaId);
            if (drama == null) return NotFound();

            var vm = new DramaActorAddViewModel
            {
                DramaId = dramaId,
                ActorOptions = await _context.Actors
                    .OrderBy(a => a.Name)
                    .Select(a => new SelectListItem
                    {
                        Value = a.Id.ToString(),
                        Text = a.Name
                    })
                    .ToListAsync()
            };

            ViewBag.DramaTitle = drama.Title;
            return View(vm);
        }

        //  ADD POST 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(DramaActorAddViewModel model)
        {
            var drama = await _context.Dramas
                .Include(d => d.DramaActors)
                .FirstOrDefaultAsync(d => d.Id == model.DramaId);

            if (drama == null) return NotFound();

            if (!ModelState.IsValid)
            {
                model.ActorOptions = await _context.Actors
                    .OrderBy(a => a.Name)
                    .Select(a => new SelectListItem
                    {
                        Value = a.Id.ToString(),
                        Text = a.Name
                    })
                    .ToListAsync();

                ViewBag.DramaTitle = drama.Title;
                return View(model);
            }

            bool exists = await _context.DramaActors
                .AnyAsync(da => da.DramaId == model.DramaId && da.ActorId == model.ActorId);

            if (!exists)
            {
                var roleName = $"{model.RoleType}: {model.CharacterName}";

                var da = new DramaActor
                {
                    DramaId = model.DramaId,
                    ActorId = model.ActorId,
                    RoleName = roleName,
                    IsMainRole = model.RoleType == "Main role"
                };

                _context.DramaActors.Add(da);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Details", "Dramas", new { id = model.DramaId });
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int dramaId, int actorId)
        {
            var da = await _context.DramaActors
                .Include(x => x.Drama)
                .Include(x => x.Actor)
                .FirstOrDefaultAsync(x => x.DramaId == dramaId && x.ActorId == actorId);

            if (da == null) return NotFound();


            var roleText = da.RoleName ?? "";
            string roleType = "";
            string character = "";

            if (!string.IsNullOrWhiteSpace(roleText))
            {
                var parts = roleText.Split(':', 2);
                roleType = parts[0].Trim();
                if (parts.Length > 1)
                {
                    character = parts[1].Trim();
                }
            }

            var vm = new DramaActorEditViewModel
            {
                DramaId = da.DramaId,
                ActorId = da.ActorId,
                OriginalActorId = da.ActorId,
                DramaTitle = da.Drama.Title,
                RoleType = string.IsNullOrEmpty(roleType) ? "Main role" : roleType,
                CharacterName = character
            };


            vm.ActorOptions = await _context.Actors
                .OrderBy(a => a.Name)
                .Select(a => new SelectListItem
                {
                    Value = a.Id.ToString(),
                    Text = a.Name,
                    Selected = (a.Id == vm.ActorId)
                })
                .ToListAsync();

            return View(vm);
        }

        // EDIT POST 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(DramaActorEditViewModel model)
        {

            var existing = await _context.DramaActors
                .FirstOrDefaultAsync(x => x.DramaId == model.DramaId &&
                                          x.ActorId == model.OriginalActorId);

            if (existing == null) return NotFound();


            string newRoleName = $"{model.RoleType}: {model.CharacterName}".TrimEnd(':', ' ');


            if (model.ActorId != model.OriginalActorId)
            {
                bool already = await _context.DramaActors
                    .AnyAsync(x => x.DramaId == model.DramaId && x.ActorId == model.ActorId);

                if (!already)
                {
                    _context.DramaActors.Remove(existing);

                    var newDa = new DramaActor
                    {
                        DramaId = model.DramaId,
                        ActorId = model.ActorId,
                        RoleName = newRoleName,
                        IsMainRole = model.RoleType == "Main role"
                    };

                    _context.DramaActors.Add(newDa);
                }

            }
            else
            {

                existing.RoleName = newRoleName;
                existing.IsMainRole = model.RoleType == "Main role";
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "Dramas", new { id = model.DramaId });
        }


        // DELETE GET 
        [HttpGet]
        public async Task<IActionResult> Delete(int dramaId, int actorId)
        {
            var da = await _context.DramaActors
                .Include(x => x.Actor)
                .Include(x => x.Drama)
                .FirstOrDefaultAsync(x => x.DramaId == dramaId && x.ActorId == actorId);

            if (da == null) return NotFound();

            string roleType = "";
            string character = "";
            if (!string.IsNullOrWhiteSpace(da.RoleName))
            {
                var parts = da.RoleName.Split(':', 2);
                roleType = parts[0].Trim();
                if (parts.Length > 1)
                    character = parts[1].Trim();
            }

            var vm = new DramaActorEditViewModel
            {
                DramaId = dramaId,
                ActorId = actorId,
                ActorName = da.Actor?.Name,
                RoleType = roleType,
                CharacterName = character
            };

            ViewBag.DramaTitle = da.Drama?.Title;
            return View(vm);  
        }

        //  DELETE POST 
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int dramaId, int actorId)
        {
            var da = await _context.DramaActors
                .FirstOrDefaultAsync(x => x.DramaId == dramaId && x.ActorId == actorId);

            if (da != null)
            {
                _context.DramaActors.Remove(da);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Details", "Dramas", new { id = dramaId });
        }
    }
}
