using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RentACar.Data.Models;
using RentACar.Data.ViewModels;
using System.Linq;
using System.Threading.Tasks;

[Authorize(Roles = "Admin")]
public class UserController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public UserController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<IActionResult> GetAllUsers()
    {
        var users = _userManager.Users.ToList();
        var userVMs = users.Select(u => new UserVM
        {
            Firstname = u.Firstname,
            Surname = u.Surname,
            NIN = u.NIN,
            PhoneNumber = u.PhoneNumber,
            Email = u.Email,
            Requests = u.Requests
        }).ToList();

        ViewBag.UserRoles = new Dictionary<string, string>();
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            ViewBag.UserRoles[user.Email] = roles.FirstOrDefault();
        }

        return View(userVMs);
    }

    public async Task<IActionResult> GetUserById(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null) return NotFound();

        var vm = new UserVM
        {
            Firstname = user.Firstname,
            Surname = user.Surname,
            NIN = user.NIN,
            PhoneNumber = user.PhoneNumber,
            Email = user.Email,
            Requests = user.Requests,
            Role = (await _userManager.GetRolesAsync(user)).FirstOrDefault()
        };

        return View(vm);
    }

    [AllowAnonymous]
    public IActionResult CreateUser()
    {
        return View(new UserVM());
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> CreateUser(UserVM vm)
    {
        if (ModelState.IsValid)
        {
            var user = new User
            {
                UserName = vm.Email,
                Email = vm.Email,
                Firstname = vm.Firstname,
                Surname = vm.Surname,
                NIN = vm.NIN,
                PhoneNumber = vm.PhoneNumber
            };

            var result = await _userManager.CreateAsync(user, "DefaultPassword123!"); // In production, add password field to UserVM
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "BasicUser");
                return RedirectToAction("Index", "Home");
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
        return View(vm);
    }

    public async Task<IActionResult> EditUser(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null) return NotFound();

        var vm = new UserVM
        {
            Firstname = user.Firstname,
            Surname = user.Surname,
            NIN = user.NIN,
            PhoneNumber = user.PhoneNumber,
            Email = user.Email,
            Role = (await _userManager.GetRolesAsync(user)).FirstOrDefault()
        };

        ViewBag.AvailableRoles = new SelectList(_roleManager.Roles, "Name", "Name");
        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> EditUser(UserVM vm)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByEmailAsync(vm.Email);
            if (user == null) return NotFound();

            user.Firstname = vm.Firstname;
            user.Surname = vm.Surname;
            user.NIN = vm.NIN;
            user.PhoneNumber = vm.PhoneNumber;

            await _userManager.UpdateAsync(user);
            return RedirectToAction(nameof(GetAllUsers));
        }
        ViewBag.AvailableRoles = new SelectList(_roleManager.Roles, "Name", "Name");
        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> ToggleRole(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null) return NotFound();

        var currentRole = (await _userManager.GetRolesAsync(user)).FirstOrDefault();
        var newRole = currentRole == "Admin" ? "BasicUser" : "Admin";

        await _userManager.RemoveFromRoleAsync(user, currentRole);
        await _userManager.AddToRoleAsync(user, newRole);

        return RedirectToAction(nameof(EditUser), new { email });
    }
}