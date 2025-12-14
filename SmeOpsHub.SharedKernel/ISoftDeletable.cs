namespace SmeOpsHub.SharedKernel;

public interface ISoftDeletable
{
    bool IsDeleted { get; }
    DateTimeOffset? DeletedAt { get; }
    string? DeletedBy { get; }

    void SoftDelete(string? deletedBy);
    void Restore();
}
