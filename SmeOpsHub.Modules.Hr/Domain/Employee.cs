using SmeOpsHub.SharedKernel;

namespace SmeOpsHub.Modules.Hr.Domain;

public class Employee : SoftDeletableEntity<Guid>, IAggregateRoot
{
    public string FirstName { get; private set; } = null!;
    public string LastName { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public string? Department { get; private set; }
    public DateOnly JoinDate { get; private set; }

    public List<LeaveRequest> LeaveRequests { get; private set; } = new();

    private Employee() { }

    private Employee(Guid id, string firstName, string lastName, string email, string? department, DateOnly joinDate)
        : base(id)
    {
        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        Email = email.Trim().ToLowerInvariant();
        Department = department?.Trim();
        JoinDate = joinDate;
    }

    public static Employee Create(string firstName, string lastName, string email, string? department, DateOnly joinDate)
        => new(Guid.NewGuid(), firstName, lastName, email, department, joinDate);

    public void Update(string firstName, string lastName, string email, string? department, DateOnly joinDate)
    {
        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        Email = email.Trim().ToLowerInvariant();
        Department = department?.Trim();
        JoinDate = joinDate;
    }
}
