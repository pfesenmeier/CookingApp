using CookingApp.Data;

namespace CookingApp.Web.Route;

public static class Recipe
{
    public static void MapRecipe(this WebApplication app)
    {
        _ = app.MapGet("/recipe", async (RecipeRepository recipeRepository) => await recipeRepository.GetRecipes());
    }
}
