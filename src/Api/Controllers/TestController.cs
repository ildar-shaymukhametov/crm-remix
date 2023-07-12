using AwesomeApi.Filters;
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
}
