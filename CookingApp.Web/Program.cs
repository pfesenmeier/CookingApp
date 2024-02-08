using CookingApp.Data;
using CookingApp.Web;
using CookingApp.Web.Routes;

using HandlebarsDotNet;

IHandlebars handlebars = Handlebars.Create(new HandlebarsConfiguration()
{
    FileSystem = new DiskFileSystem()
});

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

// TODO
// builder.Services.AddAntiforgery();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// compile all handlebar files
{
    TemplateDictionary handlbarHandlers = [];

    // TODO skip partials that are not used as HTMX...
    foreach (string template in Directory.EnumerateFiles("Views", "*.hbs", SearchOption.AllDirectories))
    {
        Console.WriteLine(template);

        try
        {
            HandlebarsTemplate handler = handlebars.CompileView(template);
            handlbarHandlers.Add(template, handler);
        }
        catch (HandlebarsCompilerException)
        {
            Console.WriteLine($"Error compiling template {template}");
            throw;
        }
    }
    builder.Services.AddSingleton(handlbarHandlers);
}

// start database connection
string connectionString =
  builder.Configuration.GetConnectionString("Postgres")
  ?? throw new InvalidOperationException("ConnectionString 'Postgres' is not defined");
builder.Services.AddDb(connectionString);

builder.Services.AddScoped<RecipeRepository>();
builder.Services.AddScoped<UserRepository>();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// TODO
// app.UseAntiforgery();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.MapCounter();
app.MapRecipe();
app.MapUserRoutes();

// .WithName("GetWeatherForecast")
// .WithOpenApi();

app.Run();
