using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebEnterprise.Data;
using WebEnterprise.Models;
using Microsoft.AspNetCore.Identity;


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
    public async Task<IActionResult> Index()
    {
        // taking current login user ID
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

        var userList = await _db.Users.Where(x => x.Id != claim.Value).ToListAsync();
    
        // Eagerly load roles for all users
        foreach (var user in userList)
        {
            await _userManager.GetRolesAsync(user);
        }

        return View(userList);
    }
    
    [HttpGet]
    public async Task<IActionResult> Edit(string id)
    {
        var user = await _db.Users.FindAsync(id);

        // Get the current roles available in the system
        var roles = await _roleManager.Roles.ToListAsync();
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
                existingUser.Email = user.Email;
                existingUser.FullName = user.FullName;
                existingUser.Gender = user.Gender;
                existingUser.DoB = user.DoB;
                existingUser.Role = user.Role;
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


}