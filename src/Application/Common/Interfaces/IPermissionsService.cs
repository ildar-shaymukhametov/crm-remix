namespace CRM.Application.Common.Interfaces;

public interface IPermissionsService
{
    Task<string[]> CheckAccessAsync(string userId, params string[] permissions);
}
