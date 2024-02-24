

using System.ComponentModel.DataAnnotations;

namespace WebEnterprise.Models;

public class FileModel
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
    public string ContentType { get; set; }
    public byte[] Data { get; set; }
}