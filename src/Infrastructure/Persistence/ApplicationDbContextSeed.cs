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
            new UserClaimType { Id = id++, Value = Claims.Company.Any.Update, Name = Claims.Company.Any.Update },
            new UserClaimType { Id = id++, Value = Claims.Company.Any.Delete, Name = Claims.Company.Any.Delete },
            new UserClaimType { Id = id++, Value = Claims.Company.Any.View, Name = Claims.Company.Any.View },
            new UserClaimType { Id = id++, Value = Claims.Company.WhereUserIsManager.Update, Name = Claims.Company.WhereUserIsManager.Update },
            new UserClaimType { Id = id++, Value = Claims.Company.WhereUserIsManager.Delete, Name = Claims.Company.WhereUserIsManager.Delete },
            new UserClaimType { Id = id++, Value = Claims.Company.WhereUserIsManager.View, Name = Claims.Company.WhereUserIsManager.View },
            new UserClaimType { Id = id++, Value = Claims.Company.Any.SetManagerFromAnyToAny, Name = Claims.Company.Any.SetManagerFromAnyToAny },
            new UserClaimType { Id = id++, Value = Claims.Company.Any.SetManagerFromAnyToSelf, Name = Claims.Company.Any.SetManagerFromAnyToSelf },
            new UserClaimType { Id = id++, Value = Claims.Company.Any.SetManagerFromNoneToSelf, Name = Claims.Company.Any.SetManagerFromNoneToSelf },
            new UserClaimType { Id = id++, Value = Claims.Company.Any.SetManagerFromNoneToAny, Name = Claims.Company.Any.SetManagerFromNoneToAny },
            new UserClaimType { Id = id++, Value = Claims.Company.WhereUserIsManager.SetManagerFromSelfToAny, Name = Claims.Company.WhereUserIsManager.SetManagerFromSelfToAny },
            new UserClaimType { Id = id++, Value = Claims.Company.Any.SetManagerFromAnyToNone, Name = Claims.Company.Any.SetManagerFromAnyToNone },
            new UserClaimType { Id = id++, Value = Claims.Company.Any.SetManagerFromSelfToAny, Name = Claims.Company.Any.SetManagerFromSelfToAny },
            new UserClaimType { Id = id++, Value = Claims.Company.Any.SetManagerFromSelfToNone, Name = Claims.Company.Any.SetManagerFromSelfToNone }
        );
    }
}