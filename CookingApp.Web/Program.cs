using CookingApp.Data;
using CookingApp.Data.Model;
using CookingApp.Data.Monad;
using CookingApp.Web;

using Microsoft.AspNetCore.Mvc;

WebApplication app = WebApplication.CreateBuilder(args)
    .AddAppServices()
    .Build()
    .UseAppFeatures();

#region recipes
{
    RouteGroupBuilder recipes = app.MapGroup("/recipe");
    _ = recipes.MapGet("/", async (
        View view,
        RecipeRepository recipeRepository,
        HttpContext context) =>
    {
        // TODO add authentication to do this...
        if (!context.Request.Cookies.TryGetValue("userid", out string? useridCookie) ||
                !Guid.TryParse(useridCookie, out Guid userid))
        {
            return TypedResults.RedirectToRoute("login");
        }

        IEnumerable<Recipe> recipes = await recipeRepository.GetRecipesByUserIdAsync(userid);
        Dictionary<string, object> renderData = new() { { "recipes", recipes } };

        return view.Render("list", renderData);
    }).WithName("recipes");
    _ = recipes.MapGet("/{id}", async (
            Guid id,
            RecipeRepository recipeRepository,
            View view
            ) => view.Render("show", await recipeRepository.GetRecipeAsync(id)));
    _ = recipes.MapGet("/create", (View view) => view.Render("create"));
    _ = recipes.MapPost("/create", async (
            RecipeRepository recipeRepository,
            [FromForm] string title,
            [FromForm] string ingredients,
            [FromForm] string steps) =>
        {
            const StringSplitOptions options =
                StringSplitOptions.TrimEntries |
                StringSplitOptions.RemoveEmptyEntries;
            string[] ingredientList = ingredients.Split(Environment.NewLine, options);
            string[] stepList = steps.Split(Environment.NewLine, options);

            await recipeRepository.CreateAsync(title, ingredientList, stepList);

            // TODO recipes frontend
            // TODO only on success
            // TODO show server error on error
            // TODO repopulate with original values if bad
            return Results.RedirectToRoute("recipes");
        })
        // TODO
        .DisableAntiforgery();
}
#endregion recipes

#region users
{
    RouteGroupBuilder login = app.MapGroup("/login");
    _ = login.MapGet("/", (View view) => view.Render("login")).WithName("login");
    _ = login.MapPost("/", async (
            HttpContext context,
            UserRepository userRepository,
            [FromForm] string username) =>
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

    RouteGroupBuilder signup = app.MapGroup("/signup");
    _ = signup.MapGet("/", (View view) => view.Render("signup")).WithName("signup");
    _ = signup.MapPost("/", async (
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
#endregion users

app.Run();
