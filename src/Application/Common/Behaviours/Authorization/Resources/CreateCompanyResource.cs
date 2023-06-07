using CRM.Application.Companies.Commands.CreateCompany;

namespace CRM.Application.Common.Behaviours.Authorization.Resources;

public class CreateCompanyResource
{
    public CreateCompanyCommand Request { get; }

    public CreateCompanyResource(CreateCompanyCommand request)
    {
        Request = request;
    }
}
