using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmeOpsHub.Modules.Projects.Domain;

namespace SmeOpsHub.Modules.Projects.Persistence.Configurations;

public class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.ToTable("Projects", "projects");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(1000);

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.StartDate).HasConversion<DateOnlyConverter, DateOnlyComparer>();
        builder.Property(x => x.EndDate).HasConversion<DateOnlyConverter, DateOnlyComparer>();

        builder.HasIndex(x => x.Name);
        builder.HasIndex(x => x.IsDeleted);

        builder.HasMany(x => x.Tasks)
            .WithOne(t => t.Project)
            .HasForeignKey(t => t.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
