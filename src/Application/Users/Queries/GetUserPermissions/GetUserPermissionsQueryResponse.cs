using System.Globalization;
using System.Text;

namespace CRM.Application.Users.Queries.GetUserPermissions;

public record GetUserPermissionsQueryResponse
{
    public string[] Permissions { get; set; } = Array.Empty<string>();

    protected virtual bool PrintMembers(StringBuilder stringBuilder)
    {
        stringBuilder.Append($"{nameof(Permissions)} = [{string.Join(", ", Permissions)}]");
        return true;
    }
}
