using CookingApp.Data;
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
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

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
