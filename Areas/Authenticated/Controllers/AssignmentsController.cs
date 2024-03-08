using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebEnterprise.Data;
using WebEnterprise.Views;

namespace WebEnterprise.Areas.Authenticated.Controllers;
[Area(Constants.Areas.AuthenticatedArea)]
[Authorize(Roles = Constants.Roles.AdminRole)]
public class AssignmentsController : Controller
{
    private static int facultyId;

    private readonly ApplicationDbContext _db;

    public AssignmentsController(ApplicationDbContext db)
    {
        _db = db;
    }
    
}