using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebEnterprise.Data;
using WebEnterprise.Models;

namespace WebEnterprise.Areas.Authenticated.Controllers
{
    [Area(Constants.Areas.AuthenticatedArea)]
    [Authorize(Roles = Constants.Roles.UniversityMarketingManagerRole)]
    public class FacultiesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FacultiesController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var faculties = _context.Faculties.ToList();
            return View(faculties);
        }

        public IActionResult CreateFaculty()
        {
            var faculty = new Faculty();
            return View(faculty);
        }

        [HttpPost]
        public IActionResult CreateFaculty(Faculty faculty)
        {
            if (ModelState.IsValid)
            {
                _context.Faculties.Add(faculty);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View("CreateFaculty", faculty);
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
            return View("EditFaculty",faculty);
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
                return RedirectToAction(nameof(Index));
            }
            return View("EditFaculty", faculty);
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

            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ActionName("DeleteFaculty")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var faculty = await _context.Faculties.FindAsync(id);
            _context.Faculties.Remove(faculty);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        private bool FacultyExists(int id)
        {
            return _context.Faculties.Any(e => e.Id == id);
        }
    }
}
