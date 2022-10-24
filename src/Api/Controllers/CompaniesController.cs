using CRM.Application.Companies.Commands.CreateCompany;
using CRM.Application.Companies.Commands.DeleteCompany;
using CRM.Application.Companies.Queries.GetCompanies;
using CRM.Application.Companies.Queries.GetCompany;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM.App.Controllers;

[Authorize]
public class CompaniesController : ApiControllerBase
{
    [HttpGet]
    [Route("{id}")]
    public async Task<ActionResult<CompanyVm>> Get(int id)
    {
        return await Mediator.Send(new GetCompanyQuery { Id = id });
    }

    [HttpGet]
    public async Task<ActionResult<CompanyDto[]>> Get()
    {
        return await Mediator.Send(new GetCompaniesQuery());
    }

    [HttpPost]
    public async Task<ActionResult<int>> Create(CreateCompanyCommand command)
    {
        var id = await Mediator.Send(command);
        return Ok(new { Id = id });
    }

    [HttpDelete]
    [Route("{id}")]
    public async Task<ActionResult<int>> Delete(int id)
    {
        await Mediator.Send(new DeleteCompanyCommand { Id = id });
        return NoContent();
    }
}