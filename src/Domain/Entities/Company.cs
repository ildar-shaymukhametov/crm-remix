﻿namespace CRM.Domain.Entities;

public class Company : BaseAuditableEntity
{
    public int? TypeId { get; set; }
    public virtual CompanyType? Type { get; set; }
    public string Name { get; set; } = default!;
    public string? Inn { get; set; }
    public string? Address { get; set; }
    public string? Ceo { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Contacts { get; set; }
    public string? ManagerId { get; set; }
    public virtual ApplicationUser? Manager { get; set; }
}
