using SmeOpsHub.Modules.Projects.Application.Models;

namespace SmeOpsHub.Modules.Projects.Application;

public interface ITaskService
{
    Task<IReadOnlyList<TaskListItemVm>> GetForProjectAsync(Guid projectId, bool includeDeleted = false, CancellationToken ct = default);

    Task<Guid> CreateAsync(Guid projectId, TaskUpsertVm model, CancellationToken ct = default);
    Task<TaskUpsertVm?> GetForEditAsync(Guid taskId, bool includeDeleted = false, CancellationToken ct = default);
    Task<bool> UpdateAsync(Guid taskId, TaskUpsertVm model, CancellationToken ct = default);

    Task<bool> SoftDeleteAsync(Guid taskId, CancellationToken ct = default);
    Task<bool> RestoreAsync(Guid taskId, CancellationToken ct = default);
}
