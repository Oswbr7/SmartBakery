using SmartBakery.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// La cadena de conexión NO está en appsettings.json: vive en user-secrets
// (desarrollo) o en variables de entorno (otros ambientes).
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "Connection string 'DefaultConnection' is not configured. " +
        "Run: dotnet user-secrets set \"ConnectionStrings:DefaultConnection\" \"<connection string>\" --project src/SmartBakery.API");
}

builder.Services.AddInfrastructure(connectionString);

const string FrontendCorsPolicy = "Frontend";

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddHealthChecks();

builder.Services.AddCors(options =>
{
    options.AddPolicy(FrontendCorsPolicy, policy =>
    {
        var origins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
        policy.WithOrigins(origins).AllowAnyHeader().AllowAnyMethod();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "Smart Bakery API v1"));
}

app.UseHttpsRedirection();
app.UseCors(FrontendCorsPolicy);

app.MapHealthChecks("/health");
app.MapControllers();

app.Run();

public partial class Program;
