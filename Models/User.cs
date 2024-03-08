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
    
    [Required(ErrorMessage = "Password is required")]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [NotMapped]
    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; }
    
    public string Gender { get; set; }
    
    public string DoB { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // public int? FacultyId { get; set; }
    // public Faculty Faculty { get; set; }
}