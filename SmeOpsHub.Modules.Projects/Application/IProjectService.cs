using SmeOpsHub.Modules.Projects.Application.Models;

namespace SmeOpsHub.Modules.Projects.Application;

public interface IProjectService
{
    Task<IReadOnlyList<ProjectListItemVm>> GetAllAsync(bool includeDeleted = false, CancellationToken ct = default);
    Task<Guid> CreateAsync(ProjectUpsertVm model, CancellationToken ct = default);
    Task<ProjectUpsertVm?> GetForEditAsync(Guid id, bool includeDeleted = false, CancellationToken ct = default);
    Task<bool> UpdateAsync(Guid id, ProjectUpsertVm model, CancellationToken ct = default);

    Task<bool> SoftDeleteAsync(Guid id, CancellationToken ct = default);
    Task<bool> RestoreAsync(Guid id, CancellationToken ct = default);
}
