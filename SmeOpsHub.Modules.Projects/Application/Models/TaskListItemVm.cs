using SmeOpsHub.Modules.Projects.Domain;
using TaskStatus = SmeOpsHub.Modules.Projects.Domain.TaskStatus;

namespace SmeOpsHub.Modules.Projects.Application.Models;

public record TaskListItemVm(Guid Id, Guid ProjectId, string Title, TaskStatus Status, TaskPriority Priority, DateOnly? DueDate, Guid? AssignedToContactId, string AssignedToDisplay, bool IsDeleted);
