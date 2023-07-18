namespace ConsoleClient.Identity;

public class IdentityClaim
{
    public IdentityClaim(string type, string value)
    {
        Type = type;
        Value = value;
    }

    public int Id { get; set; }
    public string Type { get; set; }
    public string Value { get; set; }
}
