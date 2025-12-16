using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmeOpsHub.Modules.Hr.Application;
using SmeOpsHub.Modules.Hr.Application.Services;
using SmeOpsHub.SharedKernel;

namespace SmeOpsHub.Modules.Hr;

public class HrModule : IModule
{
    public string Name => "HR & Leave";
    public int Order => 300;

    public void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IEmployeeService, EmployeeService>();
        services.AddScoped<ILeaveService, LeaveService>();
    }

    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapAreaControllerRoute(
            name: "hr-area",
            areaName: "HR",
            pattern: "hr/{controller=Employees}/{action=Index}/{id?}");
    }

    public void ConfigureMenu(IMenuBuilder menu)
    {
        menu.AddMenuItem(new MenuItem(
            key: "hr",
            displayName: "HR & Leave",
            url: "/hr/employees",
            order: Order));
    }
}
