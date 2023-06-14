using CRM.Application.Common.Interfaces;
using CRM.Application.Common.Security;
using CRM.Domain.Entities;
using CRM.Domain.Events;
using MediatR;

namespace CRM.Application.Companies.Commands.CreateCompany;

[Authorize(Constants.Policies.Company.Commands.Create)]
public record CreateCompanyCommand(string Name) : IRequest<int>
{
    public int? TypeId { get; set; }
    public string? Inn { get; set; }
    public string? Address { get; set; }
    public string? Ceo { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Contacts { get; set; }
    public string? ManagerId { get; set; }
}

public class CreateCompanyCommandHandler : IRequestHandler<CreateCompanyCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly IAccessService _accessService;

    public CreateCompanyCommandHandler(IApplicationDbContext context, IAccessService accessService)
    {
        _context = context;
        _accessService = accessService;
    }

    public async Task<int> Handle(CreateCompanyCommand request, CancellationToken cancellationToken)
    {
        var entity = new Company
        {
            Address = request.Address,
            Ceo = request.Ceo,
            Contacts = request.Contacts,
            Email = request.Email,
            Inn = request.Inn,
            ManagerId = request.ManagerId,
            Name = request.Name,
            Phone = request.Phone,
            TypeId = request.TypeId
        };

        entity.AddDomainEvent(new CompanyCreatedEvent(entity));

        _context.Companies.Add(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
