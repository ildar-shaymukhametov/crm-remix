using AutoMapper;
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
    private readonly IMapper _mapper;

    public CreateTestCompanyCommandHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public Task<int> Handle(CreateTestCompanyCommand request, CancellationToken cancellationToken)
    {
        var handler = new CreateCompanyCommandHandler(_context, _mapper);
        return handler.Handle(request, cancellationToken);
    }
}
