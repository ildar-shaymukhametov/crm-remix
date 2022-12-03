using CRM.Application.Companies.Commands.DeleteCompany;
using CRM.Application.Companies.Commands.UpdateCompany;
using CRM.Application.Companies.Queries.GetCompany;

namespace CRM.Application.Common.Behaviours.Authorization;

public interface IRequestResourceProvider
{
    Task<(object? Resource, bool IsResourceRequest)> GetResourceAsync<TRequest>(TRequest request);
}

public class RequestResourceProvider : IRequestResourceProvider
{
    private readonly IResourceProvider _resourceProvider;

    public RequestResourceProvider(IResourceProvider resourceProvider)
    {
        _resourceProvider = resourceProvider;
    }

    public async Task<(object? Resource, bool IsResourceRequest)> GetResourceAsync<TRequest>(TRequest request)
    {
        return request switch
        {
            GetCompanyQuery query => (await _resourceProvider.GetCompanyAsync(query.Id), true),
            UpdateCompanyCommand query => (await _resourceProvider.GetCompanyAsync(query.Id), true),
            DeleteCompanyCommand query => (await _resourceProvider.GetCompanyAsync(query.Id), true),
            _ => (null, false),
        };
    }
}
