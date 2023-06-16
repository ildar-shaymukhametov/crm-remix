using AwesomeApi.Filters;
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

    [HttpGet]
    [Route("UserClaimTypes")]
    public async Task<ActionResult<UserClaimTypeVm[]>> GetClaimTypes()
    {
        return await Mediator.Send(new GetTestUserClaimTypesQuery());
    }
}
