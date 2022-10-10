using CRM.Application.Companies.Commands.CreateCompany;
using CRM.Application.Companies.Queries.GetCompanies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM.App.Controllers;

[Authorize]
public class CompaniesController : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<CompanyDto[]>> Get()
    {
        return await Mediator.Send(new GetCompaniesQuery());
    }

    [HttpPost]
    public async Task<ActionResult<int>> Create(CreateCompanyCommand command)
    {
        return await Mediator.Send(command);
    }
}