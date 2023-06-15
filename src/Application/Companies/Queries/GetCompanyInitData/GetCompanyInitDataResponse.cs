namespace CRM.Application.Companies.Queries.GetCompanyManagers;

public record GetCompanyInitDataResponse
{
    public List<ManagerDto> Managers { get; set; } = new List<ManagerDto>();
    public List<CompanyTypeDto> CompanyTypes { get; internal set; } = new List<CompanyTypeDto>();
}
