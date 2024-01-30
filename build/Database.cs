using System;
using System.Threading.Tasks;

using Npgsql;

using Nuke.Common.IO;

using Serilog;

namespace Build;

public class Database(string connectionString)
{
    public static string GenerateMigrationFileName(string postfix)
    {
        return DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + postfix.Trim() + ".sql";
    }

    // version 1
    // version 2: Run migration in a transaction where only commits if migration has not been ran
    // version 3: Db init script to create migrations table, main Database
    // version 4: reset, seed database  
    public async Task RunMigration(AbsolutePath migrationFile)
    {
        string fileContents = migrationFile.ReadAllText();

        // https://www.npgsql.org/doc/index.html#getting-started
        await using NpgsqlDataSource dataSource = NpgsqlDataSource.Create(connectionString);
        await using NpgsqlCommand cmd = dataSource.CreateCommand(fileContents);

        int numRowsAffected = await cmd.ExecuteNonQueryAsync();

        Log.Information(
            "Executed migration {Migration}. {Rows} rows affected.",
            migrationFile.NameWithoutExtension,
            numRowsAffected);
    }
}
