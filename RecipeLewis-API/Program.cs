using Azure.Identity;
using Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using RecipeLewis.Business;
using RecipeLewis.Middleware;
using RecipeLewis.Models;
using RecipeLewis.Services;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

bool IsOriginAllowed(string host)
{
    var corsStrictOriginAllowed = new[] { "https://main.d2v2unaw7gu6b3.amplifyapp.com" };
    var corsOriginAllowed = new[] { "localhost", "recipelewis-api.azurewebsites.net", "main.d2v2unaw7gu6b3.amplifyapp.com", "recipelewis.com" };
    return corsOriginAllowed.Any(origin =>
        Regex.IsMatch(host, $@"^((http(s)?)|({origin}))?://.*{origin}(:[0-9]+)?$", RegexOptions.IgnoreCase) || corsStrictOriginAllowed.Any(origin => origin == host));
}

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
{
    var services = builder.Services;
    var env = builder.Environment;

    if (!env.IsDevelopment())
    {
        builder.Host.ConfigureHostConfiguration(host => {
            host.AddAzureKeyVault(
               new Uri("https://recipelewisvault2.vault.azure.net/"),
               new DefaultAzureCredential());
        });
    }

    builder.Host.ConfigureLogging(logging => {
        logging.ClearProviders();
        logging.AddConsole();
        logging.AddDebug();
    });

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
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

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