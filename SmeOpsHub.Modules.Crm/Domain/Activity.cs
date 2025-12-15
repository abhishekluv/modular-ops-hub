using SmeOpsHub.SharedKernel;

namespace SmeOpsHub.Modules.Crm.Domain;

public class Activity : SoftDeletableEntity<Guid>
{
    public Guid ContactId { get; private set; }
    public Contact Contact { get; private set; } = null!;

    public ActivityType Type { get; private set; }
    public DateTimeOffset OccurredAt { get; private set; }
    public string Summary { get; private set; } = null!;

    private Activity() { } // EF

    private Activity(Guid id, Guid contactId, ActivityType type, DateTimeOffset occurredAt, string summary) : base(id)
    {
        ContactId = contactId;
        Type = type;
        OccurredAt = occurredAt;
        Summary = summary.Trim();
    }

    public static Activity Create(Guid contactId, ActivityType type, DateTimeOffset occurredAt, string summary)
        => new(Guid.NewGuid(), contactId, type, occurredAt, summary);
}
