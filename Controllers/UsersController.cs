using MafiaProj.Data;
using MafiaProj.Models;
using MafiaProj.OtherStuff;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace MafiaProj.Controllers
{
    public class UsersController
    {
        // Создание пользователя
        public static async Task CreateUser(HttpRequest httpRequest, HttpResponse httpResponse, AppDbContext db)
        {
            try
            {
                // Читаем данные пользователя из JSON
                var user = await httpRequest.ReadFromJsonAsync<User>();
                if (user != null)
                {
                    if(await db.Users.FirstOrDefaultAsync(x => x.Name == user.Name) == null)
                    {
                        // Генерируем UID
                        user.Id = Guid.NewGuid().ToString();
                        // Преобразуем пароль в SHA256
                        user.Password = SHA256Helper.ComputeSHA256(user.Password);
                        // Добавляем пользователя и оформляем транзакцию
                        db.Users.Add(user);
                        httpResponse.StatusCode = 201;
                        await db.SaveChangesAsync();
                    }
                    else
                    {
                        httpResponse.StatusCode = 409;
                        await httpResponse.WriteAsJsonAsync(new { message = "UserExist" });
                    }
                }
                else
                {
                    throw new Exception();
                }
            }
            catch (Exception)
            {
                httpResponse.StatusCode = 400;
                await httpResponse.WriteAsJsonAsync(new { message = "IncorrectData" });
            }
        }

        public static async Task AuthUser(HttpRequest httpRequest, HttpResponse httpResponse, AppDbContext db, ISession session)
        {
            try
            {
                // Читаем данные пользователя из JSON
                var user = await httpRequest.ReadFromJsonAsync<User>();
                if (user != null)
                {
                    var findUser = await db.Users.FirstOrDefaultAsync(x => x.Name == user.Name);
                    if (findUser != null)
                    {
                        if(findUser.Password == SHA256Helper.ComputeSHA256(user.Password))
                        {
                            httpResponse.StatusCode = 200;
                            session.SetString("Id", findUser.Id);
                        }
                        else
                        {
                            httpResponse.StatusCode = 401;
                            await httpResponse.WriteAsJsonAsync(new { message = "IncorrectPassword" });
                        }
                    }
                    else
                    {
                        httpResponse.StatusCode = 404;
                        await httpResponse.WriteAsJsonAsync(new { message = "UserNotFound" });
                    }
                    // Добавляем пользователя и оформляем транзакцию
                    db.Users.Add(user);
                    httpResponse.StatusCode = 201;
                    await db.SaveChangesAsync();
                }
                else
                {
                    throw new Exception();
                }
            }
            catch (Exception)
            {
                httpResponse.StatusCode = 400;
                await httpResponse.WriteAsJsonAsync(new { message = "IncorrectData" });
            }
        }
    }
}
