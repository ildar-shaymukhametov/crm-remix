using CRM.App.Controllers;
using CRM.Application.Tests;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Api.Controllers;

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
