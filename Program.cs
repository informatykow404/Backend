using Backend.Data.Models;
using Backend.EntityFramework.Contexts;
using Backend.Repositories.Implementations;
using Backend.Repositories.Interfaces;
using Backend.Services.Implementations;
using Backend.Services.Interfaces;
using Backend.Settings;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Npgsql;
using System.Security.Claims;
using System.Text;

namespace Backend;

public class Program
{
    public static void Main(string[] args)
    {
        Env.Load();
        var builder = WebApplication.CreateBuilder(args);
        builder = AddJwtService(builder);
        builder = AddSwaggerConfig(builder);
        builder = AddDbContext(builder);
        builder = AddServices(builder);

        builder.Services.AddControllers();
        builder.Services.AddRouting(options => options.LowercaseUrls = true);
        builder.Services.AddEndpointsApiExplorer();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthentication();
        
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }

    private static WebApplicationBuilder AddJwtService(WebApplicationBuilder builder)
    {
        // PRD
        // var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET");

        // DEV (remove on PRD)
        var jwtSecret = builder.Configuration["JWT_SECRET"];

        //JWT AUTHORIZATION
        builder.Services.AddAuthorization();

        //JWT AUTHENTICATION
        builder.Services.AddIdentity<User, IdentityRole>()
            .AddEntityFrameworkStores<DataContext>()
            .AddDefaultTokenProviders();
        // Adding Jwt Bearer
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.SaveToken = true;
            options.RequireHttpsMetadata = false;
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidAudience = builder.Configuration["Authentication:ValidAudience"],
                ValidIssuer = builder.Configuration["Authentication:ValidIssuer"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
            };
        });

        return builder;
    }

    private static WebApplicationBuilder AddSwaggerConfig(WebApplicationBuilder builder)
    {
        builder.Services.AddSwaggerGen(c =>
        {
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT"
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
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

        return builder;
    }

    private static WebApplicationBuilder AddDbContext(WebApplicationBuilder builder)
    {
        string envFilePath = ".env";
        string postgreUrl;
        if (File.Exists(envFilePath))
        {
            postgreUrl = Environment.GetEnvironmentVariable("POSTGRE_URL") ?? builder.Configuration["POSTGRE_URL"];
        }
        else
        {
            postgreUrl = builder.Configuration["POSTGRE_URL"];
        }

        var uri = new Uri(postgreUrl);
        var userInfo = uri.UserInfo.Split(':');
        var npgsqlBuilder = new NpgsqlConnectionStringBuilder
        {
            Host = uri.Host,
            Port = uri.Port,
            Database = uri.AbsolutePath.TrimStart('/'),
            Username = userInfo[0],
            Password = userInfo[1],
            Pooling = true,
            SslMode = SslMode.Require
        };

        builder.Services.AddDbContext<DataContext>(opts =>
            opts.UseNpgsql(
                npgsqlBuilder.ConnectionString,
                sqlOpts => sqlOpts.EnableRetryOnFailure()
            )
        );

        return builder;
    }

    private static WebApplicationBuilder AddServices(WebApplicationBuilder builder)
    {
        builder.Services
            .AddScoped<IScienceClubRepository, ScienceClubRepository>()
            .AddScoped<IUserRepository, UserRepository>()
            .AddScoped<IScienceClubService, ScienceClubService>()
            .AddScoped<IUserService, UserService>()
            .AddScoped<IRefreshTokenService, RefreshTokenService>()
            .AddScoped<IJwtService, JwtService>();

        builder.Services.AddTransient<IEmailSender, EmailSender>();
        builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("Smtp"));

        return builder;
    }
}