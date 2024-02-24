using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WebEnterprise.BaseEntity;

[Serializable]
public abstract class BaseEntity
{
    
    protected BaseEntity()
    {
        Id = Guid.NewGuid().ToString();
        CreateAt = DateTime.UtcNow;
    }
    [Key] 
    public string Id { get; set; }
    public DateTime CreateAt { get; set; }
    public string CreatedBy { get; set; }

    public DateTime? UpdateAt { get; set; }

    public string UpdateBy { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime? DeleteAt { get; set; }

    public string DeleteBy { get; set; }
    
    public Dictionary<string, object>? ExtraElements { get; set; }
}