using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmeOpsHub.Modules.Hr.Domain;

namespace SmeOpsHub.Modules.Hr.Persistence.Configurations;

public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.ToTable("Employees", "hr");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.FirstName).HasMaxLength(100).IsRequired();
        builder.Property(e => e.LastName).HasMaxLength(100).IsRequired();
        builder.Property(e => e.Email).HasMaxLength(320).IsRequired();
        builder.Property(e => e.Department).HasMaxLength(120);

        builder.Property(e => e.JoinDate)
            .HasConversion<DateOnlyConverter, DateOnlyComparer>()
            .IsRequired();

        // Consultant-style: unique among active (soft-delete friendly)
        builder.HasIndex(e => e.Email)
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");

        builder.HasIndex(e => e.Department);
        builder.HasIndex(e => e.IsDeleted);

        builder.HasMany(e => e.LeaveRequests)
            .WithOne(l => l.Employee)
            .HasForeignKey(l => l.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
