using SmeOpsHub.Modules.Hr.Application.Models;

namespace SmeOpsHub.Modules.Hr.Application;

public interface IEmployeeService
{
    Task<IReadOnlyList<EmployeeListItemVm>> GetAllAsync(bool includeDeleted = false, CancellationToken ct = default);
    Task<IReadOnlyList<EmployeeLookupVm>> GetActiveLookupAsync(CancellationToken ct = default);

    Task<Guid> CreateAsync(EmployeeUpsertVm vm, CancellationToken ct = default);
    Task<EmployeeUpsertVm?> GetForEditAsync(Guid id, bool includeDeleted = false, CancellationToken ct = default);
    Task<bool> UpdateAsync(Guid id, EmployeeUpsertVm vm, CancellationToken ct = default);

    Task<bool> SoftDeleteAsync(Guid id, CancellationToken ct = default);
    Task<bool> RestoreAsync(Guid id, CancellationToken ct = default);
}
