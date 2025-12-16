using SmeOpsHub.Modules.Hr.Domain;
using System.ComponentModel.DataAnnotations;

namespace SmeOpsHub.Modules.Hr.Application.Models;

public class LeaveRequestCreateVm
{
    [Required]
    public Guid EmployeeId { get; set; }

    [Required]
    public LeaveType Type { get; set; } = LeaveType.Annual;

    [Required]
    public DateOnly StartDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);

    [Required]
    public DateOnly EndDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);

    [StringLength(500)]
    public string? Reason { get; set; }
}
