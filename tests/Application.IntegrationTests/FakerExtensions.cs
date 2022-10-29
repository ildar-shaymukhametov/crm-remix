using CRM.Domain.Entities;

namespace Faker;

public static class Date
{
    public static DateTime RandomDateTime()
    {
        var year = Random.Shared.Next(1990, 2022);
        var month = Random.Shared.Next(1, 13);
        var day = Random.Shared.Next(1, DateTime.DaysInMonth(year, month) + 1);
        var hour = Random.Shared.Next(1, 24);
        var minute = Random.Shared.Next(1, 60);
        var second = Random.Shared.Next(1, 60);

        return new DateTime(year, month, day, hour, minute, second);
    }

    public static DateTime RandomDateTimeUtc()
    {
        return RandomDateTime().ToUniversalTime();
    }
}

public static class Builders
{
    public static CRM.Domain.Entities.Company Company()
    {
        var result = new CRM.Domain.Entities.Company
        {
            Type = Faker.Company.Suffix(),
            Name = Faker.Company.Name(),
            Inn = Faker.RandomNumber.Next(1_000_000_000, 9_999_999_999).ToString(),
            Address = $"{Faker.Address.City}, {Faker.Address.StreetAddress}",
            Ceo = Faker.Name.FullName(),
            Phone = Faker.Phone.Number(),
            Email = Faker.Internet.Email(),
            Contacts = Faker.Internet.FreeEmail(),
            CreatedAtUtc = Faker.Date.RandomDateTimeUtc()
        };

        return result;
    }
}
