using SmeOpsHub.SharedKernel;

namespace SmeOpsHub.Modules.Projects.Domain;

public class ProjectTask : SoftDeletableEntity<Guid>
{
    public Guid ProjectId { get; private set; }
    public Project Project { get; private set; } = null!;

    public string Title { get; private set; } = null!;
    public string? Description { get; private set; }
    public TaskStatus Status { get; private set; }
    public TaskPriority Priority { get; private set; }
    public DateOnly? DueDate { get; private set; }

    // Cross-module reference (CRM ContactId) without coupling
    public Guid? AssignedToContactId { get; private set; }

    private ProjectTask() { }

    private ProjectTask(Guid id, Guid projectId, string title, string? description, TaskStatus status,
        TaskPriority priority, DateOnly? dueDate, Guid? assignedToContactId) : base(id)
    {
        ProjectId = projectId;
        Title = title.Trim();
        Description = description?.Trim();
        Status = status;
        Priority = priority;
        DueDate = dueDate;
        AssignedToContactId = assignedToContactId;
    }

    public static ProjectTask Create(Guid projectId, string title, string? description, TaskStatus status,
        TaskPriority priority, DateOnly? dueDate, Guid? assignedToContactId)
        => new(Guid.NewGuid(), projectId, title, description, status, priority, dueDate, assignedToContactId);

    public void Update(string title, string? description, TaskStatus status, TaskPriority priority, DateOnly? dueDate, Guid? assignedToContactId)
    {
        Title = title.Trim();
        Description = description?.Trim();
        Status = status;
        Priority = priority;
        DueDate = dueDate;
        AssignedToContactId = assignedToContactId;
    }
}
