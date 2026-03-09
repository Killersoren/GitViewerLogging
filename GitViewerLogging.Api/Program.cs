
using GitViewerLogging.DataAccess.Models;
using GitViewerLogging.RabbitMQ;
using GitViewerLogging.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace GitViewerLogging;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.AddServiceDefaults();

        var configuration = builder.Configuration;

        // Add services to the container.
        builder.Services.AddHostedService<RabbitMQConsumer>();
        builder.Services.AddScoped<ILoggingQueryService, LoggingQueryService>();

        builder.Services.Configure<RabbitMQSettings>(builder.Configuration.GetSection("RabbitMQ"));

        var allowedOrigins = builder.Configuration
         .GetSection("AllowedOrigins")
         .Get<string[]>();

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new() { Title = "GitviewerLogging API", Version = "v1" });

            // Add JWT Authentication to Swagger
            options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                Description = "Enter 'Bearer' and token"
            });

            options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
                {
                    {
                        new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                        {
                            Reference = new Microsoft.OpenApi.Models.OpenApiReference
                            {
                                Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
        });

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["AppSettings:Issuer"],
                    ValidAudience = configuration["AppSettings:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["AppSettings:Token"]!)
                    )
                };
            });

        builder.Services.AddAuthorization();

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowFrontend",
                builder =>
                {
                    builder.WithOrigins(allowedOrigins!).AllowCredentials().AllowAnyMethod().AllowAnyHeader();
                });
        });


        if (builder.Environment.IsDevelopment())
        {
            builder.Services.AddDbContext<LoggingContext>(options =>
            {
                var connectionString = builder.Configuration.GetConnectionString("postgres");
                options.UseNpgsql(connectionString);
            });
        }

        else
        {
            builder.Services.AddDbContext<LoggingContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),
            npgsqlOptions => npgsqlOptions.CommandTimeout(100)));
        }



        var app = builder.Build();

        app.MapDefaultEndpoints();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseCors("AllowFrontend");
        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}
