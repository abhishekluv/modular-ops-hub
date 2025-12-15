using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmeOpsHub.SharedKernel;

namespace SmeOpsHub.Modules.Projects;

public class ProjectsModule : IModule
{
    public string Name => "Projects & Tasks";
    public int Order => 200;

    public void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        // Later: register Projects services, etc.
    }

    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapAreaControllerRoute(
            name: "projects-area",
            areaName: "Projects",
            pattern: "projects/{controller=Home}/{action=Index}/{id?}");
    }

    public void ConfigureMenu(IMenuBuilder menu)
    {
        menu.AddMenuItem(new MenuItem(
            key: "projects",
            displayName: "Projects",
            url: "/projects",
            order: Order));
    }
}
