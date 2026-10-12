using Microsoft.EntityFrameworkCore;
using Serilog;
using Simone.Common.RabbitMQ.Extensions;
using Simone.Common.Security.Extensions;
using Simone.Common.StatusTransitions.Web.Test.Data;
using Simone.Common.Startup;


var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, config) => config.ReadFrom.Configuration(context.Configuration));

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Database")));

builder.CalculateDefaultIdentityOptions<Program>();
builder.AddSimoneAuthorization();

builder.Services.AddControllersWithViews()
    .AddNewtonsoftJson(o => o.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

builder.Services.AddRabbitMQ(builder.Configuration);

builder.Services.AddLocalization(o => o.ResourcesPath = "Resources");


var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseCookiePolicy();

app.UseSimoneAuthorization();

app.UseLocalization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Subcontracts}/{action=Index}/{id?}");

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    logger.LogInformation("Applying database migrations...");
    await db.Database.MigrateAsync();
}

Console.WriteLine("Simone.Common.StatusTransitions.Web.Test is launched!");
await app.RunAsync();
Console.WriteLine("Simone.Common.StatusTransitions.Web.Test is stopped!");
