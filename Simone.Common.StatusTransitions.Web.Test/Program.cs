using Microsoft.EntityFrameworkCore;
using Serilog;
using Simone.Common.RabbitMQ.Extensions;
using Simone.Common.Security.Extensions;
using Simone.Common.StatusTransitions.Web.Test.Data;
using Simone.Common.Startup;
using Simone.Common.StatusTransitions.Extensors;


var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<Microsoft.AspNetCore.Authentication.OpenIdConnect.OpenIdConnectOptions>(
    "OpenIdConnect",
    options =>
    {
        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (sender, certificate, chain, sslPolicyErrors) =>
            {
                if (sslPolicyErrors == System.Net.Security.SslPolicyErrors.None)
                    return true;

                // Acepta certificados emitidos para *.tenco.dev o en desarrollo
                if (certificate != null && (certificate.Subject.Contains("tenco.dev") || builder.Environment.IsDevelopment()))
                {
                    return true;
                }

                return false;
            }
        };

        options.Backchannel = new HttpClient(handler);
    });

builder.Host.UseSerilog((context, config) => config.ReadFrom.Configuration(context.Configuration));

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Database")));

builder.CalculateDefaultIdentityOptions<Program>();
builder.AddSimoneAuthorization();

builder.Services.AddControllersWithViews()
    .AddNewtonsoftJson(o => o.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

builder.Services.AddRabbitMQ(builder.Configuration);

builder.Services.AddLocalization(o => o.ResourcesPath = "Resources");
builder.Services.AddStatusTransitions<AppDbContext>(options =>
{
    options.PolicyBuilder = policy => policy.RequireAuthenticatedUser();
});


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

app.UseStatusTransitions();

app.UseLocalization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Subcontracts}/{action=Index}/{id?}");

if (!EF.IsDesignTime)
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    logger.LogInformation("Applying database migrations...");
    await db.Database.MigrateAsync();
}

Console.WriteLine("Simone.Common.StatusTransitions.Web.Test is launched!");
await app.RunAsync();
Console.WriteLine("Simone.Common.StatusTransitions.Web.Test is stopped!");
