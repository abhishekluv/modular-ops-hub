using SmeOpsHub.Modules.Hr.Application.Models;

namespace SmeOpsHub.Modules.Hr.Application;

public interface ILeaveService
{
    Task<IReadOnlyList<LeaveRequestListItemVm>> GetAllAsync(Guid? employeeId = null, bool includeDeleted = false, CancellationToken ct = default);

    Task<Guid> CreateAsync(LeaveRequestCreateVm vm, CancellationToken ct = default);

    Task<bool> ApproveAsync(Guid id, string? note, CancellationToken ct = default);
    Task<bool> RejectAsync(Guid id, string note, CancellationToken ct = default);

    Task<bool> SoftDeleteAsync(Guid id, CancellationToken ct = default);
    Task<bool> RestoreAsync(Guid id, CancellationToken ct = default);
}
