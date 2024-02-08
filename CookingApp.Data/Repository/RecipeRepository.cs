using System.Data.Common;

using CookingApp.Data.Model;

using Dapper;

namespace CookingApp.Data;

public class RecipeRepository(DbDataSource dbDataSource)
{
    public async Task CreateAsync(string title, string[] ingredients, string[] steps)
    {
        DbConnection connection = await dbDataSource.OpenConnectionAsync();

        await connection.ExecuteAsync(@"
            INSERT INTO recipes (title, steps, ingredients) VALUES 
            (@title, @ingredients, @steps);
            ", new { title, ingredients, steps });
    }

    public async Task<Recipe> GetRecipeAsync(Guid id)
    {
        DbConnection connection = await dbDataSource.OpenConnectionAsync();

        IEnumerable<Recipe> recipes = await connection.QueryAsync<Recipe>(
            "SELECT * FROM recipes WHERE \"id\" = @id", new { id });

        return recipes.Single();
    }

    public async Task<IEnumerable<Recipe>> GetRecipes()
    {
        DbConnection connection = await dbDataSource.OpenConnectionAsync();

        return await connection.QueryAsync<Recipe>("SELECT * FROM recipes;");
    }

    public async Task<IEnumerable<Recipe>> GetRecipesByUserIdAsync(Guid userid)
    {
        DbConnection connection = await dbDataSource.OpenConnectionAsync();

        return await connection.QueryAsync<Recipe>(
                @"SELECT * FROM recipes WHERE ""userid"" = @userid;", new { userid });
    }
}
