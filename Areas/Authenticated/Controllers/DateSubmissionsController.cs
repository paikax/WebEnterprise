using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebEnterprise.Data;
using WebEnterprise.Models;

namespace WebEnterprise.Areas.Authenticated.Controllers;

[Area(Constants.Areas.AuthenticatedArea)]
[Authorize(Roles = Constants.Roles.UniversityMarketingManagerRole)]
public class DateSubmissionsController : Controller
{
    private readonly ApplicationDbContext _context;
    
    public DateSubmissionsController(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<IActionResult> Index()
    {
        var closures = await _context.ClosureDates
            .Include(c => c.Faculty) 
            .ToListAsync();

        return View("~/Areas/Authenticated/Views/DateSubmissions/Index.cshtml", closures);
    }

    public IActionResult Create()
    {
        // Retrieve all faculties
        var faculties = _context.Faculties.ToList();

        // Check if closure dates exist for any faculties
        var facultiesWithClosure = _context.ClosureDates.Select(c => c.FacultyId).ToList();

        // Filter out faculties that already have closure dates
        var availableFaculties = faculties.Where(f => !facultiesWithClosure.Contains(f.Id)).ToList();

        // Pass available faculties to the view
        ViewBag.Faculties = new SelectList(availableFaculties, "Id", "Name");

        return View("~/Areas/Authenticated/Views/DateSubmissions/Create.cshtml");
    }

    [HttpPost]
    public async Task<IActionResult> CreateClosure(ContributionClosureDate closure)
    {
        closure.Faculty = await _context.Faculties.FindAsync(closure.FacultyId);
        _context.ClosureDates.Add(closure);
        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }
    
    [HttpGet]
    public async Task<IActionResult> EditClosure(int? id)
    {
        var closure = await _context.ClosureDates.FindAsync(id);
        if (closure == null)
        {
            return NotFound();
        }

        // Retrieve faculties for dropdown list
        var faculties = await _context.Faculties.ToListAsync();

        // Pass faculties to the view
        ViewBag.Faculties = new SelectList(faculties, "Id", "Name", closure.FacultyId);

        return View("~/Areas/Authenticated/Views/DateSubmissions/EditClosure.cshtml", closure);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditClosure(int id, ContributionClosureDate closure)
    {
        if (id != closure.Id)
        {
            return NotFound();
        }
        
        try
        {
            // Retrieve the existing closure from the database
            var existingClosure = await _context.ClosureDates.FindAsync(id);
            if (existingClosure == null)
            {
                return NotFound();
            }

            // Update the properties of the existing closure with the values from the submitted form
            existingClosure.StartDate = closure.StartDate;
            existingClosure.EndDate = closure.EndDate;
            existingClosure.FacultyId = closure.FacultyId;

            // Update the closure in the database
            _context.Update(existingClosure);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ClosureExists(closure.Id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }
        return RedirectToAction(nameof(Index));
    }
    

    private bool ClosureExists(int id)
    {
        return _context.ClosureDates.Any(e => e.Id == id);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteClosure(int id)
    {
        var closure = await _context.ClosureDates.FindAsync(id);
        if (closure == null)
        {
            return NotFound();
        }

        _context.ClosureDates.Remove(closure);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }


}