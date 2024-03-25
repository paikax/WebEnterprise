using System.IO.Compression;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebEnterprise.Data;
using WebEnterprise.Models;
using WebEnterprise.ViewModels;

namespace WebEnterprise.Areas.Authenticated.Controllers;

[Area("Authenticated")]

public class ContributionsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IWebHostEnvironment _environment;
    
    public ContributionsController(ApplicationDbContext context, UserManager<IdentityUser> userManager, IWebHostEnvironment environment)
    {
        _context = context;
        _userManager = userManager;
        _environment = environment;
    }

    [Area(Constants.Areas.AuthenticatedArea)]
    [Authorize(Roles = Constants.Roles.StudentRole)]
    public async Task<IActionResult> Index()
    {
        // Retrieve the current authenticated user
        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null)
        {
            // Handle the case where the user is not found
            return NotFound();
        }

        // Retrieve contributions made by the current user
        var contributions = await _context.Contributions
            .Where(c => c.StudentId == currentUser.Id) // Filter by current user's ID
            .Include(c => c.Student)
            .Include(c => c.Faculty)
            .ToListAsync();

        // Retrieve closure dates for faculties
        var closureDates = await _context.ClosureDates
            .Include(cd => cd.Faculty)
            .ToListAsync();

        var schoolSystemData = await _context.SchoolSystemDatas.ToListAsync();

        // Create the view model
        var viewModel = new ContributionIndexViewModel
        {
            Contributions = contributions,
            ClosureDates = closureDates,
            SchoolSystemDatas = schoolSystemData,
        };

        return View("~/Areas/Authenticated/Views/Contributions/Index.cshtml", viewModel);
    }
    
    public async Task<IActionResult> CoordinatorIndex()
    {
        // Retrieve the current authenticated coordinator
        var currentCoordinator = await _userManager.GetUserAsync(User);
        if (currentCoordinator == null)
        {
            // Handle the case where the coordinator is not found
            return NotFound();
        }

        // Retrieve the assignment for the current coordinator
        var assignment = await _context.Assignments
            .FirstOrDefaultAsync(a => a.CoordinatorId == currentCoordinator.Id);

        if (assignment == null)
        {
            // Handle the case where the coordinator is not assigned to any faculty
            return NotFound("Coordinator is not assigned to any faculty.");
        }

        // Retrieve contributions from the coordinator's assigned faculty
        var contributions = await _context.Contributions
            .Where(c => c.FacultyId == assignment.FacultyId) // Filter by the coordinator's assigned faculty
            .ToListAsync();

        Console.WriteLine(contributions);

        return View("~/Areas/Authenticated/Views/Contributions/CoordinatorIndex.cshtml", contributions);
    }
    
    public async Task<IActionResult> SelectedIndex()
    {
        // Retrieve contributions with status "Approved"
        var approvedContributions = await _context.Contributions
            .Where(c => c.Status == "Approve")
            .ToListAsync();

        Console.WriteLine(approvedContributions);

        return View("~/Areas/Authenticated/Views/Contributions/SelectedIndex.cshtml",approvedContributions);
    }
    
    
    [HttpGet]
    public IActionResult Create()
    {
        // Initialize a new Contribution object
        var contribution = new Contribution();

        var faculties = _context.Faculties.ToList();

        // Pass faculties to the view
        ViewBag.Faculties = new SelectList(faculties, "Id", "Name");
        return View("~/Areas/Authenticated/Views/Contributions/CreateContribution.cshtml", contribution);
    }
    
    [Authorize(Roles = Constants.Roles.StudentRole)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Contribution contribution, IFormFile imageFile, IFormFile documentFile, bool termsAndConditionsAccepted)
    {

        try
        {
            contribution.CoordinatorComment = "Waiting for your Coordinator to comment!!";
            contribution.Status = "Pending";
            contribution.SubmissionDate = DateTime.Now;
            contribution.SelectedForPublication = false;
            contribution.TermsAndConditionsAccepted = termsAndConditionsAccepted;

            // Get the current authenticated user
            var currentUser = await _userManager.GetUserAsync(User);
            contribution.StudentId = currentUser.Id;

            // Ensure the directory for file uploads exists
            string uploadsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            if (!Directory.Exists(uploadsDirectory))
            {
                Directory.CreateDirectory(uploadsDirectory);
            }

            // Save the image file if provided
            if (imageFile != null && imageFile.Length > 0)
            {
                string imageFileName = Path.GetFileName(imageFile.FileName);
                string imageFilePath = Path.Combine(uploadsDirectory, imageFileName);
                using (var imageFileStream = new FileStream(imageFilePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(imageFileStream);
                }
                // Set the image file path in the contribution object
                contribution.ImageUrl = "~/uploads/" + imageFileName;
            }

            // Save the document file if provided
            if (documentFile != null && documentFile.Length > 0)
            {
                // Read the content of the document file into a byte array
                using (var memoryStream = new MemoryStream())
                {
                    await documentFile.CopyToAsync(memoryStream);
                    contribution.FileContent = memoryStream.ToArray();
                }

                string documentFileName = Path.GetFileName(documentFile.FileName);
                string documentFilePath = Path.Combine(uploadsDirectory, documentFileName);
                using (var documentFileStream = new FileStream(documentFilePath, FileMode.Create))
                {
                    await documentFile.CopyToAsync(documentFileStream);
                }
                // Set the document file path in the contribution object
                contribution.FilePath = "~/uploads/" + documentFileName;
            }
            // Add the contribution to the context and save changes

            _context.Add(contribution);
            Console.WriteLine(contribution);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            // Handle any exceptions
            ModelState.AddModelError(string.Empty, "An error occurred while creating the contribution.");
            Console.WriteLine($"Error creating contribution: {ex.Message}");
            return RedirectToAction(nameof(Index));
        }
    }
    private bool ContributionExists(int id)
    {
        return _context.Contributions.Any(e => e.Id == id);
    }

    // GET: Contributions/Edit/5
    [Authorize]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var contribution = await _context.Contributions.FindAsync(id);
        if (contribution == null)
        {
            return NotFound();
        }

        var faculties = _context.Faculties.ToList();
        ViewBag.Faculties = new SelectList(faculties, "Id", "Name");

        return View("~/Areas/Authenticated/Views/Contributions/EditContribution.cshtml", contribution);
    }

    // POST: Contributions/Edit/5
    // POST: Contributions/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Contribution contribution, IFormFile imageFile, IFormFile documentFile)
    {
        if (id != contribution.Id)
        {
            return NotFound();
        }

        try
        {
            // Get the existing contribution from the database
            var existingContribution = await _context.Contributions.FindAsync(id);
            if (existingContribution == null)
            {
                return NotFound();
            }

            // Update only the properties that are modified
            existingContribution.Title = contribution.Title;
            existingContribution.Content = contribution.Content;
            existingContribution.FacultyId = contribution.FacultyId;

            // Handle image file update
            if (imageFile != null && imageFile.Length > 0)
            {
                // Save the new image file
                string imageFileName = Path.GetFileName(imageFile.FileName);
                string imageFilePath = Path.Combine("wwwroot", "uploads", imageFileName);
                using (var imageFileStream = new FileStream(imageFilePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(imageFileStream);
                }
                // Set the new image file path
                existingContribution.ImageUrl = "~/uploads/" + imageFileName;
            }

            // Handle document file update
            if (documentFile != null && documentFile.Length > 0)
            {
                // Save the new document file
                string documentFileName = Path.GetFileName(documentFile.FileName);
                string documentFilePath = Path.Combine("wwwroot", "uploads", documentFileName);
                using (var documentFileStream = new FileStream(documentFilePath, FileMode.Create))
                {
                    await documentFile.CopyToAsync(documentFileStream);
                }
                // Set the new document file path
                existingContribution.FilePath = "~/uploads/" + documentFileName;
            }

            // Update the contribution in the database
            _context.Update(existingContribution);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ContributionExists(contribution.Id))
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


    // GET: Contributions/Delete/5
    // GET: Contributions/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var contribution = await _context.Contributions.FirstOrDefaultAsync(m => m.Id == id);
        if (contribution == null)
        {
            return NotFound();
        }

        _context.Contributions.Remove(contribution);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }


    // POST: Contributions/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var contribution = await _context.Contributions.FindAsync(id);
        if (contribution == null)
        {
            return NotFound();
        }

        _context.Contributions.Remove(contribution);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "UniversityMarketingManager, Student")]
    public async Task<IActionResult> DownloadContribution(int facultyId, int id)
    {
        // Retrieve the contribution for the specified faculty
        var contribution = await _context.Contributions
            .FirstOrDefaultAsync(c => c.Id == id);

        if (contribution == null)
        {
            TempData["Message"] = "No contribution found for the specified faculty.";
            return RedirectToAction("Index");
        }

        using (var memoryStream = new MemoryStream())
        {
            using (var zipArchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
            {
                if (contribution.FileContent != null && contribution.FileContent.Length > 0)
                {
                    var facultyName = await _context.Faculties
                    .Where(f => f.Id == facultyId)
                    .Select(f => f.Name)
                    .FirstOrDefaultAsync();

                    var fileName = $"{facultyName}_{contribution.Id}";

                    var entry = zipArchive.CreateEntry(fileName + ".doc", CompressionLevel.Fastest);

                    // Write the file content to the entry in the ZIP archive
                    using (var entryStream = entry.Open())
                    {
                        await entryStream.WriteAsync(contribution.FileContent);
                    }
                }
            }

            memoryStream.Seek(0, SeekOrigin.Begin);

            var fileBytes = memoryStream.ToArray();

            var faculty = await _context.Faculties.FirstOrDefaultAsync(c => c.Id == facultyId);
            string facultyNameForFile = faculty.Name.ToString()+ "_" + contribution.Id.ToString() + ".zip";
            return File(fileBytes, "application/zip", facultyNameForFile);
        }
    }

    [Authorize]
    public async Task<IActionResult> CheckClosureDate(int facultyId)
    {

        var closureDate = await _context.ClosureDates
            .Where(cd => cd.FacultyId == facultyId)
            .FirstOrDefaultAsync();

         Console.WriteLine(closureDate);

        if (closureDate != null && DateTime.Now > closureDate.EndDate)
        {
            Console.WriteLine(closureDate != null && DateTime.Now > closureDate.EndDate);
            return Json(new { closureDatePassed = true });
        }

        Console.WriteLine(closureDate != null && DateTime.Now > closureDate.EndDate);
        return Json(new { closureDatePassed = false });
    }

    [HttpPost]
    public async Task<IActionResult> UpdateStatus(int id, string status)
    {
        // Find the contribution by ID
        var contribution = await _context.Contributions.FindAsync(id);
        if (contribution == null)
        {
            return NotFound();
        }

        // Update the status
        contribution.Status = status;
        // Save changes to the database
        try
        {
            await _context.SaveChangesAsync();
            return Ok();
        }
        catch (DbUpdateException)
        {
            // Log the error
            return StatusCode(500);
        }
    }

    public IActionResult AddComment(int contributionId)
    {
        // Retrieve the contribution from the database based on the ID
        var contribution = _context.Contributions.FirstOrDefault(c => c.Id == contributionId);
        return View("AddComenet",contribution);
    }

    [HttpPost]
    public async Task<IActionResult> AddComment(int contributionId, string commentText)
    {
        try
        {
            // Retrieve the contribution by its ID
            var contribution = await _context.Contributions.FindAsync(contributionId);
            if (contribution == null)
            {
                return NotFound();
            }

            // Update the comment attribute of the contribution
            contribution.CoordinatorComment = commentText;

            // Save changes to the database
            await _context.SaveChangesAsync();

            // Redirect to the Coordinator Index page
            return RedirectToAction(nameof(CoordinatorIndex));
        }
        catch (Exception ex)
        {
            // Handle errors
            ModelState.AddModelError(string.Empty, "An error occurred while adding the comment: " + ex.Message);
            return View("AddComenet");
        }
    }
}
