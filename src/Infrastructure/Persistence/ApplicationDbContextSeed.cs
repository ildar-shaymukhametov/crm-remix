using Microsoft.EntityFrameworkCore;

public static class ApplicationDbContextSeed
{
    public static void Seed(this ModelBuilder builder)
    {
        builder.Entity<UserClaimType>().HasData(
            new UserClaimType { Id = 1, Value = "company.create", Name = "Компания. Добавление" },
            new UserClaimType { Id = 2, Value = "company.update", Name = "Компания. Редактирование" },
            new UserClaimType { Id = 3, Value = "company.delete", Name = "Компания. Удаление" },
            new UserClaimType { Id = 4, Value = "company.view", Name = "Компания. Просмотр" }
        );
    }
}