using InteractHub.Api.Extensions;
using InteractHub.Application.Interfaces.Infrastructure;
using InteractHub.Application.Interfaces.Services;
using InteractHub.Application.Interfaces.Repositories;
using InteractHub.Application.Services;
using InteractHub.Infrastructure.Auth;
using InteractHub.Infrastructure.Options;
using InteractHub.Persistence.Extensions;
using InteractHub.Persistence.Repositories;
using System.Text.Json.Serialization;
using Microsoft.OpenApi.Models;
using InteractHub.Application.Interfaces.Infrastructure;
using InteractHub.Application.Interfaces.Repositories;
using InteractHub.Application.Interfaces.Services;
using InteractHub.Application.Services;
using InteractHub.Persistence.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(JwtOptions.SectionName));
builder.Services.Configure<IdentitySeedOptions>(builder.Configuration.GetSection(IdentitySeedOptions.SectionName));
builder.Services.Configure<CorsOptions>(builder.Configuration.GetSection(CorsOptions.SectionName));
builder.Services.Configure<BlobStorageOptions>(
    builder.Configuration.GetSection(BlobStorageOptions.SectionName));

builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

//  Azure Blob options
builder.Services.Configure<BlobStorageOptions>(
builder.Configuration.GetSection(BlobStorageOptions.SectionName));

// Đăng ký tầng Application (Services)
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPostService, PostService>();
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IFriendshipService, FriendshipService>();
builder.Services.AddScoped<IFileStorageService, FileStorageService>();
builder.Services.AddScoped<IStoryService, StoryService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<ILikeService, LikeService>();

// Đăng ký tầng Application (Repositories)
builder.Services.AddScoped<IPostRepository, PostRepository>();
builder.Services.AddScoped<IPostMediaRepository, PostMediaRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<IFriendshipRepository, FriendshipRepository>();
builder.Services.AddScoped<IStoryRepository, StoryRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IReportRepository, ReportRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IReportRepository, ReportRepository>();
builder.Services.AddScoped<ILikeRepository, LikeRepository>();





builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "InteractHub API",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Enter JWT Bearer token only. Example: Bearer {your token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactClient", policy =>
    {
        policy.WithOrigins(
                "http://localhost:3000",
                "http://localhost:5173",
                "http://localhost:5226")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "InteractHub API v1");
        options.DocumentTitle = "InteractHub API Docs";
    });
}

await app.SeedIdentityAsync();

app.UseHttpsRedirection();

app.UseCors(CorsExtensions.ReactClientPolicyName);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();