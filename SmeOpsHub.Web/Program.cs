using Microsoft.EntityFrameworkCore;
using SmeOpsHub.Infrastructure.Persistence;
using SmeOpsHub.Web.Infrastructure.Modules;
using SmeOpsHub.Web.Infrastructure.Navigation;

var builder = WebApplication.CreateBuilder(args);


var modules = ModuleLoader.DiscoverModules();

var mvcBuilder = builder.Services.AddControllersWithViews();

foreach(var module in modules)
{
    mvcBuilder.AddApplicationPart(module.GetType().Assembly);
}

builder.Services.AddDbContext<AppDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("SmeOpsHub") ?? throw new InvalidOperationException("Connection string 'SmeOpsHub' not found");

    options.UseSqlServer(connectionString);
});

var menuBuilder = new MenuBuilder();

foreach(var module in modules)
{
    module.RegisterServices(builder.Services, builder.Configuration);
    module.ConfigureMenu(menuBuilder);
}

builder.Services.AddSingleton<IModuleCatalog>(new ModuleCatalog(modules, menuBuilder.Items));
builder.Services.AddSingleton(modules);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

var moduleCatalog = app.Services.GetRequiredService<IModuleCatalog>();

foreach(var module in moduleCatalog.Modules)
{
    module.MapEndpoints(app);
}

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
