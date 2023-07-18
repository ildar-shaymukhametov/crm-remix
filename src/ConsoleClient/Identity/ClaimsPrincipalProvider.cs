using System.Security.Claims;
using ConsoleClient.Persistence;
using CRM.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ConsoleClient.Identity;

public class ClaimsPrincipalProvider : IClaimsPrincipalProvider
{
    private readonly IServiceScopeFactory _scopeFactory;

    public ClaimsPrincipalProvider(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public async Task<ClaimsPrincipal> CreateClaimsPrincipalAsync(string userId)
    {
        using var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var user = await dbContext.IdentityUsers.Include(x => x.Claims).SingleOrDefaultAsync(x => x.Id == userId);
        var claims = user?.Claims?.Select(x => new Claim(x.Type, x.Value));

        return new ClaimsPrincipal(new ClaimsIdentity(claims));
    }
}
