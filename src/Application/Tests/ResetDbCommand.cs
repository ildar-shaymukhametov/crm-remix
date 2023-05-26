using CRM.Application.Common.Interfaces;
using MediatR;

namespace CRM.Application.Tests;

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
