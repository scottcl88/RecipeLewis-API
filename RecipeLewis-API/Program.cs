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
    var corsStrictOriginAllowed = new[] { "capacitor://localhost" };
    var corsOriginAllowed = new[] { "capacitor", "com.lewis.food", "foodlewis.com", "scottcl.com", "foodlewis.com", "localhost", "surf-n-eat.com", "surfneat.com" };
    return corsOriginAllowed.Any(origin =>
        Regex.IsMatch(host, $@"^((http(s)?)|({origin}))?://.*{origin}(:[0-9]+)?$", RegexOptions.IgnoreCase)) || corsStrictOriginAllowed.Any(origin => host == origin);
}

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
{
    var services = builder.Services;
    var env = builder.Environment;

    builder.Services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseLazyLoadingProxies()
                   .UseSqlServer(
                       builder.Configuration["DefaultConnection"]));
    services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
    {
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
    services.AddSwaggerGen(c =>
    {
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
    services.AddScoped<IRecipeService, RecipeService>();
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

//using (var scope = app.Services.CreateScope())
//{
//    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
//    var testUser = new User
//    {
//        Name = "User",
//        Username = "test",
//        PasswordHash = BCrypt.Net.BCrypt.HashPassword("test")
//    };
//    context.Users.Add(testUser);
//    context.SaveChanges();
//}

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
