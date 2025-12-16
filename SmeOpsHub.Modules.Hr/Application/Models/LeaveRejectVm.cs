using System.ComponentModel.DataAnnotations;

namespace SmeOpsHub.Modules.Hr.Application.Models;

public class LeaveRejectVm
{
    [Required]
    public Guid Id { get; set; }

    [Required, StringLength(500)]
    public string Note { get; set; } = string.Empty;

    public Guid? ReturnEmployeeId { get; set; }
}
