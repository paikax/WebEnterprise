using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebEnterprise.Models;

public class Contribution
{
    public int Id { get; set; }
    public string Status { get; set; }
    public string Title { get; set; }
    public string Content { get; set; } // Content of the article
    public string ImageUrl { get; set; } // URL to the uploaded image
    public DateTime SubmissionDate { get; set; }
    public bool SelectedForPublication { get; set; }
    public bool TermsAndConditionsAccepted { get; set; }
    public string StudentId { get; set; }
    public User Student { get; set; }
    public int FacultyId { get; set; } // Foreign key for Faculty
    public Faculty Faculty { get; set; } // Navigation property
}