namespace CRM.Application.Common.Interfaces;

public interface ITestService
{
    Task ResetDbAsync();
    Task CreateCheckpointAsync();
}
