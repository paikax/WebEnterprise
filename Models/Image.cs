using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebEnterprise.Models;

public class Image
{
    [Key]
    public int Id { get; set; }

    public int ContributionId { get; set; }
    public Contribution Contribution { get; set; }

    public string ImagePath { get; set; }
    public string ImageDescription { get; set; }
    public DateTime UploadDate { get; set; }
}