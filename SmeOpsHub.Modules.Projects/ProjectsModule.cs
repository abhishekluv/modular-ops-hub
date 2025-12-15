using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmeOpsHub.Modules.Projects.Application;
using SmeOpsHub.Modules.Projects.Application.Services;
using SmeOpsHub.SharedKernel;

namespace SmeOpsHub.Modules.Projects;

public class ProjectsModule : IModule
{
    public string Name => "Projects & Tasks";
    public int Order => 200;

    public void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IProjectService, ProjectService>();
        services.AddScoped<ITaskService, TaskService>();
    }

    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapAreaControllerRoute(
            name: "projects-area",
            areaName: "Projects",
            pattern: "projects/{controller=Projects}/{action=Index}/{id?}");
    }

    public void ConfigureMenu(IMenuBuilder menu)
    {
        menu.AddMenuItem(new MenuItem(
            key: "projects",
            displayName: "Projects",
            url: "/projects/projects",
            order: Order));
    }
}
