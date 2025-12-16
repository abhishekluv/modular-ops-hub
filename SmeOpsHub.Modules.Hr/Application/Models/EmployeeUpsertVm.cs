using System.ComponentModel.DataAnnotations;

namespace SmeOpsHub.Modules.Hr.Application.Models;

public class EmployeeUpsertVm
{
    [Required, StringLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required, StringLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required, StringLength(320), EmailAddress]
    public string Email { get; set; } = string.Empty;

    [StringLength(120)]
    public string? Department { get; set; }

    [Required]
    public DateOnly JoinDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);
}
