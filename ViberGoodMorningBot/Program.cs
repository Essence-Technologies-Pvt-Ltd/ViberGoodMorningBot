
using Scalar.AspNetCore;
using ViberGoodMorningBot.Models;

namespace ViberGoodMorningBot
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            // Bind ViberSettings section to ViberSettings class
            builder.Services.Configure<ViberSettings>(
                builder.Configuration.GetSection("ViberSettings")
            );

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (true || app.Environment.IsDevelopment())
            {
                app.MapOpenApi();

                // Add Scalar
                app.MapScalarApiReference();
            }

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
