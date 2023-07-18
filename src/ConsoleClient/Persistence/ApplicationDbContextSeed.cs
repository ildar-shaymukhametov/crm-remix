using CRM.Domain;
using CRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ConsoleClient.Persistence;

public static class ApplicationDbContextSeed
{
    public static void Seed(this ModelBuilder builder)
    {
        builder.Entity<UserClaimType>().HasData(DataSeed.GetUserClaimTypes());
        builder.Entity<CompanyType>().HasData(DataSeed.GetCompanyTypes());
    }
}