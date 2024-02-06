using System.Data.Common;
using Dapper;

namespace CookingApp.Data;

public class UserRepository(DbDataSource dbDataSource)
{
    public async Task CreateAsync(string username)
    {
        DbConnection connection = await dbDataSource.OpenConnectionAsync();

        await connection.ExecuteAsync("INSERT INTO users (username) VALUES (@username);",
                new { username });
    }
}
