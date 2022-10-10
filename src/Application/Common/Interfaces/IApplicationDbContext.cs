using CRM.App.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CRM.App.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Company> Companies { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
