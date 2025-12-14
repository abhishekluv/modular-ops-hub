namespace SmeOpsHub.SharedKernel;

public interface ICurrentUserService
{
    string? UserId { get; }
    string? UserName { get; }
}
