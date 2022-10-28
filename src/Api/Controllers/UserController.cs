using CRM.Application.Users.Commands.UpdateUserAuthorizationClaims;
using CRM.Application.Users.Queries.GetUserAuthorizationClaims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM.App.Controllers;

[Authorize]
public class UserController : ApiControllerBase
{
    [HttpPost]
    [Route("AuthorizationClaims")]
    public async Task<ActionResult> UpdateAuthorizationClaims(UpdateUserAuthorizationClaimsCommand request)
    {
        return Ok(await Mediator.Send(request));
    }

    [Route("AuthorizationClaims")]
    public async Task<ActionResult> GetAuthorizationClaims()
    {
        return Ok(await Mediator.Send(new GetUserAuthorizationClaimsQuery()));
    }
}