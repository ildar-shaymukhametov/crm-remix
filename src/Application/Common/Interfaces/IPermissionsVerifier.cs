namespace CRM.Application.Common.Interfaces;

public interface IPermissionsVerifier
{
    Task<string[]> VerifyAsync(string userId, string? resourceKey, params string[] permissions);
}
