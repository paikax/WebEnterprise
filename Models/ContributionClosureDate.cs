using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebEnterprise.Models;

public class ContributionClosureDate
{
    public int Id { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int FacultyId { get; set; } 
    public Faculty Faculty { get; set; }
}