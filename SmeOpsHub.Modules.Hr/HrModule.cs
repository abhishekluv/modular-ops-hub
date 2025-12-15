using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmeOpsHub.SharedKernel;

namespace SmeOpsHub.Modules.Hr;

public class HrModule : IModule
{
    public string Name => "HR & Leave";
    public int Order => 300;

    public void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        // Later: register HR services, etc.
    }

    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapAreaControllerRoute(
            name: "hr-area",
            areaName: "HR",
            pattern: "hr/{controller=Home}/{action=Index}/{id?}");
    }

    public void ConfigureMenu(IMenuBuilder menu)
    {
        menu.AddMenuItem(new MenuItem(
            key: "hr",
            displayName: "HR & Leave",
            url: "/hr",
            order: Order));
    }
}
