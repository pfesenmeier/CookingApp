using CookingApp.Data;
using CookingApp.Data.Model;

using Microsoft.AspNetCore.Mvc;

namespace CookingApp.Web.Routes;

public static class RecipeRoutes
{
    public static void MapRecipe(this WebApplication app)
    {
        _ = app.MapGet("/recipe", async (
                    RecipeRepository recipeRepository,
                    TemplateDictionary templateDictionary,
                    HttpContext context
                    ) =>
                {
                    if (!context.Request.Cookies.TryGetValue("userid", out string? useridCookie) ||
                            !Guid.TryParse(useridCookie, out Guid userid))
                    {
                        return TypedResults.RedirectToRoute("login");
                    }

                    IEnumerable<Recipe> recipes = await recipeRepository.GetRecipesByUserIdAsync(userid);

                    Dictionary<string, object> renderData = [];
                    renderData.Add("recipes", recipes);
                    HandlebarsTemplate handlebarsTemplate =
                        templateDictionary[@"Views\recipe\list\list.hbs"];

                    return Results.Extensions.Html(handlebarsTemplate(renderData));
                }).WithName("recipes");

        _ = app.MapGet("/recipe/{id}", async (
                    TemplateDictionary templateDictionary,
                    RecipeRepository recipeRepository,
                    Guid id) =>
                {
                    Recipe recipe = await recipeRepository.GetRecipeAsync(id);
                    HandlebarsTemplate handler =
                        templateDictionary[@"Views\recipe\show\show.hbs"];
                    return Results.Extensions.Html(handler(recipe));
                });

        _ = app.MapGet("/recipe/create", (TemplateDictionary templateDictionary) =>
                {
                    HandlebarsTemplate handler =
                        templateDictionary[@"Views\recipe\create\create.hbs"];
                    return Results.Extensions.Html(handler(null!));
                });

        _ = app.MapPost(
            "/recipe/create",
            async (
                RecipeRepository recipeRepository,
                [FromForm] string title,
                [FromForm] string ingredients,
                [FromForm] string steps
            ) =>
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

}
