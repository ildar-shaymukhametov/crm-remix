using CRM.Domain.Entities;
using CRM.Infrastructure.Identity;
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

public class AspNetUserConfiguration : IEntityTypeConfiguration<AspNetUser>
{
    public void Configure(EntityTypeBuilder<AspNetUser> builder)
    {
        builder.HasOne(x => x.ApplicationUser)
            .WithOne()
            .HasForeignKey("ApplicationUser", "Id");
    }
}
