﻿using System.ComponentModel.DataAnnotations;

namespace WebEnterprise.Models;

public class Faculty
{
    [Key]
    public int Id { get; set; }
    public string RoleName { get; set; }
}
