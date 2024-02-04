using CookingApp.Web;

public static class Counter
{
    public static int counter = 0;
    public static void MapCounter(this WebApplication app)
    {
        _ = app.MapGet("/counter", (TemplateDictionary templateDictionary) =>
        {
            HandlebarsTemplate template = templateDictionary[@"Views\counter\counter.hbs"];

            var data = new { counter };
            string html = template(data);
            return Results.Extensions.Html(html);
        });

        _ = app.MapPost("/counter", (TemplateDictionary templateDictionary) =>
        {
            HandlebarsTemplate template = templateDictionary[@"Views\counter\partials\counterDisplay.hbs"];

            var data = new { counter = ++counter };
            string html = template(data);

            return Results.Extensions.Html(html);
        });
    }
}
