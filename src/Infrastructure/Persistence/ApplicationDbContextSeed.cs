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
            new UserClaimType { Id = 8, Value = Claims.SetManagerToAnyFromAny, Name = "Company. Manager. Assign anyone from anyone" },
            new UserClaimType { Id = 9, Value = Claims.SetManagerToAnyFromNone, Name = "Company. Manager. Assign anyone from none" },
            new UserClaimType { Id = 10, Value = Claims.SetManagerToAnyFromSelf, Name = "Company. Manager. Assign anyone from self" },
            new UserClaimType { Id = 11, Value = Claims.SetManagerToNoneFromAny, Name = "Company. Manager. Assign none from anyone" },
            new UserClaimType { Id = 12, Value = Claims.SetManagerToNoneFromSelf, Name = "Company. Manager. Assign none from self" },
            new UserClaimType { Id = 13, Value = Claims.SetManagerToSelfFromAny, Name = "Company. Manager. Assign self from anyone" },
            new UserClaimType { Id = 14, Value = Claims.SetManagerToSelfFromNone, Name = "Company. Manager. Assign self from none" }
        );
    }
}