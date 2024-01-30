using System;
using System.Collections.Generic;
using System.Linq;

using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.Execution;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Utilities.Collections;

using Serilog;

using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;

namespace Build;

class Build : NukeBuild
{
    [Parameter("MigrationName")]
    readonly string MigrationName;

    [Parameter("ConnectionString")]
    readonly string ConnectionString;

    public static int Main() => Execute<Build>(x => x.Compile);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [PathVariable]
    readonly Tool Docker;

    readonly AbsolutePath MigrationsDir = RootDirectory / "migrations";

    Target StartDb => _ => _
        .Executes(() =>
        {
            AbsolutePath DbComposePath = RootDirectory / "docker" / "db";

            Docker(arguments: "compose up", workingDirectory: DbComposePath);
        });

    Target CreateMigration => _ => _
        .Requires(() => MigrationName)
        .Executes(() =>
        {
            MigrationsDir.CreateDirectory();
            string newMigrationFile = Database.GenerateMigrationFileName(MigrationName);
            AbsolutePath newMigrationFilePath = MigrationsDir / newMigrationFile;
            newMigrationFilePath.TouchFile();
            RelativePath pathOutput = RootDirectory.GetRelativePathTo(newMigrationFilePath);
            Log.Information("Create New Migration {Migration}", pathOutput);
        });

    Target ApplyMigrations => _ => _
        .Requires(() => ConnectionString)
        .Executes(async () =>
        {
            Database database = new(ConnectionString);
            List<AbsolutePath> migrationFiles = [.. MigrationsDir.GlobFiles("*")];
            migrationFiles.Sort((file1, file2) => file1.Name.CompareTo(file2.Name));

            foreach (AbsolutePath file in migrationFiles)
            {
                await database.RunMigration(file);
            }

            // execute sql script or string...
            // can use dbcontext / npgsql? 
        });

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
        });

    Target Restore => _ => _
        .Executes(() =>
        {
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
        });

}
