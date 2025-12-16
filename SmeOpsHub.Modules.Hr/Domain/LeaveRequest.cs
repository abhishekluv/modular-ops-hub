using SmeOpsHub.SharedKernel;

namespace SmeOpsHub.Modules.Hr.Domain;

public class LeaveRequest : SoftDeletableEntity<Guid>
{
    public Guid EmployeeId { get; private set; }
    public Employee Employee { get; private set; } = null!;

    public LeaveType Type { get; private set; }
    public LeaveStatus Status { get; private set; }

    public DateOnly StartDate { get; private set; }
    public DateOnly EndDate { get; private set; }

    public string? Reason { get; private set; }
    public DateTimeOffset RequestedAt { get; private set; }

    public DateTimeOffset? DecisionAt { get; private set; }
    public string? DecisionBy { get; private set; }
    public string? DecisionNote { get; private set; }

    private LeaveRequest() { }

    private LeaveRequest(Guid id, Guid employeeId, LeaveType type, DateOnly start, DateOnly end, string? reason)
        : base(id)
    {
        EmployeeId = employeeId;
        Type = type;
        Status = LeaveStatus.Pending;
        StartDate = start;
        EndDate = end;
        Reason = reason?.Trim();
        RequestedAt = DateTimeOffset.UtcNow;
    }

    public static LeaveRequest Create(Guid employeeId, LeaveType type, DateOnly start, DateOnly end, string? reason)
        => new(Guid.NewGuid(), employeeId, type, start, end, reason);

    public void Approve(string? approvedBy, string? note = null)
    {
        if (Status != LeaveStatus.Pending)
            throw new InvalidOperationException("Only pending leave requests can be approved.");

        Status = LeaveStatus.Approved;
        DecisionAt = DateTimeOffset.UtcNow;
        DecisionBy = approvedBy;
        DecisionNote = note?.Trim();
    }

    public void Reject(string? rejectedBy, string note)
    {
        if (Status != LeaveStatus.Pending)
            throw new InvalidOperationException("Only pending leave requests can be rejected.");

        if (string.IsNullOrWhiteSpace(note))
            throw new InvalidOperationException("Rejection note is required.");

        Status = LeaveStatus.Rejected;
        DecisionAt = DateTimeOffset.UtcNow;
        DecisionBy = rejectedBy;
        DecisionNote = note.Trim();
    }
}
