using CRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using static CRM.Application.Constants;

namespace CRM.Infrastructure.Persistence;

public static class ApplicationDbContextSeed
{
    public static void Seed(this ModelBuilder builder)
    {
        builder.Entity<UserClaimType>().HasData(
            new UserClaimType { Id = 1, Value = Claims.CreateCompany, Name = "Company. Create" },
            new UserClaimType { Id = 2, Value = Claims.UpdateCompany, Name = "Company. Update" },
            new UserClaimType { Id = 3, Value = Claims.DeleteCompany, Name = "Company. Delete" },
            new UserClaimType { Id = 4, Value = Claims.ViewCompany, Name = "Company. View" }
        );
    }
}