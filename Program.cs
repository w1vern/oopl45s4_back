using MafiaAPI.Data;
using MafiaAPI.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MafiaAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddTransient<IUserRepository, EFUserRepository>();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            /*builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();*/

            // Add session service
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.Cookie.Name = ".MafiaGame.Session";
                options.IdleTimeout = TimeSpan.FromHours(12);
            });

            // Add Authentication Service
            builder.Services.AddAuthentication("Cookies").AddCookie(options => options.LoginPath = "/api/users/login");
            builder.Services.AddAuthorization();

            // Connecting PostgreSQL
            var connection = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connection));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            /*if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }*/

            // Start Authentication Middleware
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
