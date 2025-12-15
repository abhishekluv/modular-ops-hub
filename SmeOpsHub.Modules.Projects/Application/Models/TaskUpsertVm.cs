using System.ComponentModel.DataAnnotations;
using SmeOpsHub.Modules.Projects.Domain;
using TaskStatus = SmeOpsHub.Modules.Projects.Domain.TaskStatus;

namespace SmeOpsHub.Modules.Projects.Application.Models;

public class TaskUpsertVm
{
    [Required, StringLength(250)]
    public string Title { get; set; } = string.Empty;

    [StringLength(2000)]
    public string? Description { get; set; }

    [Required]
    public TaskStatus Status { get; set; } = TaskStatus.Todo;

    [Required]
    public TaskPriority Priority { get; set; } = TaskPriority.Medium;

    public DateOnly? DueDate { get; set; }

    public Guid? AssignedToContactId { get; set; }
}
