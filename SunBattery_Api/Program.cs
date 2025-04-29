using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Quartz;
using SunBattery_Api.Helpers;
using SunBattery_Api.Helpers.Jobs;
using SunBattery_Api.Services.Commands;
using System.Data;
using static Quartz.Logging.OperationName;

namespace SunBattery_Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var configuration = builder.Configuration;

            // Add services to the container.
            builder.Services.AddScoped<IParseCommand, ParseCommand>();
            builder.Services.AddScoped<IJob, WriteDatasJob>();

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                var connetionString = builder.Configuration.GetConnectionString("Default");
                options.UseMySql(connetionString, ServerVersion.AutoDetect(connetionString));
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

            app.UseRouting();

            app.UseAuthorization();


            //  ParseCommand.ParseCommandStr(args);
           //  WriteDatasFromCommandJob.Start();

            app.MapControllers();

            app.Run();
        }
    }
}


