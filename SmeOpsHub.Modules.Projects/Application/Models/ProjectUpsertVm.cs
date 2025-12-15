using SmeOpsHub.Modules.Projects.Domain;
using System.ComponentModel.DataAnnotations;

namespace SmeOpsHub.Modules.Projects.Application.Models;

public class ProjectUpsertVm
{
    [Required, StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; set; }

    [Required]
    public ProjectStatus Status { get; set; } = ProjectStatus.Planned;

    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
}
