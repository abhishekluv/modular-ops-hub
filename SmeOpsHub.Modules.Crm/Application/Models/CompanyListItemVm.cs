using SmeOpsHub.Modules.Crm.Domain;

namespace SmeOpsHub.Modules.Crm.Application.Models;

public record CompanyListItemVm(Guid Id, string Name, LeadStage LeadStage, int ContactsCount, bool IsDeleted);
