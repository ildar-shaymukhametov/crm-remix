using CRM.Application.Companies.Commands.CreateCompany;

namespace CRM.Application.Common.Behaviours.Authorization.Resources;

public class CreateCompanyResource : IResource<CreateCompanyCommand>
{
    public CreateCompanyCommand Request { get; }

    public CreateCompanyResource(CreateCompanyCommand request)
    {
        Request = request;
    }
}
