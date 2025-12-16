namespace SmeOpsHub.Modules.Hr.Application.Models;

public record EmployeeListItemVm(Guid Id, string FullName, string Email, string? Department, DateOnly JoinDate, bool IsDeleted);
