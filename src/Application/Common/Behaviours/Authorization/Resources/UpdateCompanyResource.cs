using CRM.Application.Companies.Commands.UpdateCompany;
using CRM.Domain.Entities;

namespace CRM.Application.Common.Behaviours.Authorization.Resources;

public class UpdateCompanyResource
{
    public Company Company { get; }
    public UpdateCompanyCommand Request { get; set; }

    public UpdateCompanyResource(Company company, UpdateCompanyCommand request)
    {
        Company = company;
        Request = request;
    }
}
