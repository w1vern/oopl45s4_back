using MafiaProj.Controllers;
using MafiaProj.Data;
using MafiaProj.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace MafiaProj
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // ������� ������ ����������
            var builder = WebApplication.CreateBuilder(args);

            // ��������� ������ ������
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.Cookie.Name = ".MafiaGame.Session";
                options.IdleTimeout = TimeSpan.FromHours(12);
            });

            // ��������� ����������� PostgreSQL
            var connection = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connection));

            // ������ ����������
            var app = builder.Build();

            // ���������� ������ � ����������
            app.UseSession();

            // ��������� �������������
            app.Run(async (context) =>
            {
                var response = context.Response;
                var request = context.Request;
                var session = context.Session;
                var path = request.Path;
                var dbContext = app.Services.CreateScope().ServiceProvider.GetService<AppDbContext>();

                if (path == "/api/user/auth" && request.Method == "POST")
                {
                    await UsersController.AuthUser(request, response, dbContext, session);
                }
                else if (path == "/api/user/register" && request.Method == "POST")
                {
                    await UsersController.CreateUser(request, response, dbContext);
                }



                /*if (path == "/api/users" && request.Method == "GET")
                {
                    await GetAllPeople(response);
                }
                else if (Regex.IsMatch(path, expressionForGuid) && request.Method == "GET")
                {
                    // �������� id �� ������ url
                    string? id = path.Value?.Split("/")[3];
                    await GetPerson(id, response);
                }
                else if (path == "/api/users" && request.Method == "POST")
                {
                    await CreatePerson(response, request);
                }
                else if (path == "/api/users" && request.Method == "PUT")
                {
                    await UpdatePerson(response, request);
                }
                else if (Regex.IsMatch(path, expressionForGuid) && request.Method == "DELETE")
                {
                    string? id = path.Value?.Split("/")[3];
                    await DeletePerson(id, response);
                }
                else
                {
                    response.ContentType = "text/html; charset=utf-8";
                    await response.SendFileAsync("html/index.html");
                }*/
            });

            // ������ ����������
            app.Run();
        }
    }
}
