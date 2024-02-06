
using CookingApp.Data;
using Microsoft.AspNetCore.Mvc;

namespace CookingApp.Web.Routes;

public static class UserRoutes
{
    public static void MapUserRoutes(this WebApplication app)
    {
        _ = app
            .MapGet("/login", (TemplateDictionary templateDictionary)
                    => Results.Extensions.Html(templateDictionary[@"Views\user\login.hbs"](null!)))
            .WithName("login");
        _ = app
            .MapGet("/signup", (TemplateDictionary templateDictionary)
                    => Results.Extensions.Html(templateDictionary[@"Views\user\signup.hbs"](null!)))
            .WithName("signup");
        _ = app
            .MapPost("/signup", async (
                UserRepository userRepository,
                [FromForm] string username) => await userRepository.CreateAsync(username));
    }
}
