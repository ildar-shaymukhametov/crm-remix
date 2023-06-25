using FluentValidation;

namespace CRM.Application.Companies.Commands.UpdateCompany;

public class UpdateCompanyCommandValidator : AbstractValidator<UpdateCompanyCommand>
{
    public UpdateCompanyCommandValidator()
    {
        // RuleFor(x => x.Name)
        //     .MaximumLength(200)
        //     .NotEmpty();
        // RuleFor(x => x.Email)
        //     .EmailAddress()
        //     .When(x => !string.IsNullOrWhiteSpace(x.Email));
        // RuleFor(x => x.Inn)
        //     .Length(10)
        //     .When(x => !string.IsNullOrWhiteSpace(x.Inn));
    }
}