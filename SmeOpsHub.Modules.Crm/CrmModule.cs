using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmeOpsHub.Modules.Crm.Application;
using SmeOpsHub.Modules.Crm.Application.Services;
using SmeOpsHub.SharedKernel;

namespace SmeOpsHub.Modules.Crm;

public class CrmModule : IModule
{
    public string Name => "CRM";
    public int Order => 100;

    public void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ICompanyService, CompanyService>();
    }

    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        // Area routing for CRM module (controllers will come later)
        endpoints.MapAreaControllerRoute(
            name: "crm-area",
            areaName: "CRM",
            pattern: "crm/{controller=Companies}/{action=Index}/{id?}");
    }

    public void ConfigureMenu(IMenuBuilder menu)
    {
        menu.AddMenuItem(new MenuItem(
            key: "crm",
            displayName: "CRM",
            url: "/crm/companies",
            order: Order));
    }
}
