using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebEnterprise.Models;

public class Assigment
{
    [Key] public int Id { get; set; }

    [Required] public int FacultyId { get; set; }

    [ForeignKey("FacultyId")] public Faculty Faculty { get; set; }

    [Required] public string CoordinatorId { get; set; }

    [ForeignKey("CoordinatorId")] public User Coordinator { get; set; }
}