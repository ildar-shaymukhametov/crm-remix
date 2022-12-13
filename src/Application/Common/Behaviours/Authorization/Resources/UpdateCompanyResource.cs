using CRM.Application.Companies.Commands.UpdateCompany;

namespace CRM.Application.Common.Behaviours.Authorization.Resources;

public class UpdateCompanyResource : IResource<UpdateCompanyCommand>
{
    public UpdateCompanyCommand Request { get; }
    public CompanyDto Company { get; set; }

    public UpdateCompanyResource(UpdateCompanyCommand request, CompanyDto company)
    {
        Request = request;
        Company = company;
    }
}
