using SmeOpsHub.Modules.Projects.Domain;

namespace SmeOpsHub.Modules.Projects.Application.Models;

public record ProjectListItemVm(Guid Id, string Name, ProjectStatus Status, int TasksCount, bool IsDeleted);
