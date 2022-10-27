using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Duende.IdentityServer.EntityFramework.Options;
using CRM.Infrastructure.Identity;
using CRM.Application.Common.Interfaces;
using System.Reflection;
using MediatR;
using CRM.Domain.Entities;
using CRM.Infrastructure.Persistence.Interceptors;

namespace CRM.Infrastructure.Persistence;

public class ApplicationDbContext : ApiAuthorizationDbContext<AspNetUser>, IApplicationDbContext
{
    private readonly IMediator _mediator;
    private readonly AuditableEntitySaveChangesInterceptor _auditableEntitySaveChangesInterceptor;

    public DbSet<Company> Companies => Set<Company>();
    public DbSet<UserClaimType> UserClaimTypes => Set<UserClaimType>();
    public DbSet<ApplicationUser> ApplicationUsers => Set<ApplicationUser>();

    public ApplicationDbContext(DbContextOptions options, IOptions<OperationalStoreOptions> operationalStoreOptions, IMediator mediator, AuditableEntitySaveChangesInterceptor auditableEntitySaveChangesInterceptor) : base(options, operationalStoreOptions)
    {
        _mediator = mediator;
        _auditableEntitySaveChangesInterceptor = auditableEntitySaveChangesInterceptor;
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        builder.Seed();

        base.OnModelCreating(builder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(_auditableEntitySaveChangesInterceptor);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _mediator.DispatchDomainEvents(this);

        return await base.SaveChangesAsync(cancellationToken);
    }

    public void SetValues<T, U>(T entity, U dto) where T : notnull where U : notnull
    {
        base.Entry(entity).CurrentValues.SetValues(dto);
    }
}
