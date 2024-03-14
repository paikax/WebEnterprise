using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebEnterprise.Data;
using WebEnterprise.Models;
using WebEnterprise.ViewModels;

namespace WebEnterprise.Areas.Authenticated.Controllers
{
    [Area(Constants.Areas.AuthenticatedArea)]
    [Authorize(Roles = Constants.Roles.AdminRole)]
    public class AssignmentsController : Controller
    {
        private readonly ApplicationDbContext _db;

        public AssignmentsController(ApplicationDbContext db)
        {
            _db = db;
        }
        
        [HttpGet]
        public IActionResult Index()
        {
            var assignments = _db.Assignments
                .Include(a => a.Faculty)
                .Include(a => a.Coordinator)
                .ToList();
        
            return View(assignments);
        }

        
        [HttpGet]
        public IActionResult Assign()
        {
            var viewModel = new AssignViewModel
            {
                Coordinators = _db.Users.Where(u => u.Role == Constants.Roles.CoordinatorRole).ToList(),
                Faculties = _db.Faculties.ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Assign(AssignViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var existingAssignment = _db.Assignments
                    .FirstOrDefault(a => a.FacultyId == viewModel.FacultyId && a.CoordinatorId == viewModel.SelectedCoordinatorId);

                if (existingAssignment != null)
                {
                    ModelState.AddModelError(string.Empty, "Assignment already exists for this Faculty and Coordinator.");
                }
                else
                {
                    var assignment = new Assignment
                    {
                        FacultyId = viewModel.FacultyId,
                        CoordinatorId = viewModel.SelectedCoordinatorId
                    };

                    _db.Assignments.Add(assignment);
                    _db.SaveChanges();

                    return RedirectToAction(nameof(Index));
                }
            }

            // If model state is invalid, re-render the view with validation errors
            viewModel.Coordinators = _db.Users.Where(u => u.Role == Constants.Roles.CoordinatorRole).ToList();
            viewModel.Faculties = _db.Faculties.ToList();
            return View(viewModel);
        }
    }
}
