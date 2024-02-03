using HandlebarsDotNet;
using CookingApp.Web;

IHandlebars handlebars = Handlebars.Create(new HandlebarsConfiguration()
{
    FileSystem = new DiskFileSystem()
});

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

{
    TemplateDictionary handlbarHandlers = [];

    foreach (string template in Directory.EnumerateFiles("Views", "*.hbs", SearchOption.AllDirectories))
    {
        Console.WriteLine(template);
        HandlebarsTemplate handler = handlebars.CompileView(template);

        handlbarHandlers.Add(template, handler);
    }
    builder.Services.AddSingleton(handlbarHandlers);
}

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapCounter();

// .WithName("GetWeatherForecast")
// .WithOpenApi();

app.Run();
