using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmeOpsHub.Modules.Projects.Domain;

namespace SmeOpsHub.Modules.Projects.Persistence.Configurations;

public class ProjectTaskConfiguration : IEntityTypeConfiguration<ProjectTask>
{
    public void Configure(EntityTypeBuilder<ProjectTask> builder)
    {
        builder.ToTable("ProjectTasks", "projects");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title).HasMaxLength(250).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(2000);

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Priority)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.DueDate).HasConversion<DateOnlyConverter, DateOnlyComparer>();

        builder.HasIndex(x => x.ProjectId);
        builder.HasIndex(x => x.DueDate);
        builder.HasIndex(x => x.AssignedToContactId);
        builder.HasIndex(x => x.IsDeleted);
    }
}
