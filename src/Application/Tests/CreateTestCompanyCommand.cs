using CRM.Application.Common.Interfaces;
using CRM.Application.Companies.Commands.CreateCompany;
using MediatR;

namespace CRM.Application.Tests;

public record CreateTestCompanyCommand : CreateCompanyCommand
{
}

public class CreateTestCompanyCommandHandler : IRequestHandler<CreateTestCompanyCommand, int>
{
    private readonly IApplicationDbContext _context;

    public CreateTestCompanyCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public Task<int> Handle(CreateTestCompanyCommand request, CancellationToken cancellationToken)
    {
        var handler = new CreateCompanyCommandHandler(_context);
        return handler.Handle(request, cancellationToken);
    }
}
