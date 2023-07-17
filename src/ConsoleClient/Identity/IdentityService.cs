using ConsoleClient.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ConsoleClient.Identity;

internal class IdentityService
{
    public static IdentityUser User = default!;
    private readonly IServiceScopeFactory _scopeFactory;

    public IdentityService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public async Task<(bool, IdentityUser?)> AuthenticateAsync(string? userName, string? password)
    {
        using var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var user = await dbContext.Users.SingleOrDefaultAsync(x => x.UserName == userName);
        if (user is null)
        {
            return (false, null);
        }

        var ok = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
        if (!ok)
        {
            return (false, null);
        }

        return (true, user);
    }
}
