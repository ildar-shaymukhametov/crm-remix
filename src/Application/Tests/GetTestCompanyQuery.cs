using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace CRM.Application.Companies.Queries.GetCompany;

public record GetTestCompanyQuery(int id) : GetCompanyQuery(id)
{
}

public class GetTestCompanyRequestHandler : IRequestHandler<GetTestCompanyQuery, CompanyVm>
{
    private readonly IServiceProvider _serviceProvider;

    public GetTestCompanyRequestHandler(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<CompanyVm> Handle(GetTestCompanyQuery request, CancellationToken cancellationToken)
    {
        var handler = _serviceProvider.GetRequiredService<GetCompanyRequestHandler>();
        return await handler.Handle(request, cancellationToken);
    }
}