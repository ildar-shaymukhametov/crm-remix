using CRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using static CRM.Application.Constants;

namespace CRM.Infrastructure.Persistence;

public static class ApplicationDbContextSeed
{
    public static void Seed(this ModelBuilder builder)
    {
        var id = 1;
        builder.Entity<UserClaimType>().HasData(
            new UserClaimType { Id = id++, Value = Claims.Company.Create, Name = Claims.Company.Create, },
            new UserClaimType { Id = id++, Value = Claims.Company.Any.Other.Update, Name = Claims.Company.Any.Other.Update },
            new UserClaimType { Id = id++, Value = Claims.Company.Any.Delete, Name = Claims.Company.Any.Delete },
            new UserClaimType { Id = id++, Value = Claims.Company.Any.Other.View, Name = Claims.Company.Any.Other.View },
            new UserClaimType { Id = id++, Value = Claims.Company.WhereUserIsManager.Other.Update, Name = Claims.Company.WhereUserIsManager.Other.Update },
            new UserClaimType { Id = id++, Value = Claims.Company.WhereUserIsManager.Delete, Name = Claims.Company.WhereUserIsManager.Delete },
            new UserClaimType { Id = id++, Value = Claims.Company.WhereUserIsManager.Other.View, Name = Claims.Company.WhereUserIsManager.Other.View },
            new UserClaimType { Id = id++, Value = Claims.Company.Any.Manager.SetFromAnyToAny, Name = Claims.Company.Any.Manager.SetFromAnyToAny },
            new UserClaimType { Id = id++, Value = Claims.Company.Any.Manager.SetFromAnyToSelf, Name = Claims.Company.Any.Manager.SetFromAnyToSelf },
            new UserClaimType { Id = id++, Value = Claims.Company.Any.Manager.SetFromNoneToSelf, Name = Claims.Company.Any.Manager.SetFromNoneToSelf },
            new UserClaimType { Id = id++, Value = Claims.Company.Any.Manager.SetFromNoneToAny, Name = Claims.Company.Any.Manager.SetFromNoneToAny },
            new UserClaimType { Id = id++, Value = Claims.Company.Any.Manager.SetFromAnyToNone, Name = Claims.Company.Any.Manager.SetFromAnyToNone },
            new UserClaimType { Id = id++, Value = Claims.Company.Any.Manager.SetFromSelfToAny, Name = Claims.Company.Any.Manager.SetFromSelfToAny },
            new UserClaimType { Id = id++, Value = Claims.Company.Any.Manager.SetFromSelfToNone, Name = Claims.Company.Any.Manager.SetFromSelfToNone }
        );

        builder.Entity<CompanyType>().HasData(
            new CompanyType { Id = 1, Name = "ООО" },
            new CompanyType { Id = 2, Name = "АО" },
            new CompanyType { Id = 3, Name = "ПАО" },
            new CompanyType { Id = 4, Name = "ИП" }
        );
    }
}