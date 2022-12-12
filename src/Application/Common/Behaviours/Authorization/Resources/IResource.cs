namespace CRM.Application.Common.Behaviours.Authorization.Resources;

public interface IResource<T>
{
    public T Request { get; }
}
