using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace WebEnterprise.Models;

public class User : IdentityUser
{
    public User()
    {
        CreatedAt = DateTime.Now;
    }
    
    [Required(ErrorMessage = "Full Name is required")]
    [StringLength(50, ErrorMessage = "Full Name should be less than 50 characters")]
    public string FullName { get; set; }
    public string Role { get; set; }
    
    public string Gender { get; set; }
    
    public string DoB { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // public int? FacultyId { get; set; }
    // public Faculty Faculty { get; set; }
}