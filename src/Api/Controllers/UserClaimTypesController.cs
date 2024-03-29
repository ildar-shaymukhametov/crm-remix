using CRM.Application.Companies.Queries.GetUserClaimsTypes;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Api.Controllers;

public class UserClaimTypesController : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<UserClaimTypeVm[]>> Get()
    {
        return await Mediator.Send(new GetUserClaimTypesQuery());
    }
}