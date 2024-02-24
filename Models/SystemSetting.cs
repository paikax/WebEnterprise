using System.ComponentModel.DataAnnotations;

namespace WebEnterprise.Models;

public class SystemSetting
{
    [Key]
    public int Id { get; set; }
    public string SettingName { get; set; }
    public string SettingValue { get; set; }
    public string Description { get; set; }
}