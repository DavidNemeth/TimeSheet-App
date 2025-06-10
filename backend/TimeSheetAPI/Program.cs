using Azure.Identity;

using Microsoft.EntityFrameworkCore;

using System.Security.Cryptography.X509Certificates;
using System.Text.Json.Serialization;

using TimeSheet.Api;
using TimeSheet.Api.Data;
using TimeSheet.Api.Services;
using TimeSheet.Api.Services.Interfaces;

var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

var builder = WebApplication.CreateBuilder(args);

ConfigureConfiguration(builder.Configuration, environmentName);

builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

// Register TokenService with a named HttpClient
builder.Services.AddHttpClient<ITokenService, TokenService>();
// Register AuthenticationMessageHandler
builder.Services.AddTransient<AuthenticationMessageHandler>();
builder.Services.AddHttpClient("AuthenticatedClient")
    .AddHttpMessageHandler<AuthenticationMessageHandler>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();
builder.Services.AddHttpClient();

var connectionString = builder.Configuration[$"PG-TIMESHEET-SQL-CONNECTION-STRING-{builder.Configuration["AppSettings:ENV"]}"];
var certPath = "/etc/tls/" + builder.Configuration["OPPO-APP-TLS-CERT-NAME"];
var certPassword = builder.Configuration[$"OPPO-APP-TLS-CERT-PW-{builder.Configuration["AppSettings:ENV"]}"];

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddScoped<ITimesheetService, TimesheetService>();
int httpsPort = builder.Configuration.GetSection("AppSettings").GetValue<int>("ExposedHttpsPort");
if (!string.IsNullOrEmpty(certPath) && !string.IsNullOrEmpty(certPassword))
{
    builder.WebHost.ConfigureKestrel(serverOptions =>
    {
        try
        {
            var certificate = new X509Certificate2(certPath, certPassword);
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

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddAutoMapper(typeof(Program));

var app = builder.Build();

app.UsePathBase("/apitimesheet");
app.UseSwagger();
app.UseSwaggerUI();
app.UseHsts();

app.MapHealthChecks("/api/healthz").AllowAnonymous();
app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

app.Run();

void ConfigureConfiguration(IConfigurationBuilder configurationBuilder, string environmentName)
{
    configurationBuilder
        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
        .AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: true)
        .AddEnvironmentVariables()
        .AddKeyPerFile("/mnt/secrets-store", optional: true)
        .AddUserSecrets<Program>(optional: true);

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
