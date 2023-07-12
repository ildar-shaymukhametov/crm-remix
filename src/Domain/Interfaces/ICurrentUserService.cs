namespace CRM.Domain.Interfaces;

public interface ICurrentUserService
{
    string? UserId { get; }
}
