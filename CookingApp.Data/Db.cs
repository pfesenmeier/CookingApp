using System.Data.Common;

using Microsoft.Extensions.DependencyInjection;

using Npgsql;

namespace CookingApp.Data;

public static class Db
{
    public static void AddDb(this IServiceCollection services, string connectionString)
    {
        services.AddSingleton<DbDataSource>(_ => NpgsqlDataSource.Create(connectionString));
        // services.AddScoped(sp =>
        // {
        //     NpgsqlDataSource dataSource = sp.GetService<NpgsqlDataSource>()!;
        //     return dataSource.OpenConnection();
        // });
    }
}
