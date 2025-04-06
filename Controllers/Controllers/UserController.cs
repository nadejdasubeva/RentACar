using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RentACar.Data.Models;
using RentACar.Data.ViewModels;
using RentACar.Repositories.Interfaces;
using RentACar.Services.Interfaces;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[Authorize(Roles = "Administrator")]
public class UserController : Controller
{
    private readonly IUserRepository _userRepository;
    private readonly IUserService _userService;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly SignInManager<User> _signInManager;

    public UserController(
        IUserRepository userRepository,
        UserManager<User> userManager,
        RoleManager<IdentityRole> roleManager,
        SignInManager<User> signInManager)
    {
        _userRepository = userRepository;
        _userManager = userManager;
        _roleManager = roleManager;
        _signInManager = signInManager;
    }

    public async Task<IActionResult> Index()
    {
        var users = await _userRepository.GetAllAsync();
        var userVMs = new List<UserVM>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var userVM = new UserVM
            {
                Id = user.Id,  // Make sure to include this
                Firstname = user.Firstname,
                Surname = user.Surname,
                NIN = user.NIN,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email,
                Requests = user.Requests,
                Role = roles.FirstOrDefault()  // Add the role here
            };
            userVMs.Add(userVM);
        }

        return View(userVMs);
    }

    public async Task<IActionResult> Details(string id)
    {
        var user = await _userRepository.GetByIdAsync(id);
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
    public IActionResult Create()
    {
        return View(new CreateUserVM());
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateUserVM vm)
    {
        if (await _userRepository.AnyAsync(u => u.NIN == vm.NIN))
        {
            ModelState.AddModelError("NIN", "This NIN is already registered");
        }

        if (await _userManager.FindByEmailAsync(vm.Email) != null)
        {
            ModelState.AddModelError("Email", "Email is already in use");
        }

        if (await _userManager.FindByNameAsync(vm.UserName) != null)
        {
            ModelState.AddModelError("UserName", "Username is taken");
        }
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

            // Use the password from the view model instead of hardcoded one
            var result = await _userRepository.AddAsync(user, vm.Password);

            if (result)
            {
                // Optionally sign in the user immediately after registration
                await _signInManager.SignInAsync(user, isPersistent: false);

                return RedirectToAction("Index", "Home");
            }
        }

        // If we got this far, something failed, redisplay form
        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(string id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null) return NotFound();

        var roles = await _userManager.GetRolesAsync(user);

        var vm = new EditUserVM
        {
            Id = user.Id,
            Firstname = user.Firstname,
            Surname = user.Surname,
            PhoneNumber = user.PhoneNumber,
            Email = user.Email,
            Role = roles.FirstOrDefault()
        };

        ViewBag.Roles = new SelectList(await _roleManager.Roles.ToListAsync(), "Name", "Name");
        return View(vm);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditUserVM model)
    {
        var log = new StringBuilder();
        log.AppendLine($"--- EDIT POST ---");
        log.AppendLine($"ID: {model.Id}");
        log.AppendLine($"Name: {model.Firstname} {model.Surname}");
        log.AppendLine($"Role: {model.Role}");

        // Log all form values
        foreach (var key in Request.Form.Keys)
        {
            log.AppendLine($"{key}: {Request.Form[key]}");
        }

        System.Diagnostics.Debug.WriteLine(log.ToString());
        // Debug: Verify model received
        Console.WriteLine($"--- EDIT POST STARTED ---");
        Console.WriteLine($"Model ID: {model?.Id ?? "NULL"}");
        Console.WriteLine($"Name: {model?.Firstname} {model?.Surname}");
        Console.WriteLine($"Role: {model?.Role}");
        Console.WriteLine($"Phone: {model?.PhoneNumber}");

        if (!ModelState.IsValid)
        {
            Console.WriteLine("❌ ModelState Invalid. Errors:");
            foreach (var error in ModelState.Where(e => e.Value.Errors.Any()))
            {
                Console.WriteLine($"{error.Key}: {string.Join(", ", error.Value.Errors.Select(e => e.ErrorMessage))}");
            }
            ViewBag.Roles = await GetRolesSelectList();
            return View(model);
        }

        try
        {
            // Get user with tracking
            var user = await _userManager.Users
                .Include(u => u.Requests)
                .FirstOrDefaultAsync(u => u.Id == model.Id);

            if (user == null)
            {
                Console.WriteLine("❌ USER NOT FOUND IN DATABASE");
                return NotFound();
            }

            // Debug before/after values
            Console.WriteLine($"🔄 Updating user {user.Id}");
            Console.WriteLine($"FROM: {user.Firstname} {user.Surname} | {user.NIN} | {user.PhoneNumber}");
            Console.WriteLine($"TO: {model.Firstname} {model.Surname} | {model.PhoneNumber}");

            // Update properties
            user.Firstname = model.Firstname;
            user.Surname = model.Surname;
            user.PhoneNumber = model.PhoneNumber;

            // Save changes
            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                Console.WriteLine("❌ UPDATE FAILED. Errors:");
                foreach (var error in updateResult.Errors)
                {
                    Console.WriteLine($"{error.Code}: {error.Description}");
                }

                ModelState.AddModelError("", "Failed to update user");
                ViewBag.Roles = await GetRolesSelectList();
                return View(model);
            }

            // Handle role change
            var currentRoles = await _userManager.GetRolesAsync(user);
            var currentRole = currentRoles.FirstOrDefault();

            if (model.Role != currentRole)
            {
                Console.WriteLine($"🔄 CHANGING ROLE FROM {currentRole} TO {model.Role}");

                var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
                if (!removeResult.Succeeded)
                {
                    Console.WriteLine("❌ FAILED TO REMOVE ROLES");
                    foreach (var error in removeResult.Errors)
                    {
                        Console.WriteLine($"{error.Code}: {error.Description}");
                    }
                }

                var addResult = await _userManager.AddToRoleAsync(user, model.Role);
                if (!addResult.Succeeded)
                {
                    Console.WriteLine("❌ FAILED TO ADD ROLE");
                    foreach (var error in addResult.Errors)
                    {
                        Console.WriteLine($"{error.Code}: {error.Description}");
                    }
                }
            }

            Console.WriteLine("✅ UPDATE SUCCESSFUL");
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"‼️ EXCEPTION: {ex.Message}");
            Console.WriteLine($"STACK TRACE: {ex.StackTrace}");

            ModelState.AddModelError("", "An unexpected error occurred");
            ViewBag.Roles = await GetRolesSelectList();
            return View(model);
        }
    }

    private async Task<SelectList> GetRolesSelectList()
    {
        try
        {
            var roles = await _roleManager.Roles
                .AsNoTracking()
                .OrderBy(r => r.Name)
                .ToListAsync();

            Console.WriteLine($"Loaded {roles.Count} roles for dropdown");
            return new SelectList(roles, "Name", "Name");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ ERROR LOADING ROLES: {ex.Message}");
            return new SelectList(Enumerable.Empty<SelectListItem>());
        }
    }
    //[HttpPost]
    //[ValidateAntiForgeryToken]
    //public async Task<IActionResult> Edit(EditUserVM model)
    //{
    //    if (!ModelState.IsValid)
    //    {
    //        ViewBag.Roles = new SelectList(await _roleManager.Roles.ToListAsync(), "Name", "Name");
    //        return View(model);
    //    }
    //
    //    var user = await _userRepository.GetByIdAsync(model.Id);
    //    if (user == null) return NotFound();
    //
    //    // Update basic info
    //    user.Firstname = model.Firstname;
    //    user.Surname = model.Surname;
    //    user.NIN = model.NIN;
    //    user.PhoneNumber = model.PhoneNumber;
    //
    //    // Update using repository
    //    var updateResult = await _userRepository.UpdateAsync(user);
    //
    //    if (!updateResult)
    //    {
    //        ModelState.AddModelError(string.Empty, "Failed to update user");
    //        ViewBag.Roles = new SelectList(await _roleManager.Roles.ToListAsync(), "Name", "Name");
    //        return View(model);
    //    }
    //
    //    // Handle role change through service
    //    var currentRole = (await _userManager.GetRolesAsync(user)).FirstOrDefault();
    //    if (model.Role != currentRole)
    //    {
    //        await _userService.ChangeRoleAsync(user.Id);
    //    }
    //
    //    return RedirectToAction(nameof(Index));
    //}

    [HttpPost]
    public async Task<IActionResult> ToggleRole(string id)
    {
        var result = await _userService.ChangeRoleAsync(id);
        if (!result)
        {
            return NotFound();
        }
        return RedirectToAction(nameof(Edit), new { id });
    }

    public async Task<IActionResult> Delete(string id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null) return NotFound();

        var vm = new UserVM
        {
            Firstname = user.Firstname,
            Surname = user.Surname,
            Email = user.Email,
            Role = (await _userManager.GetRolesAsync(user)).FirstOrDefault()
        };

        return View(vm);
    }

    [HttpPost, ActionName("DeleteUser")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteUser(string id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null) return NotFound();

        var result = await _userRepository.DeleteAsync(user);
        if (result)
        {
            TempData["SuccessMessage"] = "User deleted successfully";
            return RedirectToAction(nameof(Index));
        }

        TempData["ErrorMessage"] = "Failed to delete user";
        return View(await _userRepository.GetByIdAsync(id));
    }
}