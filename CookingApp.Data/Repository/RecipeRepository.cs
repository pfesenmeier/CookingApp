using System.Data.Common;

using CookingApp.Data.Model;

using Dapper;

namespace CookingApp.Data;

public class RecipeRepository(DbDataSource dbDataSource)
{
    public async Task<IEnumerable<Recipe>> GetRecipes()
    {
        DbConnection connection = await dbDataSource.OpenConnectionAsync();

        return await connection.QueryAsync<Recipe>("SELECT * FROM recipes;");
    }
}
