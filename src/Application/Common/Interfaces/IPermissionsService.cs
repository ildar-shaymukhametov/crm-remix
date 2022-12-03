namespace CRM.Application.Common.Interfaces;

public interface IPermissionsService
{
    Task<string[]> CheckUserPermissionsAsync(string userId, string? resourceKey, params string[] permissions);
}
