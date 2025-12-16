using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmeOpsHub.SharedKernel.Auditing;

namespace SmeOpsHub.Infrastructure.Persistence.Configurations;

public class AuditEventConfiguration : IEntityTypeConfiguration<AuditEvent>
{
    public void Configure(EntityTypeBuilder<AuditEvent> builder)
    {
        builder.ToTable("AuditEvents", "audit");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Action).HasMaxLength(80).IsRequired();
        builder.Property(x => x.EntityType).HasMaxLength(120).IsRequired();
        builder.Property(x => x.EntityId).HasMaxLength(80).IsRequired();

        builder.Property(x => x.Module).HasMaxLength(50);
        builder.Property(x => x.Summary).HasMaxLength(500);
        builder.Property(x => x.UserId).HasMaxLength(100);
        builder.Property(x => x.UserName).HasMaxLength(256);
        builder.Property(x => x.TraceId).HasMaxLength(100);

        builder.HasIndex(x => x.OccurredAtUtc);
        builder.HasIndex(x => x.Action);
        builder.HasIndex(x => new { x.EntityType, x.EntityId });
    }
}
