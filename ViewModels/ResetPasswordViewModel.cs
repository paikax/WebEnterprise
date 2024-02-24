using System.ComponentModel.DataAnnotations;

namespace WebEnterprise.ViewModels;

public class ResetPasswordViewModel
{
    [Microsoft.Build.Framework.Required] [EmailAddress] public string? Email { get; set; }

    [Microsoft.Build.Framework.Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Microsoft.Build.Framework.Required]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Password and confirm password must match")]
    public string ConfirmPassword { get; set; }

    public string? Token { get; set; }
}