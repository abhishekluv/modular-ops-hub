using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmeOpsHub.SharedKernel;

namespace SmeOpsHub.Modules.Crm;

public class CrmModule : IModule
{
    public string Name => "CRM";
    public int Order => 100;

    public void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        // Later: register CRM DbContext, services, etc.
    }

    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        // Area routing for CRM module (controllers will come later)
        endpoints.MapAreaControllerRoute(
            name: "crm-area",
            areaName: "CRM",
            pattern: "crm/{controller=Home}/{action=Index}/{id?}");
    }

    public void ConfigureMenu(IMenuBuilder menu)
    {
        menu.AddMenuItem(new MenuItem(
            key: "crm",
            displayName: "CRM",
            url: "/crm",
            order: Order));
    }
}
