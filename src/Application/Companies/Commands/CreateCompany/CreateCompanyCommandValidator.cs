using FluentValidation;

namespace CRM.App.Application.Companies.Commands.CreateCompany;

public class CreateCompanyCommandValidator : AbstractValidator<CreateCompanyCommand>
{
    public CreateCompanyCommandValidator()
    {
        RuleFor(x => x.Name)
            .MaximumLength(200)
            .NotEmpty();
        RuleFor(x => x.Email)
            .EmailAddress();
        RuleFor(x => x.Inn)
            .Length(10);
    }
}