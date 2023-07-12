namespace CRM.Application.Companies.Queries.GetUpdateCompany;

public class UpdateCompanyVm
{
    public int Id { get; }

    public UpdateCompanyVm(int id)
    {
        Id = id;
    }

    public Dictionary<string, object?> Fields { get; } = new();
    public InitData InitData { get; } = new();
}

public class InitData
{
    public List<CompanyTypeDto> CompanyTypes { get; set; } = new();
    public List<ManagerDto> Managers { get; set; } = new();
}