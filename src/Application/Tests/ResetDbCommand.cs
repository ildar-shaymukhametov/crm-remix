using CRM.Application.Common.Interfaces;
using CRM.Application.Common.Security;
using MediatR;

namespace CRM.Application.Tests;

[Authorize(Roles = Constants.Roles.Tester)]
public record ResetDbCommand : IRequest<Unit> { }

public class ResetDbCommandHandler : IRequestHandler<ResetDbCommand>
{
    private readonly ITestService _testService;

    public ResetDbCommandHandler(ITestService testService)
    {
        _testService = testService;
    }

    public async Task<Unit> Handle(ResetDbCommand request, CancellationToken cancellationToken)
    {
        await _testService.ResetDbAsync();
        return Unit.Value;
    }
}
