using Microsoft.AspNetCore.Mvc;
using WebEnterprise.Data;
using WebEnterprise.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebEnterprise.Areas.Authenticated.Controllers.MarketingManager
{
    [Area("Authenticated")]
    public class ClosureController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ClosureController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Dashboard()
        {
            var closures = await _context.ClosureDates
                .Include(c => c.Faculty) // Include the Faculty navigation property
                .ToListAsync();

            return View("~/Areas/Authenticated/Views/MarketingManager/ClosureView.cshtml", closures);
        }

        //public async Task<IActionResult> Dashboard(int facultyId)
        //{
        //    // Retrieve the faculty by ID
        //    var faculty = await _context.Faculties.FindAsync(facultyId);

        //    // Check if the faculty exists
        //    if (faculty == null)
        //    {
        //        // Handle the case where the faculty is not found
        //        return NotFound();
        //    }

        //    // Retrieve closures for the given faculty
        //    var closures = await _context.ClosureDates
        //        .Where(c => c.FacultyId == facultyId)
        //        .ToListAsync();

        //    // Pass closures and faculty to the view
        //    ViewBag.Faculty = faculty;
        //    return View("~/Areas/Authenticated/Views/MarketingManager/ClosureView.cshtml", closures);
        //}


        public IActionResult CreateClosure()
        {
            // Retrieve faculties for dropdown list
            var faculties = _context.Faculties.ToList();

            // Pass faculties to the view
            ViewBag.Faculties = new SelectList(faculties, "Id", "RoleName");

            return View("~/Areas/Authenticated/Views/MarketingManager/CreateClosure.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> CreateClosure(ContributionClosureDate closure)
        {
                closure.Faculty = await _context.Faculties.FindAsync(closure.FacultyId);
                _context.ClosureDates.Add(closure);
                await _context.SaveChangesAsync();
                return RedirectToAction("Dashboard");

        }

        public async Task<IActionResult> EditClosure(int? id)
        {
            var closure = await _context.ClosureDates.FindAsync(id);
            if (closure == null)
            {
                return NotFound();
            }

            ViewBag.Faculties = new SelectList(_context.Faculties, "Id", "RoleName");

            return View("~/Areas/Authenticated/Views/MarketingManager/EditClosure.cshtml", closure);
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
                return RedirectToAction(nameof(Dashboard));
        }


        //public async Task<IActionResult> DeleteClosure(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var closure = await _context.ClosureDates.FindAsync(id);
        //    if (closure == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(closure);
        //}

        [HttpPost, ActionName("DeleteClosure")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteClosureConfirmed(int id)
        {
            var closure = await _context.ClosureDates.FindAsync(id);
            _context.ClosureDates.Remove(closure);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Dashboard));
        }

        private bool ClosureExists(int id)
        {
            return _context.ClosureDates.Any(e => e.Id == id);
        }

    }
}