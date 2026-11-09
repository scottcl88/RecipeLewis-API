using Azure.Identity;
using Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using RecipeLewis.Middleware;
using RecipeLewis.Models;
using RecipeLewis.Services;
using System.Text.Json.Serialization;

const string CorsPolicyName = "RecipeLewisCors";

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
{
    var services = builder.Services;
    var env = builder.Environment;

    if (!env.IsDevelopment())
    {
        builder.Configuration.AddAzureKeyVault(
               new Uri("https://recipelewisvault2.vault.azure.net/"),
               new DefaultAzureCredential());
    }

    builder.Logging.ClearProviders().AddConsole().AddDebug();

    builder.Services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseLazyLoadingProxies()
                   .UseSqlServer(
                       builder.Configuration["DefaultConnection"]));
    // CORS: single named policy, strict exact-match origin allowlist from configuration (Cors:AllowedOrigins).
    var allowedOrigins = CorsOrigins.Normalize(builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>());
    if (allowedOrigins.Length == 0)
    {
        throw new InvalidOperationException("Cors:AllowedOrigins is empty. Configure at least one explicit origin (no wildcards).");
    }
    services.AddCors(o => o.AddPolicy(CorsPolicyName, policy => policy
        .SetIsOriginAllowed(origin => CorsOrigins.IsAllowed(origin, allowedOrigins))
        .AllowAnyMethod()
        .AllowAnyHeader()
        .WithExposedHeaders("content-disposition")
        .AllowCredentials()));
    services.AddControllers().AddJsonOptions(x => {
        // serialize enums as strings in api responses (e.g. Role)
        x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen(c => {
        c.EnableAnnotations();
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "RecipeLewis-API",
            Version = "v1",
            Contact = new OpenApiContact()
            {
                Name = "Scott Lewis",
                Email = "scottcl@outlook.com"
            },
            Description = "API for the Recipe Lewis project. Copyright 2022 Scott Lewis. All rights reserved."
        });
    });
    // configure strongly typed settings object
    services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));

    // configure DI for application services
    services.AddScoped<IJwtUtils, JwtUtils>();
    services.AddScoped<LogService>();
    services.AddScoped<IUserService, UserService>();
    services.AddScoped<IDocumentService, DocumentService>();
    services.AddScoped<IRecipeService, RecipeService>();
    services.AddScoped<ICategoryService, CategoryService>();
    services.AddScoped<IEmailService, EmailService>();
}

var app = builder.Build();

// Fail fast when the JWT signing key is absent. Without this guard an empty
// Secret silently yields tokens signed with a zero-length key, which is worse
// than refusing to start: auth appears to work while being trivially forgeable.
// Provide the value via `dotnet user-secrets` locally, or the
// AppSettings__Secret environment variable in deployment. Never commit it.
var jwtSecret = builder.Configuration.GetValue<string>("AppSettings:Secret");
if (string.IsNullOrWhiteSpace(jwtSecret) || jwtSecret.Length < 32)
{
    throw new InvalidOperationException(
        "AppSettings:Secret is missing, empty, or shorter than 32 characters. " +
        "Set it with `dotnet user-secrets set \"AppSettings:Secret\" \"<32+ random chars>\"` " +
        "for local development, or via the AppSettings__Secret environment variable in deployment.");
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// CORS must run before authorization and the custom JWT middleware so preflight and error responses carry CORS headers.
app.UseCors(CorsPolicyName);

app.UseAuthorization();

// global error handler
app.UseMiddleware<ErrorHandlerMiddleware>();

// custom jwt auth middleware
app.UseMiddleware<JwtMiddleware>();

app.MapControllers();

app.Run();