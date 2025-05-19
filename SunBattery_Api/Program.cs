using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Quartz;
using SunBattery.Core.Entities;

using SunBattery_Api.Models.EmailSenderModels;
using SunBattery_Api.Services.Commands;
using SunBattery_Api.Services.EmailServices;
using SunBattery_Api.Services.UserManagements;
using System.Data;
using System.Text;


namespace SunBattery_Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //// The location that uploaded files will be stored
            //// This ideally should be stored as a setting
            //const string fileStoreLocation = "/Users/rad/Projects/Temp/Conrad/Uploaded";

            //// Create the location, if it doesn't exist
            //string rootDir = Directory.GetCurrentDirectory();
            //if (!Directory.Exists(fileStoreLocation))
            //    Directory.CreateDirectory(fileStoreLocation);

            //// Allowed file extensions
            //string[] allowedFileExtensions = [".jpg", ".jpeg", ".png", ".gif", ".pdf", ".docx", ".xlsx"];


            var configuration = builder.Configuration;

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                var connetionString = builder.Configuration.GetConnectionString("Default");
                options.UseMySql(connetionString, ServerVersion.AutoDetect(connetionString));
            });

            // For Identity
            builder.Services
             .AddIdentity<ApplicationUser, IdentityRole>()
             .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // Add Config for Required Email
            builder.Services.Configure<IdentityOptions>(options => options.SignIn.RequireConfirmedEmail = true);

            builder.Services.Configure<DataProtectionTokenProviderOptions>(opt => opt.TokenLifespan = TimeSpan.FromHours(10));

            // Adding Authentification
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(opt =>
            {
                opt.SaveToken = true;
                opt.RequireHttpsMetadata = false;
                opt.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = configuration["JWT:ValidAudience"],
                    ValidIssuer = configuration["JWT:ValidIssuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]))
                };
            });

            //Add Email Configs
            var emailConfig = configuration.GetSection("EmailConfiguration").Get<EmailConfiguration>();
            if (emailConfig != null)
            {
                builder.Services.AddSingleton(emailConfig);
            }
            builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services.AddScoped<IUserManagement, UserManagement>();

            // Add services to the container.
            builder.Services.AddScoped<IParseCommand, ParseCommand>();

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(option =>
            {
                option.SwaggerDoc("v1", new OpenApiInfo { Title = "SunStation API", Version = "v1" });
                option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter a valid token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "Bearer",
                });
                option.AddSecurityRequirement(new OpenApiSecurityRequirement
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
                        new string[]{}
                    }
                });
            });



            var corsOrigins = configuration["App:CorsOrigins"] == null ? "http://localhost:4200" : configuration["App:CorsOrigins"];
            
            builder.Services.AddCors(
                options => options.AddPolicy(
                    "Default Policy",
                    builder => builder
                    .WithOrigins(
                            // App:CorsOrigins in appsettings.json can contain more than one address separated by comma.
                            corsOrigins?
                                .Split(",", StringSplitOptions.RemoveEmptyEntries)
                                .ToArray()
                        )
                       .AllowAnyHeader()
                       .AllowAnyMethod()
                       .AllowCredentials()


                ));

            builder.Services.AddSignalR();


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseCors("Default Policy");
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}


