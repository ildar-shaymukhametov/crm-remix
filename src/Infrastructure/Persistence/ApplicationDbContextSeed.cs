using CRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using static CRM.Domain.Constants;

namespace CRM.Infrastructure.Persistence;

public static class ApplicationDbContextSeed
{
    public static void Seed(this ModelBuilder builder)
    {
        var id = 1;
        builder.Entity<UserClaimType>().HasData(
            new UserClaimType { Id = id++, Value = Claims.Company.Create, Name = Claims.Company.Create, },
            new UserClaimType { Id = id++, Value = Claims.Company.New.Other.Set, Name = Claims.Company.New.Other.Set, },
            new UserClaimType { Id = id++, Value = Claims.Company.New.Manager.SetToAny, Name = Claims.Company.New.Manager.SetToAny, },
            new UserClaimType { Id = id++, Value = Claims.Company.New.Manager.SetToNone, Name = Claims.Company.New.Manager.SetToNone, },
            new UserClaimType { Id = id++, Value = Claims.Company.New.Manager.SetToSelf, Name = Claims.Company.New.Manager.SetToSelf, },
            new UserClaimType { Id = id++, Value = Claims.Company.Any.Delete, Name = Claims.Company.Any.Delete },
            new UserClaimType { Id = id++, Value = Claims.Company.Any.Other.Get, Name = Claims.Company.Any.Other.Get },
            new UserClaimType { Id = id++, Value = Claims.Company.Any.Other.Set, Name = Claims.Company.Any.Other.Set },
            new UserClaimType { Id = id++, Value = Claims.Company.Any.Name.Get, Name = Claims.Company.Any.Name.Get },
            new UserClaimType { Id = id++, Value = Claims.Company.Any.Name.Set, Name = Claims.Company.Any.Name.Set },
            new UserClaimType { Id = id++, Value = Claims.Company.Any.Manager.Get, Name = Claims.Company.Any.Manager.Get },
            new UserClaimType { Id = id++, Value = Claims.Company.Any.Manager.SetFromAnyToAny, Name = Claims.Company.Any.Manager.SetFromAnyToAny },
            new UserClaimType { Id = id++, Value = Claims.Company.Any.Manager.SetFromAnyToSelf, Name = Claims.Company.Any.Manager.SetFromAnyToSelf },
            new UserClaimType { Id = id++, Value = Claims.Company.Any.Manager.SetFromNoneToSelf, Name = Claims.Company.Any.Manager.SetFromNoneToSelf },
            new UserClaimType { Id = id++, Value = Claims.Company.Any.Manager.SetFromNoneToAny, Name = Claims.Company.Any.Manager.SetFromNoneToAny },
            new UserClaimType { Id = id++, Value = Claims.Company.Any.Manager.SetFromAnyToNone, Name = Claims.Company.Any.Manager.SetFromAnyToNone },
            new UserClaimType { Id = id++, Value = Claims.Company.Any.Manager.SetFromSelfToAny, Name = Claims.Company.Any.Manager.SetFromSelfToAny },
            new UserClaimType { Id = id++, Value = Claims.Company.Any.Manager.SetFromSelfToNone, Name = Claims.Company.Any.Manager.SetFromSelfToNone },
            new UserClaimType { Id = id++, Value = Claims.Company.WhereUserIsManager.Delete, Name = Claims.Company.WhereUserIsManager.Delete },
            new UserClaimType { Id = id++, Value = Claims.Company.WhereUserIsManager.Other.Get, Name = Claims.Company.WhereUserIsManager.Other.Get },
            new UserClaimType { Id = id++, Value = Claims.Company.WhereUserIsManager.Other.Set, Name = Claims.Company.WhereUserIsManager.Other.Set },
            new UserClaimType { Id = id++, Value = Claims.Company.WhereUserIsManager.Name.Get, Name = Claims.Company.WhereUserIsManager.Name.Get },
            new UserClaimType { Id = id++, Value = Claims.Company.WhereUserIsManager.Name.Set, Name = Claims.Company.WhereUserIsManager.Name.Set },
            new UserClaimType { Id = id++, Value = Claims.Company.WhereUserIsManager.Manager.Get, Name = Claims.Company.WhereUserIsManager.Manager.Get },
            new UserClaimType { Id = id++, Value = Claims.Company.WhereUserIsManager.Manager.SetFromSelfToAny, Name = Claims.Company.WhereUserIsManager.Manager.SetFromSelfToAny },
            new UserClaimType { Id = id++, Value = Claims.Company.WhereUserIsManager.Manager.SetFromSelfToNone, Name = Claims.Company.WhereUserIsManager.Manager.SetFromSelfToNone }
        );

        builder.Entity<CompanyType>().HasData(
            new CompanyType { Id = 1, Name = "ООО" },
            new CompanyType { Id = 2, Name = "АО" },
            new CompanyType { Id = 3, Name = "ПАО" },
            new CompanyType { Id = 4, Name = "ИП" }
        );
    }
}