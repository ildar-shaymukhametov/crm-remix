using CRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitecture.Infrastructure.Persistence.Configurations;

public class CompanyTypeConfiguration : IEntityTypeConfiguration<CompanyType>
{
    public void Configure(EntityTypeBuilder<CompanyType> builder)
    {
        builder.Property(t => t.Name)
            .HasMaxLength(10)
            .IsRequired();
    }
}
