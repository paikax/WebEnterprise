using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebEnterprise.Models;

public class Comment
{
    public int Id { get; set; }
    public string Content { get; set; }
    public DateTime CommentDate { get; set; }
    public int ContributionId { get; set; } // Foreign key for Contribution
    public Contribution Contribution { get; set; } // Navigation property
}