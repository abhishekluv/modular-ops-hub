using SmeOpsHub.SharedKernel;

namespace SmeOpsHub.Modules.Projects.Domain;

public class Project : SoftDeletableEntity<Guid>, IAggregateRoot
{
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public ProjectStatus Status { get; private set; }

    public DateOnly? StartDate { get; private set; }
    public DateOnly? EndDate { get; private set; }

    public List<ProjectTask> Tasks { get; private set; } = new();

    private Project() { }

    private Project(Guid id, string name, string? description, ProjectStatus status, DateOnly? start, DateOnly? end)
        : base(id)
    {
        Name = name.Trim();
        Description = description?.Trim();
        Status = status;
        StartDate = start;
        EndDate = end;
    }

    public static Project Create(string name, string? description, ProjectStatus status, DateOnly? start, DateOnly? end)
        => new(Guid.NewGuid(), name, description, status, start, end);

    public void Update(string name, string? description, ProjectStatus status, DateOnly? start, DateOnly? end)
    {
        Name = name.Trim();
        Description = description?.Trim();
        Status = status;
        StartDate = start;
        EndDate = end;
    }
}
