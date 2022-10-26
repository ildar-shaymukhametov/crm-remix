using CRM.Application.Companies.Commands.UpdateClaims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM.App.Controllers;

[Authorize]
public class UsersController : ApiControllerBase
{
    [HttpPost]
    [Route("Claims")]
    public async Task<ActionResult> Claims(UpdateClaimsCommand request)
    {
        return Ok(await Mediator.Send(request));
    }
}