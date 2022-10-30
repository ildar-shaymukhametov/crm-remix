using CRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;

public static class ApplicationDbContextSeed
{
    public static void Seed(this ModelBuilder builder)
    {
        builder.Entity<UserClaimType>().HasData(
            new UserClaimType { Id = 1, Value = "company.create", Name = "Company. Create" },
            new UserClaimType { Id = 2, Value = "company.update", Name = "Company. Update" },
            new UserClaimType { Id = 3, Value = "company.delete", Name = "Company. Delete" },
            new UserClaimType { Id = 4, Value = "company.view", Name = "Company. View" }
        );
    }
}