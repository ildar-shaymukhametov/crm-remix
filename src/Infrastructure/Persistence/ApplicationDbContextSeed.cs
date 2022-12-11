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
            new UserClaimType { Id = 4, Value = Claims.ViewCompany, Name = "Company. View" },
            new UserClaimType { Id = 5, Value = Claims.ViewAnyCompany, Name = "Company. View any" },
            new UserClaimType { Id = 6, Value = Claims.DeleteAnyCompany, Name = "Company. Delete any" },
            new UserClaimType { Id = 7, Value = Claims.UpdateAnyCompany, Name = "Company. Update any" },
            new UserClaimType { Id = 8, Value = Claims.Company.Any.Manager.Any.Set.Any, Name = "Company.Any.Manager.Any.Set.Any" },
            new UserClaimType { Id = 9, Value = Claims.Company.Any.Manager.Any.Set.Self, Name = "Company.Any.Manager.Any.Set.Self" },
            new UserClaimType { Id = 10, Value = Claims.Company.Any.Manager.None.Set.Self, Name = "Company.Any.Manager.None.Set.Self" }
        );
    }
}