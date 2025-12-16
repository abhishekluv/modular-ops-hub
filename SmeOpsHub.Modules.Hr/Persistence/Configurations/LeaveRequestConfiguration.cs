using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmeOpsHub.Modules.Hr.Domain;

namespace SmeOpsHub.Modules.Hr.Persistence.Configurations;

public class LeaveRequestConfiguration : IEntityTypeConfiguration<LeaveRequest>
{
    public void Configure(EntityTypeBuilder<LeaveRequest> builder)
    {
        builder.ToTable("LeaveRequests", "hr");
        builder.HasKey(l => l.Id);

        builder.Property(l => l.Type)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(l => l.Status)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(l => l.StartDate)
            .HasConversion<DateOnlyConverter, DateOnlyComparer>()
            .IsRequired();

        builder.Property(l => l.EndDate)
            .HasConversion<DateOnlyConverter, DateOnlyComparer>()
            .IsRequired();

        builder.Property(l => l.Reason).HasMaxLength(500);
        builder.Property(l => l.DecisionBy).HasMaxLength(100);
        builder.Property(l => l.DecisionNote).HasMaxLength(500);

        builder.HasIndex(l => l.EmployeeId);
        builder.HasIndex(l => l.Status);
        builder.HasIndex(l => l.IsDeleted);
        builder.HasIndex(l => new { l.EmployeeId, l.StartDate, l.EndDate });
    }
}
