using SmeOpsHub.Modules.Crm.Domain;
using System.ComponentModel.DataAnnotations;

namespace SmeOpsHub.Modules.Crm.Application.Models;

public class CompanyUpsertVm
{
    [Required, StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [StringLength(300)]
    public string? Website { get; set; }

    [Required]
    public LeadStage LeadStage { get; set; } = LeadStage.New;
}
