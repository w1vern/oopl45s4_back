using MafiaAPI.Data;
using MafiaAPI.Hub;
using MafiaAPI.Repositories;
using MafiaAPI.Repositories.EntityFramework;
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
            builder.Services.AddTransient<IMatchRepository, EFMatchRepository>();
            builder.Services.AddTransient<IPlayerStateRepository, EFPlayerStateRepository>();
            builder.Services.AddTransient<IRoleRepository, EFRoleRepository>();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Add authentication service
            builder.Services.AddAuthentication("Cookies").AddCookie(options =>
            {
                // Specify login path
                options.LoginPath = "/api/Users/login";
                // Specify expiration time
                options.ExpireTimeSpan = TimeSpan.FromHours(1);
                // Refresh expiration time while User is active
                options.SlidingExpiration = true;
            });
            builder.Services.AddAuthorization();

            // Connecting PostgreSQL
            var connection = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connection));

            builder.Services.AddSignalR();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // Start Authentication Middleware
            app.UseAuthentication();
            app.UseAuthorization();

            // Use WebSockets MW
            app.UseWebSockets();

            app.UseCors(builder => builder.AllowAnyOrigin());

            app.MapControllers();

            app.MapHub<SignalRHub>("/api/signal");

            app.Run();
        }
    }
}
