using Microsoft.AspNetCore.Identity;

namespace SmeOpsHub.Infrastructure.Identity;

public class ApplicationUser : IdentityUser
{
    public string? DisplayName { get; set; }
}
