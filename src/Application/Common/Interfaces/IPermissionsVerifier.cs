namespace CRM.Application.Common.Interfaces;

public interface IPermissionsVerifier
{
    Task<string[]> VerifyAsync(string userId, string? resourceKey, params string[] permissions);
    Task<Dictionary<int, string[]>> VerifyCompanyPermissionsAsync(string userId, int[] ids, params string[] permissions);
}
