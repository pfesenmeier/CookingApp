
using CookingApp.Data;
using CookingApp.Data.Model;
using CookingApp.Data.Monad;
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
            .MapPost("/login",
             async (
              HttpContext context,
              UserRepository userRepository,
              [FromForm] string username
             ) =>
             {
                 Maybe<User> user = await userRepository.GetUserByUsername(username);

                 if (user.IsNone)
                 {
                     return Results.BadRequest("user not found");
                 }

                 context.Response.Cookies.Append("userid", user.Some.Id.ToString());

                 return Results.RedirectToRoute("recipes");
             })
             // TODO
             .DisableAntiforgery();

        _ = app
            .MapGet("/signup", (TemplateDictionary templateDictionary)
                    => Results.Extensions.Html(templateDictionary[@"Views\user\signup.hbs"](null!)))
            .WithName("signup");
        _ = app
            .MapPost("/signup", async (
                UserRepository userRepository,
                HttpContext context,
                [FromForm] string username) =>
                {
                    await userRepository.CreateAsync(username);

                    Maybe<User> user = await userRepository.GetUserByUsername(username);

                    // TODO
                    if (user.IsNone)
                    {
                        return TypedResults.Problem("user creation unsuccessful");
                    }

                    context.Response.Cookies.Append("userid", user.Some.Id.ToString());

                    return Results.RedirectToRoute("recipes");
                });
    }
}
