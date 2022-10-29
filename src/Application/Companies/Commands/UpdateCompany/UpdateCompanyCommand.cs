using CRM.Application.Common.Exceptions;
using CRM.Application.Common.Interfaces;
using CRM.Application.Common.Security;
using MediatR;

namespace CRM.Application.Companies.Commands.UpdateCompany;

[Authorize(Constants.Authorization.Policies.UpdateCompany)]
public record UpdateCompanyCommand : IRequest<Unit>
{
    public int Id { get; set; }
    public string? Type { get; set; }
    public string? Name { get; set; }
    public string? Inn { get; set; }
    public string? Address { get; set; }
    public string? Ceo { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Contacts { get; set; }
}

public class UpdateCompanyCommandHandler : IRequestHandler<UpdateCompanyCommand, Unit>
{
    private readonly IApplicationDbContext _context;

    public UpdateCompanyCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(UpdateCompanyCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Companies.FindAsync(new object?[] { request.Id }, cancellationToken);
        if (entity == null)
        {
            throw new NotFoundException("Company", request.Id);
        }

        _context.SetValues(entity, request);
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
