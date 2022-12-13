using CRM.Application.Companies.Commands.UpdateCompany;

namespace CRM.Application.Common.Behaviours.Authorization.Resources;

public class UpdateCompanyResource : IResource<UpdateCompanyCommand>
{
    public CompanyDto Company { get; }
    public UpdateCompanyCommand Request { get; set; }

    public UpdateCompanyResource(CompanyDto company, UpdateCompanyCommand request)
    {
        Company = company;
        Request = request;
    }
}
