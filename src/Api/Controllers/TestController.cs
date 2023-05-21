using AwesomeApi.Filters;
using CRM.Application.Companies.Queries.GetCompany;
using CRM.Application.Companies.Queries.GetUserClaimsTypes;
using CRM.Application.Tests;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Api.Controllers;

[TestApiKey]
public class TestController : ApiControllerBase
{
    [HttpPost]
    [Route("ResetDb")]
    public async Task<ActionResult> ResetDb()
    {
        await Mediator.Send(new ResetDbCommand());
        return Ok();
    }

    [HttpPost]
    [Route("Companies")]
    public async Task<ActionResult<int>> CreateCompany(CreateTestCompanyCommand command)
    {
        var id = await Mediator.Send(command);
        return Ok(new { Id = id });
    }

    [HttpGet]
    [Route("Companies/{id}")]
    public async Task<ActionResult<CompanyVm>> GetCompany(int id)
    {
        return await Mediator.Send(new GetTestCompanyQuery { Id = id });
    }

    [HttpGet]
    [Route("UserClaimTypes")]
    public async Task<ActionResult<UserClaimTypeVm[]>> GetClaimTypes()
    {
        return await Mediator.Send(new GetTestUserClaimTypesQuery());
    }
}
