using CRM.Application.Common.Behaviours.Authorization.Resources;
using CRM.Application.Common.Exceptions;
using CRM.Application.Common.Interfaces;
using CRM.Application.Companies.Commands.CreateCompany;
using CRM.Application.Companies.Commands.DeleteCompany;
using CRM.Application.Companies.Commands.UpdateCompany;
using CRM.Application.Companies.Queries.GetCompany;
using CRM.Application.Companies.Queries.GetUpdateCompany;
using CRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CRM.Application.Common.Behaviours.Authorization;

public interface IRequestResourceProvider
{
    Task<(object? Resource, object? Key)> GetResourceAsync<TRequest>(TRequest request);
}

public class RequestResourceProvider : IRequestResourceProvider
{
    private readonly IApplicationDbContext _dbContext;

    public RequestResourceProvider(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<(object? Resource, object? Key)> GetResourceAsync<TRequest>(TRequest request)
    {
        return request switch
        {
            GetCompanyQuery query => (await GetCompanyAsync(query.Id), query.Id),
            UpdateCompanyCommand query => (new UpdateCompanyResource(await GetResourceAsync(query), query), query.Id),
            DeleteCompanyCommand query => (await GetCompanyAsync(query.Id), query.Id),
            CreateCompanyCommand query => (query, null),
            GetUpdateCompanyQuery query => (await GetCompanyAsync(query.Id), query.Id),
            _ => (null, null),
        };
    }

    private async Task<Company> GetResourceAsync(UpdateCompanyCommand query)
    {
        return await GetCompanyAsync(query.Id) ?? throw new NotFoundException(nameof(Company), query.Id);
    }

    private async Task<Company?> GetCompanyAsync(int id)
    {
        return await _dbContext.Companies
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == id);
    }
}
