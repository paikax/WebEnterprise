using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebEnterprise.Data;
using WebEnterprise.Models;
using Microsoft.AspNetCore.Identity;
using WebEnterprise.ViewModels;


namespace WebEnterprise.Areas.Authenticated.Controllers;

[Area(Constants.Areas.AuthenticatedArea)]
[Authorize(Roles = Constants.Roles.AdminRole)]
public class UsersController : Microsoft.AspNetCore.Mvc.Controller
{
    private readonly ApplicationDbContext _db;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<IdentityUser> _userManager;

    public UsersController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager,
        ApplicationDbContext db)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _db = db;
    }
    
    [HttpGet]
    public async Task<IActionResult> Index(string roleFilter)
    {
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

        IQueryable<User> userList = _db.Users.Where(x => x.Id != claim.Value);

        if (!string.IsNullOrEmpty(roleFilter))
        {
            userList = userList.Where(u => u.Role == roleFilter);
        }

        var filteredUserList = await userList.ToListAsync();

        foreach (var user in filteredUserList)
        {
            await _userManager.GetRolesAsync(user);
        }

        ViewBag.SelectedRole = roleFilter; // Pass selected role to view

        return View(filteredUserList);
    }

    // [HttpGet]
    // public IActionResult Create()
    // {
    //     var newUser = new User();
    //
    //     var roles = _roleManager.Roles.Where(r => r.Name != Constants.Roles.AdminRole).ToList();
    //     ViewBag.Roles = roles;
    //
    //     return View(newUser);
    // }
    //
    // [HttpPost]
    // public async Task<IActionResult> Create(User user)
    // {
    //     if (ModelState.IsValid)
    //     {
    //         // Hash the user's password before creating the user
    //         string hashedPassword = _userManager.PasswordHasher.HashPassword(user, user.Password);
    //
    //         // Set the hashed password
    //         user.PasswordHash = hashedPassword;
    //         user.UserName = user.Email;
    //         user.EmailConfirmed = true;
    //
    //         // Create the user
    //         var result = await _userManager.CreateAsync(user);
    //
    //         if (result.Succeeded)
    //         {
    //             // Add user to selected role
    //             var role = await _roleManager.FindByNameAsync(user.Role);
    //             if (role != null)
    //             {
    //                 await _userManager.AddToRoleAsync(user, role.Name);
    //             }
    //
    //             return RedirectToAction(nameof(Index));
    //         }
    //         else
    //         {
    //             foreach (var error in result.Errors)
    //             {
    //                 ModelState.AddModelError(string.Empty, error.Description);
    //             }
    //         }
    //     }
    //
    //     // If we reach this point, something went wrong. Re-render the form with errors.
    //     // Also, re-populate the roles dropdown
    //     ViewBag.Roles = _roleManager.Roles.ToList();
    //     return View(user);
    // }
    
    [HttpGet]
    public async Task<IActionResult> Edit(string id)
    {
        var user = await _db.Users.FindAsync(id);

        // Get the current roles available in the system, excluding the Admin role
        var roles = _roleManager.Roles.Where(r => r.Name != Constants.Roles.AdminRole).ToList();
        ViewBag.Roles = roles;

        // Get the current role of the user
        var userRoles = await _userManager.GetRolesAsync(user);
        ViewBag.CurrentRole = userRoles.FirstOrDefault();

        return View(user);
    }
    
    [HttpPost]
    public async Task<IActionResult> Edit(User user)
    {
        if (ModelState.IsValid)
        {
            try
            {
                // Retrieve existing user from the database
                var existingUser = await _db.Users.FindAsync(user.Id);

                // Update editable properties
                if (existingUser != null)
                {
                    existingUser.Email = user.Email;
                    existingUser.FullName = user.FullName;
                    existingUser.Gender = user.Gender;
                    existingUser.DoB = user.DoB;
                    existingUser.Role = user.Role;
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(user.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            
            await _db.SaveChangesAsync();
            
            return RedirectToAction(nameof(Index));
        }
        
        if (!ModelState.IsValid)
        {
            foreach (var modelStateKey in ModelState.Keys)
            {
                var modelStateVal = ModelState[modelStateKey];
                foreach (var error in modelStateVal.Errors)
                {
                    // Log or debug the error messages
                    Console.WriteLine($"Key: {modelStateKey}, Error: {error.ErrorMessage}");
                }
            }
            // Return some feedback to the user or handle the error appropriately
        }
        return View(user);
    }
    
    private bool UserExists(string id)
    {
        return _db.Users.Any(e => e.Id == id);
    }

    [HttpGet]
    public async Task<IActionResult> Details(string id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var user = await _db.Users.FindAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        return View(user);
    }
    
    
    
    [HttpGet]
    public async Task<IActionResult> Delete(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();
        if (User.IsInRole(Constants.Roles.AdminRole))
        {
            var roleTemp = await _userManager.GetRolesAsync(user);
            var role = roleTemp.FirstOrDefault();
            if (role == Constants.Roles.StudentRole) return RedirectToAction(nameof(Index));
        }

        await _userManager.DeleteAsync(user);

        return RedirectToAction(nameof(Index));
    }
    
    [HttpGet]
    public async Task<IActionResult> ConfirmEmail(string id)
    {
        var user = _db.Users.Find(id);

        if (user == null) return View();

        var emailConfirmation = new EmailConfirmation()
        {
            Email = user.Email
        };

        return View(emailConfirmation);
    }
    
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ConfirmEmail(EmailConfirmation emailConfirmation)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByEmailAsync(emailConfirmation.Email);
            if (user != null)
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                return RedirectToAction("ResetPassword", "Users"
                    , new { token, email = user.Email });
            }
        }

        return View(emailConfirmation);
    }

    [HttpGet]
    [Authorize(Roles = Constants.Roles.AdminRole)]
    public async Task<IActionResult> ResetPassword(string token, string email)
    {
        if (token == null || email == null) ModelState.AddModelError("", "Invalid password reset token");

        var resetPasswordViewModel = new ResetPasswordViewModel()
        {
            Email = email,
            Token = token
        };

        return View(resetPasswordViewModel);
    }

    [HttpPost]
    [Authorize(Roles = Constants.Roles.AdminRole)]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel resetPasswordViewModel)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByEmailAsync(resetPasswordViewModel.Email);
            if (user != null)
            {
                var result = await _userManager.ResetPasswordAsync(user, resetPasswordViewModel.Token,
                    resetPasswordViewModel.Password);
                if (result.Succeeded) return RedirectToAction(nameof(Index));
            }
        }

        return View(resetPasswordViewModel);
    }

}