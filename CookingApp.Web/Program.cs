using HandlebarsDotNet;

Dictionary<string, HandlebarsTemplate<object, object>> handlbarHandlers = [];
foreach (string template in Directory.EnumerateFiles("Views"))
{
    string source = File.ReadAllText(template);

    HandlebarsTemplate<object, object> handler = Handlebars.Compile(source);

    handlbarHandlers.Add(template, handler);
    Console.WriteLine(template);
}

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton(handlbarHandlers);
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();



app.MapGet("/layout", (Dictionary<string, HandlebarsTemplate<object, object>> templateDictionary) =>
{
    var result = templateDictionary[@"Views\layout.hbs"];

    var data = new { greeting = "hello, world" };
    return result(data);
});
// 

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
