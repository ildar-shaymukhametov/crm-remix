using CRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitecture.Infrastructure.Persistence.Configurations;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.Property(t => t.FirstName)
            .HasMaxLength(256);

        builder.Property(t => t.LastName)
            .HasMaxLength(256);

        builder.Property(t => t.Email)
            .HasMaxLength(256);

        builder.Property(t => t.PhoneNumber)
            .HasMaxLength(256);
    }
}
