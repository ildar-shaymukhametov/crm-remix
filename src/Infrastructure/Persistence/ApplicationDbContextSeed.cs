using CRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using static CRM.Application.Constants;

namespace CRM.Infrastructure.Persistence;

public static class ApplicationDbContextSeed
{
    public static void Seed(this ModelBuilder builder)
    {
        builder.Entity<UserClaimType>().HasData(
            new UserClaimType { Id = 1, Value = Claims.Company.Create, Name = Claims.Company.Create, },
            new UserClaimType { Id = 2, Value = Claims.Company.Any.Update, Name = Claims.Company.Any.Update },
            new UserClaimType { Id = 3, Value = Claims.Company.Any.Delete, Name = Claims.Company.Any.Delete },
            new UserClaimType { Id = 4, Value = Claims.Company.Any.View, Name = Claims.Company.Any.View },
            new UserClaimType { Id = 5, Value = Claims.Company.WhereUserIsManager.Update, Name = Claims.Company.WhereUserIsManager.Update },
            new UserClaimType { Id = 6, Value = Claims.Company.WhereUserIsManager.Delete, Name = Claims.Company.WhereUserIsManager.Delete },
            new UserClaimType { Id = 7, Value = Claims.Company.WhereUserIsManager.View, Name = Claims.Company.WhereUserIsManager.View },
            new UserClaimType { Id = 8, Value = Claims.Company.Any.SetManagerFromAnyToAny, Name = Claims.Company.Any.SetManagerFromAnyToAny },
            new UserClaimType { Id = 9, Value = Claims.Company.Any.SetManagerFromAnyToSelf, Name = Claims.Company.Any.SetManagerFromAnyToSelf },
            new UserClaimType { Id = 10, Value = Claims.Company.Any.SetManagerFromNoneToSelf, Name = Claims.Company.Any.SetManagerFromNoneToSelf },
            new UserClaimType { Id = 11, Value = Claims.Company.Any.SetManagerFromNoneToAny, Name = Claims.Company.Any.SetManagerFromNoneToAny },
            new UserClaimType { Id = 12, Value = Claims.Company.WhereUserIsManager.SetManagerFromSelfToAny, Name = Claims.Company.WhereUserIsManager.SetManagerFromSelfToAny }
        );
    }
}