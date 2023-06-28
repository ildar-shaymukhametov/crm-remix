using CRM.Application.Common.Exceptions;
using CRM.Application.Common.Interfaces;
using CRM.Application.Common.Security;
using CRM.Domain.Entities;
using FluentValidation.Results;
using MediatR;

namespace CRM.Application.Companies.Commands.UpdateCompany;

[Authorize(Constants.Policies.Company.Commands.Update)]
public record UpdateCompanyCommand(int Id) : IRequest
{
    public Dictionary<string, object?> Fields { get; set; } = new();
}

public class UpdateCompanyCommandHandler : IRequestHandler<UpdateCompanyCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateCompanyCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(UpdateCompanyCommand request, CancellationToken cancellationToken)
    {
        if (!request.Fields.Any())
        {
            throw new ValidationException(new[] { new ValidationFailure(nameof(UpdateCompanyCommand.Fields), "Must provide at least one value") });
        }

        var entity = await _context.Companies.FindAsync(new object?[] { request.Id }, cancellationToken);
        if (entity == null)
        {
            throw new NotFoundException("Company", request.Id);
        }

        if (request.Fields.TryGetValue(nameof(Company.ManagerId), out object? managerId))
        {
            entity.ManagerId = (string?)managerId;
        }

        if (new[] {
            nameof(Company.Address),
            nameof(Company.Ceo),
            nameof(Company.Contacts),
            nameof(Company.Email),
            nameof(Company.Inn),
            nameof(Company.Phone),
            nameof(Company.TypeId)
        }.Any(request.Fields.ContainsKey))
        {
            entity.Address = (string?)request.Fields[nameof(Company.Address)];
            entity.Ceo = (string?)request.Fields[nameof(Company.Ceo)];
            entity.Contacts = (string?)request.Fields[nameof(Company.Contacts)];
            entity.Email = (string?)request.Fields[nameof(Company.Email)];
            entity.Inn = (string?)request.Fields[nameof(Company.Inn)];
            entity.Phone = (string?)request.Fields[nameof(Company.Phone)];
            entity.TypeId = (int?)request.Fields[nameof(Company.TypeId)];
        }

        if (request.Fields.TryGetValue(nameof(Company.Name), out object? name))
        {
            entity.Name = (string)name!;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return;
    }
}
