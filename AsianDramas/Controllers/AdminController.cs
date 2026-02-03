using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using AsianDramas.Models;
using System.Threading.Tasks;
using System.Linq;

namespace AsianDramas.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(UserManager<AppUser> um, RoleManager<IdentityRole> rm)
        {
            _userManager = um;
            _roleManager = rm;
        }

        public IActionResult Index()
        {
            var users = _userManager.Users.ToList();
            return View(users);
        }

        public async Task<IActionResult> EditRoles(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var model = new EditRolesViewModel
            {
                UserId = user.Id,
                Email = user.Email,
                CurrentRoles = await _userManager.GetRolesAsync(user),
                AllRoles = _roleManager.Roles.Select(r => r.Name).ToList()
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditRoles(EditRolesViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null) return NotFound();

            var current = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, current);
            await _userManager.AddToRolesAsync(user, model.SelectedRoles ?? new string[] { });
            return RedirectToAction(nameof(Index));
        }
    }

    public class EditRolesViewModel
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public IList<string> CurrentRoles { get; set; }
        public List<string> AllRoles { get; set; }
        public string[] SelectedRoles { get; set; }
    }
}
