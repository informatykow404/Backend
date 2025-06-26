using System.Configuration;
using System.Text;
using System.Text.Json;
using Backend.DataModels.Config;
using Backend.EntityFramework.Contexts;
using Backend.EntityFramework.Models;
using Backend.Services.Implementations;
using Backend.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Backend;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddScoped<IEmailSender, EmailSender>();
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
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JsonSerializer.Deserialize<Secrets>(File.ReadAllText("Properties/secrets.json"),new JsonSerializerOptions()
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    })
                    ?.JwtSecret ?? throw new InvalidOperationException()))
            };
        });

        /*builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(o =>
        {
            o.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JsonSerializer.Deserialize<Secrets>(File.ReadAllText("Properties/secrets.json"),new JsonSerializerOptions()
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    })
                    ?.JwtSecret ?? throw new InvalidOperationException())),
                ValidAudience = builder.Configuration["Authentication:ValidAudience"],
                ValidIssuer = builder.Configuration["Authentication:ValidIssuer"]
            };
        });*/
        //DB context for entity framework
        builder.Services.AddDbContext<DataContext>(options =>
        {
            options.UseNpgsql(JsonSerializer.Deserialize<Config.Config>(File.ReadAllText("Properties/config.json"),
                    new JsonSerializerOptions()
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    })
                ?.DatabaseConnectionString);
        });
//        builder.Services.AddIdentityCore<User>().AddEntityFrameworkStores<DataContext>().AddDefaultTokenProviders();
        builder.Services.AddControllers();
        builder.Services.AddRouting(options => options.LowercaseUrls = true);
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddTransient<IEmailSender, EmailSender>();
        builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("Smtp"));

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        //app.MapIdentityApi<User>();
        app.UseHttpsRedirection();

        app.UseAuthentication();
        
        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}