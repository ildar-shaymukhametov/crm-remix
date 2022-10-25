using System.ComponentModel.DataAnnotations;

public class UserClaimType : BaseEntity
{
    [MaxLength(100)]
    public string? Name { get; set; }

    [MaxLength(100)]
    public string? Value { get; set; }
}
