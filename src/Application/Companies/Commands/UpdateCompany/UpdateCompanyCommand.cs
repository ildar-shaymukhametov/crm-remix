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
            if (request.Fields.TryGetValue(nameof(Company.Address), out object? address))
            {
                entity.Address = (string?)address;
            }
            if (request.Fields.TryGetValue(nameof(Company.Ceo), out object? ceo))
            {
                entity.Ceo = (string?)ceo;
            }
            if (request.Fields.TryGetValue(nameof(Company.Contacts), out object? contacts))
            {
                entity.Contacts = (string?)contacts;
            }
            if (request.Fields.TryGetValue(nameof(Company.Email), out object? email))
            {
                entity.Email = (string?)email;
            }
            if (request.Fields.TryGetValue(nameof(Company.Inn), out object? inn))
            {
                entity.Inn = (string?)inn;
            }
            if (request.Fields.TryGetValue(nameof(Company.Phone), out object? phone))
            {
                entity.Phone = (string?)phone;
            }
            if (request.Fields.TryGetValue(nameof(Company.TypeId), out object? typeId))
            {
                if (int.TryParse((string?)typeId, out int id))
                {
                    entity.TypeId = id;
                }
                else
                {
                    throw new ValidationException(new[] { new ValidationFailure(nameof(Company.TypeId), "Value must be convertible to a number") });
                }
            }
        }

        if (request.Fields.TryGetValue(nameof(Company.Name), out object? name))
        {
            entity.Name = (string)name!;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return;
    }
}
