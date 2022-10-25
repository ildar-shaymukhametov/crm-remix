﻿using CRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CRM.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Company> Companies { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    void SetValues<T, U>(T entity, U dto) where T : notnull where U : notnull;
}
