using CRM.Application.Users.Commands.CreateUser;
using CRM.Application.Users.Commands.UpdateUserAuthorizationClaims;
using CRM.Application.Users.Queries.GetUserAuthorizationClaims;
using CRM.Application.Users.Queries.GetUserPermissions;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Api.Controllers;

public class UserController : ApiControllerBase
{
    [HttpPost]
    [Route("AuthorizationClaims")]
    public async Task<ActionResult> UpdateAuthorizationClaims(UpdateUserAuthorizationClaimsCommand request)
    {
        await Mediator.Send(request);
        return Ok();
    }

    [HttpGet]
    [Route("AuthorizationClaims")]
    public async Task<ActionResult> GetAuthorizationClaims()
    {
        return Ok(await Mediator.Send(new GetUserAuthorizationClaimsQuery()));
    }

    [HttpGet]
    [Route("Permissions")]
    public async Task<ActionResult> GetPermissions([FromQuery] string[] q, string? resourceKey)
    {
        return Ok(await Mediator.Send(new GetUserPermissionsQuery
        {
            ResourceKey = resourceKey,
            RequestedPermissions = q
        }));
    }

    [HttpPost]
    public async Task<ActionResult<string>> Create(CreateUserCommand request)
    {
        var id = await Mediator.Send(request);
        return Ok(new { Id = id });
    }
}