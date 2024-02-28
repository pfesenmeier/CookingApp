using CookingApp.Data;
using CookingApp.Web.FileWatcher;

using HandlebarsDotNet;

namespace CookingApp.Web;

public static class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder AddAppServices(this WebApplicationBuilder builder)
    {
        IHandlebars handlebars = Handlebars.Create(new HandlebarsConfiguration()
        {
            FileSystem = new DiskFileSystem()
        });

        // TODO
        // builder.Services.AddAntiforgery();
        Console.WriteLine(builder.Environment.EnvironmentName);
        if (builder.Environment.IsDevelopment())
        {
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            Console.WriteLine(Environment.CurrentDirectory);
            builder.Services.AddSingleton(container => new FileChangedEventSource("Views",
                        container.GetRequiredService<ILogger<FileChangedEventSource>>()));
        }

        builder.Services.AddSingleton
        (
            container =>
            {
                ILogger<ViewFactory> logger = container.GetRequiredService<ILogger<ViewFactory>>();
                ViewFactory viewFactory = new(logger);
                return viewFactory.Create();
            }

        );

        // start database connection
        string connectionString =
          builder.Configuration.GetConnectionString("Postgres")
          ?? throw new InvalidOperationException("ConnectionString 'Postgres' is not defined");
        builder.Services.AddDb(connectionString);

        builder.Services.AddScoped<RecipeRepository>();
        builder.Services.AddScoped<UserRepository>();

        return builder;
    }

}
