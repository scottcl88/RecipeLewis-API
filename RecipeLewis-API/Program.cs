using Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using RecipeLewis.Business;
using RecipeLewis.Middleware;
using RecipeLewis.Models;
using RecipeLewis.Services;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Azure.Identity;

bool IsOriginAllowed(string host)
{
    var corsStrictOriginAllowed = new[] { "capacitor://localhost" };
    var corsOriginAllowed = new[] { "capacitor", "com.lewis.food", "foodlewis.com", "scottcl.com", "foodlewis.com", "localhost", "surf-n-eat.com", "surfneat.com" };
    return corsOriginAllowed.Any(origin =>
        Regex.IsMatch(host, $@"^((http(s)?)|({origin}))?://.*{origin}(:[0-9]+)?$", RegexOptions.IgnoreCase)) || corsStrictOriginAllowed.Any(origin => host == origin);
}

var builder = WebApplication.CreateBuilder(args);

// Add Azure App Configuration to the container.
var azAppConfigConnection = builder.Configuration["AppConfig"];
if (!string.IsNullOrEmpty(azAppConfigConnection))
{
// Use the connection string if it is available.
builder.Configuration.AddAzureAppConfiguration(options =>
{
options.Connect(azAppConfigConnection)
.ConfigureRefresh(refresh =>
{
// All configuration values will be refreshed if the sentinel key changes.
refresh.Register("TestApp:Settings:Sentinel", refreshAll: true);
});
});
}
else if (Uri.TryCreate(builder.Configuration["Endpoints:AppConfig"], UriKind.Absolute, out var endpoint))
{
// Use Azure Active Directory authentication.
// The identity of this app should be assigned 'App Configuration Data Reader' or 'App Configuration Data Owner' role in App Configuration.
// For more information, please visit https://aka.ms/vs/azure-app-configuration/concept-enable-rbac
builder.Configuration.AddAzureAppConfiguration(options =>
{
options.Connect(endpoint, new DefaultAzureCredential())
.ConfigureRefresh(refresh =>
{
// All configuration values will be refreshed if the sentinel key changes.
refresh.Register("TestApp:Settings:Sentinel", refreshAll: true);
});
});
}
builder.Services.AddAzureAppConfiguration();

// Add services to the container.
{
    var services = builder.Services;
    var env = builder.Environment;

    if (!env.IsDevelopment())
    {
        try
        {
            Console.WriteLine("Loading AzureCred in Program...");
            builder.Configuration.AddAzureKeyVault(
                 new Uri("https://foodlewisvault.vault.azure.net/"),
                 new DefaultAzureCredential());
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error in Program: " + ex.Message);
        }
    }



    builder.Services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseLazyLoadingProxies()
                   .UseSqlServer(
                       builder.Configuration["DefaultConnection"]));
    services.AddCors(o => o.AddPolicy("MyPolicy", builder => {
        builder.AllowCredentials()
               .AllowAnyMethod()
               .AllowAnyHeader()
               .WithExposedHeaders("content-disposition")
               .SetIsOriginAllowed(IsOriginAllowed);
    }));
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
    services.AddAutoMapper(typeof(OrganizationProfile));

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

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}
app.UseSwagger();
app.UseSwaggerUI();
app.UseAzureAppConfiguration();

app.UseHttpsRedirection();

app.UseAuthorization();

// global cors policy
app.UseCors(x => x
    .SetIsOriginAllowed(origin => true)
    .AllowAnyMethod()
    .AllowAnyHeader()
    .AllowCredentials());

// global error handler
app.UseMiddleware<ErrorHandlerMiddleware>();

// custom jwt auth middleware
app.UseMiddleware<JwtMiddleware>();

app.MapControllers();

app.Run();
