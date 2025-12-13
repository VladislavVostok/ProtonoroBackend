
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProtonoroBackend.Configurations;
using ProtonoroBackend.Models;
using ProtonoroBackend.Services;
using ProtonoroBackend.Services.Interfaces;
using System;

namespace ProtonoroBackend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });



            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true)
                .Build();

            builder.Services.Configure<EmailSettings>(
                    configuration.GetSection("EmailSettings")
            );

            builder.Services.Configure<FrontendServerSettings>(
                    configuration.GetSection("FrontendServerSettings")
            );

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(configuration["Jwt:Key"]))
                };
            });

            //builder.Services.AddDbContext<PomodoroDBContext>(options =>
            // options.UseSqlServer(builder.Configuration.GetConnectionString("MsSqlSConnectionString")));            

            builder.Services.AddDbContext<PomodoroDBContext>(options =>
                options.UseSqlite("Data Source=helloapp.db")
            );


            builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
            {
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireUppercase = false;
            })
            .AddEntityFrameworkStores<PomodoroDBContext>()
            .AddDefaultTokenProviders();

            builder.Services.AddControllers();


            //builder.Services.AddScoped<IProfileService, ProfileService>();
            builder.Services.AddScoped<ITokenService, TokenService>();
            //builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddTransient<IEmailService, EmailService>();
            builder.Services.AddTransient<IAuthService, AuthService>();




            var app = builder.Build();

            //if (app.Environment.IsProduction())
            //{
                app.UseSwagger();
                app.UseSwaggerUI();
            //}


            app.UseHttpsRedirection();

            app.UseAuthorization();
            app.UseAuthentication();
            app.UseCors("AllowAll");
            app.MapControllers();

            app.Run();
        }
    }
}
