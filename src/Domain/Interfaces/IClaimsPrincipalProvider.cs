using System.Security.Claims;

namespace CRM.Domain.Interfaces;

public interface IClaimsPrincipalProvider
{
    Task<ClaimsPrincipal> CreateClaimsPrincipalAsync(string userId);
}
