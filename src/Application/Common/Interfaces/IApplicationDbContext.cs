﻿using CRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace CRM.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Company> Companies { get; }
    DbSet<UserClaimType> UserClaimTypes { get; }
    DbSet<ApplicationUser> ApplicationUsers { get; }
    DbSet<CompanyType> CompanyTypes { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    void SetValues<T, U>(T entity, U dto) where T : notnull where U : notnull;
    DatabaseFacade Database { get; }
}
