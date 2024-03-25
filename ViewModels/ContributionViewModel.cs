using System.ComponentModel.DataAnnotations;
using WebEnterprise.Models;

namespace WebEnterprise.ViewModels;

public class ContributionViewModel
{
    [Required(ErrorMessage = "Title is required")]
    public string Title { get; set; }

    [Required(ErrorMessage = "Content is required")]
    public string Content { get; set; }

    public string ImageUrl { get; set; }

    [Display(Name = "Select Faculty")]
    public int SelectedFacultyId { get; set; }

    [Display(Name = "Accept Terms and Conditions")]
    [Range(typeof(bool), "true", "true", ErrorMessage = "You must accept the terms and conditions")]
    public bool TermsAndConditionsAccepted { get; set; }

    public List<Faculty> Faculties { get; set; }
}