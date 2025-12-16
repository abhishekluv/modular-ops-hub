using SmeOpsHub.Modules.Hr.Domain;

namespace SmeOpsHub.Modules.Hr.Application.Models;

public record LeaveRequestListItemVm(
    Guid Id,
    Guid EmployeeId,
    string EmployeeName,
    LeaveType Type,
    LeaveStatus Status,
    DateOnly StartDate,
    DateOnly EndDate,
    string? Reason,
    bool IsDeleted
);
