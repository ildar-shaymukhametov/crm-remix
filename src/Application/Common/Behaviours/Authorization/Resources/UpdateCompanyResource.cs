using CRM.Application.Companies.Commands.UpdateCompany;

namespace CRM.Application.Common.Behaviours.Authorization.Resources;

public class UpdateCompanyResource : IResource<CompanyDto>
{
    public CompanyDto Request { get; }
    public UpdateCompanyCommand? Command { get; set; }


    public UpdateCompanyResource(CompanyDto company)
    {
        Request = company;
    }

    public UpdateCompanyResource(CompanyDto company, UpdateCompanyCommand request)
    {
        Request = company;
        Command = request;
    }
}
