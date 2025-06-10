using Azure.Identity;

using Microsoft.AspNetCore.Authentication;
using Microsoft.Identity.Web.UI;

using SharedLib.Extensions;
using SharedLib.Helpers;
using SharedLib.Models;

using System.Reflection;
using System.Security.Cryptography.X509Certificates;

using TimeSheet.Web.Components;
using TimeSheet.Web.Services;
using TimeSheet.Web.Services.Interfaces;

namespace TimeSheet.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            // Load environment-specific configuration
            ConfigureConfiguration(builder.Configuration, environmentName);

            builder.Services.AddHealthChecks();
            builder.Services.AddHttpContextAccessor();

            var azureAdOptions = builder.Configuration.GetSection("AzureAd").Get<AzureADOptions>();
            var msGraphOptions = builder.Configuration.GetSection("MicrosoftGraph").Get<MSGraphOptions>();
            var connectionString = builder.Configuration[$"PG-SQL-CONNECTION-STRING-{builder.Configuration["AppSettings:ENV"]}"];
            var certPassword = builder.Configuration[$"OPPO-APP-TLS-CERT-PW-{builder.Configuration["AppSettings:ENV"]}"];
            var certPath = "/etc/tls/" + builder.Configuration["OPPO-APP-TLS-CERT-NAME"];
            int httpsPort = builder.Configuration.GetSection("AppSettings").GetValue<int>("ExposedHttpsPort");
            var redisCacheOptions = builder.Configuration.GetSection("Redis").Get<RedisCacheOptions>();
            var baseUrl = builder.Configuration["UrlSettings:BaseUrl"];

            var databaseOptions = new DatabaseOptions
            {
                SharedConnectionString = connectionString
            };

            var loggingOptions = new LoggingOptions
            {
                ApplicationInsightsConnectionString = builder.Configuration["ApplicationInsights:ConnectionString"]
            };

            builder.Services.ConfigureSharedCookie("cloudapp.azure.com");
            builder.Services.AddSharedLibraryServices(
               builder.Configuration,
               loggingOptions,
               redisCacheOptions);
            builder.Services.AddControllersWithViews().AddMicrosoftIdentityUI();

            if (!string.IsNullOrEmpty(certPath) && !string.IsNullOrEmpty(certPassword))
            {
                builder.WebHost.ConfigureKestrel(serverOptions =>
                {
                    try
                    {
                        var certificate = new X509Certificate2(certPath, certPassword);
                        // Use the certificate in your Kestrel configuration
                        serverOptions.ListenAnyIP(httpsPort, listenOptions =>
                        {
                            listenOptions.UseHttps(certificate);
                        });
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error loading certificate: {ex.Message}");
                    }
                });
            }

            var apiBaseUrl = Environment.GetEnvironmentVariable("ApiBaseUrl")
                  ?? builder.Configuration.GetValue<string>("ApiBaseUrl");
            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(apiBaseUrl) });

            builder.Services.AddScoped<ITimesheetService, TimesheetService>();

            var app = builder.Build();
            app.UseCookieClearMiddleware();

            // Configure the path base
            app.UsePathBase("/timesheet");

            // Configure the HTTP request pipeline.

            app.UseExceptionHandler("/Error", createScopeForErrors: true);
            app.UseHsts();

            app.UseHttpsRedirection();

            app.UseStaticFiles(new StaticFileOptions
            {
                OnPrepareResponse = ctx =>
                {
                    // Set Cache-Control header to revalidate every 60 seconds
                    ctx.Context.Response.Headers.Append("Cache-Control", "public,max-age=60");
                }
            });
            app.UseAntiforgery();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseAntiforgery();

            app.MapRazorComponents<App>()
                .AddAdditionalAssemblies(Assembly.Load(nameof(SharedLib)))
                .AddInteractiveServerRenderMode();

            app.MapControllers();
            app.MapHealthChecks("/healthz").AllowAnonymous();

            app.MapGet("/signout-callback-oidc", async context =>
            {
                await context.SignOutAsync();
                context.Response.Redirect("/");
            });

            app.UseLocalization();

            app.Run();

            void ConfigureConfiguration(IConfigurationBuilder configurationBuilder, string environmentName)
            {
                // Default Configuration
                configurationBuilder
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables()
                    .AddKeyPerFile("/mnt/secrets-store", optional: true)
                    .AddUserSecrets<Program>(optional: true);

                // Key Vault Configuration
                var builtConfig = configurationBuilder.Build();
                var keyVaultEndpoint = builtConfig["AzureKeyVault:Endpoint"];
                var tenantId = builtConfig["AzureAd:TenantId"];
                var clientId = builtConfig["AzureAd:ClientId"];
                var clientSecret = builtConfig["CB-OPPO-CLIENT-SECRET"];

                if (!string.IsNullOrEmpty(keyVaultEndpoint) &&
                    !string.IsNullOrEmpty(tenantId) &&
                    !string.IsNullOrEmpty(clientId) &&
                    !string.IsNullOrEmpty(clientSecret))
                {
                    var credential = new ClientSecretCredential(tenantId, clientId, clientSecret);
                    configurationBuilder.AddAzureKeyVault(new Uri(keyVaultEndpoint), credential);
                }
            }
        }
    }
}
