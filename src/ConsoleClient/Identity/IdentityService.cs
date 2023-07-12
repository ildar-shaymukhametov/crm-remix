using ConsoleClient.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ConsoleClient.Identity;

internal class IdentityService
{
    public static IdentityUser User = default!;
    private readonly ApplicationDbContext _dbContext;

    public IdentityService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<(bool, IdentityUser?)> AuthenticateAsync(string? userName, string? password)
    {
        var user = await _dbContext.Users.SingleOrDefaultAsync(x => x.UserName == userName);
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
