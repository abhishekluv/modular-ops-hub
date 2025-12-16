namespace SmeOpsHub.SharedKernel.Auditing;

public class AuditEvent
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public DateTimeOffset OccurredAtUtc { get; set; } = DateTimeOffset.UtcNow;

    public string Action { get; set; } = null!;          // e.g. SOFT_DELETE, RESTORE, LEAVE_APPROVED
    public string EntityType { get; set; } = null!;      // e.g. Company, Project, Employee, LeaveRequest
    public string EntityId { get; set; } = null!;        // string for flexibility (Guid/string keys)

    public string? Module { get; set; }                  // CRM / Projects / HR
    public string? Summary { get; set; }                 // human-friendly
    public string? DataJson { get; set; }                // optional structured JSON

    public string? UserId { get; set; }
    public string? UserName { get; set; }

    public string? TraceId { get; set; }                 // useful for correlation
}
