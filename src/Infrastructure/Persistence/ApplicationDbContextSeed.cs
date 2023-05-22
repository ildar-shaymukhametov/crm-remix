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
            new UserClaimType { Id = id++, Value = Claims.Company.Old.Any.Update, Name = Claims.Company.Old.Any.Update },
            new UserClaimType { Id = id++, Value = Claims.Company.Old.Any.Delete, Name = Claims.Company.Old.Any.Delete },
            new UserClaimType { Id = id++, Value = Claims.Company.Old.Any.View, Name = Claims.Company.Old.Any.View },
            new UserClaimType { Id = id++, Value = Claims.Company.Old.WhereUserIsManager.Update, Name = Claims.Company.Old.WhereUserIsManager.Update },
            new UserClaimType { Id = id++, Value = Claims.Company.Old.WhereUserIsManager.Delete, Name = Claims.Company.Old.WhereUserIsManager.Delete },
            new UserClaimType { Id = id++, Value = Claims.Company.Old.WhereUserIsManager.View, Name = Claims.Company.Old.WhereUserIsManager.View },
            new UserClaimType { Id = id++, Value = Claims.Company.Old.Any.SetManagerFromAnyToAny, Name = Claims.Company.Old.Any.SetManagerFromAnyToAny },
            new UserClaimType { Id = id++, Value = Claims.Company.Old.Any.SetManagerFromAnyToSelf, Name = Claims.Company.Old.Any.SetManagerFromAnyToSelf },
            new UserClaimType { Id = id++, Value = Claims.Company.Old.Any.SetManagerFromNoneToSelf, Name = Claims.Company.Old.Any.SetManagerFromNoneToSelf },
            new UserClaimType { Id = id++, Value = Claims.Company.Old.Any.SetManagerFromNoneToAny, Name = Claims.Company.Old.Any.SetManagerFromNoneToAny },
            new UserClaimType { Id = id++, Value = Claims.Company.Old.WhereUserIsManager.SetManagerFromSelfToAny, Name = Claims.Company.Old.WhereUserIsManager.SetManagerFromSelfToAny },
            new UserClaimType { Id = id++, Value = Claims.Company.Old.Any.SetManagerFromAnyToNone, Name = Claims.Company.Old.Any.SetManagerFromAnyToNone },
            new UserClaimType { Id = id++, Value = Claims.Company.Old.Any.SetManagerFromSelfToAny, Name = Claims.Company.Old.Any.SetManagerFromSelfToAny },
            new UserClaimType { Id = id++, Value = Claims.Company.Old.Any.SetManagerFromSelfToNone, Name = Claims.Company.Old.Any.SetManagerFromSelfToNone },
            new UserClaimType { Id = id++, Value = Claims.Company.New.SetManagerToAny, Name = Claims.Company.New.SetManagerToAny },
            new UserClaimType { Id = id++, Value = Claims.Company.New.SetManagerToSelf, Name = Claims.Company.New.SetManagerToSelf }
        );
    }
}