using CRM.Application.Common.Interfaces;
using CRM.Application.Companies.Commands.CreateCompany;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace CRM.Application.Tests;

public record CreateTestCompanyCommand : CreateCompanyCommand
{
}

public class CreateTestCompanyCommandHandler : IRequestHandler<CreateTestCompanyCommand, int>
{
    private readonly IServiceProvider _serviceProvider;

    public CreateTestCompanyCommandHandler(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<int> Handle(CreateTestCompanyCommand request, CancellationToken cancellationToken)
    {
        var handler = _serviceProvider.GetRequiredService<CreateCompanyCommandHandler>();
        return await handler.Handle(request, cancellationToken);
    }
}
