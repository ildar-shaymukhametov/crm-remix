using CRM.Application.Common.Exceptions;
using CRM.Application.Common.Interfaces;
using CRM.Application.Common.Security;
using MediatR;

namespace CRM.Application.Companies.Commands.DeleteCompany;

[Authorize(Constants.Policies.Company.Commands.Delete)]
public record DeleteCompanyCommand : IRequest
{
    public int Id { get; set; }
}

public class DeleteCompanyCommandHandler : IRequestHandler<DeleteCompanyCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteCompanyCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(DeleteCompanyCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Companies.FindAsync(new object?[] { request.Id }, cancellationToken);
        if (entity == null)
        {
            throw new NotFoundException("Company", request.Id);
        }

        _context.Companies.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return;
    }
}
