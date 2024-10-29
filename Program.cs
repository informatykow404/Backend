using System.Text;
using System.Text.Json;
using Backend.DataModels.Config;
using Microsoft.IdentityModel.Tokens;

namespace Backend;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        
        //JWT AUTHENTICATION
        builder.Services.AddAuthentication().AddJwtBearer(o =>
        {
            o.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JsonSerializer.Deserialize<Secrets>(File.ReadAllText("Properties/secrets.json"),new JsonSerializerOptions()
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    })
                    ?.SupaBaseSecret ?? throw new InvalidOperationException())),
                ValidAudience = builder.Configuration["Authentication:ValidAudience"],
                ValidIssuer = builder.Configuration["Authentication:ValidIssuer"]
            };
        });
        
        
        

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.UseAuthentication();
        
        app.MapControllers();

        app.Run();
    }
}