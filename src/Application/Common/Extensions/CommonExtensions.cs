namespace CRM.Application.Common.Extensions;

public static class CommonExtensions
{
    public static bool ContainsAny(this string[] @this, params string[] values)
    {
        return @this.Any(values.Contains);
    }
}
