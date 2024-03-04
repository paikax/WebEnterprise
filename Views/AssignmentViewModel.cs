using WebEnterprise.Models;

namespace WebEnterprise.Views;

public class AssignmentViewModel
{
    public int FacultyId { get; set; }
    public IEnumerable<Assigment> AssignmentList { get; set; }
    public IEnumerable<User> CoordinatorList { get; set; }
}