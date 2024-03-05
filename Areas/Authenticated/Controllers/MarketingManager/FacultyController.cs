using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebEnterprise.Data;
using WebEnterprise.Models;

namespace WebEnterprise.Areas.Authenticated.Controllers.MarketingManager
{
    [Area("Authenticated")]
    public class FacultyController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FacultyController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Dashboard()
        {
            var faculties = _context.Faculties.ToList();
            return View("~/Areas/Authenticated/Views/MarketingManager/FacultyView.cshtml", faculties);
        }

        public IActionResult CreateFaculty()
        {
            return View("~/Areas/Authenticated/Views/MarketingManager/CreateFaculty.cshtml");
        }

        [HttpPost]
        public IActionResult CreateFaculty(Faculty faculty)
        {
            if (ModelState.IsValid)
            {
                _context.Faculties.Add(faculty);
                _context.SaveChanges();
                return RedirectToAction(nameof(Dashboard));
            }
            return View("~/Areas/Authenticated/Views/MarketingManager/FacultyView.cshtml", faculty);
        }

        public async Task<IActionResult> EditFaculty(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var faculty = await _context.Faculties.FindAsync(id);
            if (faculty == null)
            {
                return NotFound();
            }
            return View("~/Areas/Authenticated/Views/MarketingManager/EditFaculty.cshtml",faculty);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditFaculty(int id, Faculty faculty)
        {
            if (id != faculty.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(faculty);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FacultyExists(faculty.Id))
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
            return View("~/Areas/Authenticated/Views/MarketingManager/EditFaculty.cshtml", faculty);
        }

        public async Task<IActionResult> DeleteFaculty(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var faculty = await _context.Faculties.FindAsync(id);
            if (faculty == null)
            {
                return NotFound();
            }

            _context.Faculties.Remove(faculty);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Dashboard));
        }

        [HttpPost, ActionName("DeleteFaculty")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var faculty = await _context.Faculties.FindAsync(id);
            _context.Faculties.Remove(faculty);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Dashboard));
        }
        private bool FacultyExists(int id)
        {
            return _context.Faculties.Any(e => e.Id == id);
        }
    }
}
