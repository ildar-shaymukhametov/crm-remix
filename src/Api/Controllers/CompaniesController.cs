using CRM.Application.Companies.Commands.CreateCompany;
using CRM.Application.Companies.Commands.DeleteCompany;
using CRM.Application.Companies.Commands.UpdateCompany;
using CRM.Application.Companies.Queries;
using CRM.Application.Companies.Queries.GetCompanies;
using CRM.Application.Companies.Queries.GetCompany;
using CRM.Application.Companies.Queries.GetCompanyManagers;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Api.Controllers;

public class CompaniesController : ApiControllerBase
{
    [HttpGet]
    [Route("{id}")]
    public async Task<ActionResult<CompanyVm>> Get(int id)
    {
        return await Mediator.Send(new GetCompanyQuery(id));
    }

    [HttpGet]
    public async Task<ActionResult<CompanyVm[]>> Get()
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

    [HttpPut]
    [Route("{id}")]
    public async Task<ActionResult> Update(int id, UpdateCompanyCommand command)
    {
        command.Id = id;
        await Mediator.Send(command);
        return Ok();
    }

    [HttpGet]
    [Route("InitData")]
    public async Task<ActionResult<GetCompanyInitDataResponse>> InitData(int? id)
    {
        return await Mediator.Send(new GetCompanyInitDataQuery { Id = id });
    }
}