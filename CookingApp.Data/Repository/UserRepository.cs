using System.Data.Common;
using CookingApp.Data.Model;
using CookingApp.Data.Monad;
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

    public async Task<Maybe<User>> GetUserByUsername(string username)
    {
        DbConnection connection = await dbDataSource.OpenConnectionAsync();
        IEnumerable<User> users = await connection.QueryAsync<User>(
                "SELECT * FROM users WHERE username = @username", new { username });

        if (users.Count() is 0)
        {
            return new None();
        }

        return users.Single();
    }

    public async Task<Maybe<User>> GetUserById(Guid id)
    {
        DbConnection connection = await dbDataSource.OpenConnectionAsync();
        User? user = await connection.QuerySingleOrDefaultAsync<User>(
                "SELECT * FROM users WHERE id = @id", new { id });

        if (user is null)
        {
            return new None();
        }

        return user;
    }
}
