using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmeOpsHub.Modules.Crm.Domain;

namespace SmeOpsHub.Modules.Crm.Persistence.Configurations;

public class ContactConfiguration : IEntityTypeConfiguration<Contact>
{
    public void Configure(EntityTypeBuilder<Contact> builder)
    {
        builder.ToTable("Contacts", "crm");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.FullName).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Email).HasMaxLength(320);
        builder.Property(x => x.Phone).HasMaxLength(30);

        builder.HasIndex(x => x.CompanyId);
        builder.HasIndex(x => x.Email);
        builder.HasIndex(x => x.IsDeleted);

        builder.HasMany(x => x.Activities)
            .WithOne(x => x.Contact)
            .HasForeignKey(x => x.ContactId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
