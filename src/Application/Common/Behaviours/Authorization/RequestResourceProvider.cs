using CRM.Application.Common.Behaviours.Authorization.Resources;
using CRM.Application.Common.Exceptions;
using CRM.Application.Companies.Commands.CreateCompany;
using CRM.Application.Companies.Commands.DeleteCompany;
using CRM.Application.Companies.Commands.UpdateCompany;
using CRM.Application.Companies.Queries.GetCompany;
using CRM.Domain.Entities;

namespace CRM.Application.Common.Behaviours.Authorization;

public interface IRequestResourceProvider
{
    Task<(object? Resource, object? Key)> GetResourceAsync<TRequest>(TRequest request);
}

public class RequestResourceProvider : IRequestResourceProvider
{
    private readonly IResourceProvider _resourceProvider;

    public RequestResourceProvider(IResourceProvider resourceProvider)
    {
        _resourceProvider = resourceProvider;
    }

    public async Task<(object? Resource, object? Key)> GetResourceAsync<TRequest>(TRequest request)
    {
        return request switch
        {
            GetCompanyQuery query => (await _resourceProvider.GetCompanyAsync(query.Id), query.Id),
            UpdateCompanyCommand query => (new UpdateCompanyResource(await GetResourceAsync(query), query), query.Id),
            DeleteCompanyCommand query => (await _resourceProvider.GetCompanyAsync(query.Id), query.Id),
            CreateCompanyCommand query => (query, null),
            _ => (null, null),
        };
    }

    private async Task<Company> GetResourceAsync(UpdateCompanyCommand query)
    {
        return await _resourceProvider.GetCompanyAsync(query.Id) ?? throw new NotFoundException(nameof(Company), query.Id);
    }
}
