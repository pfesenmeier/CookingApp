using HandlebarsDotNet;

using Microsoft.AspNetCore.Http.HttpResults;

namespace CookingApp.Web;

public class ViewFactory(ILogger<ViewFactory> logger)
{
    public View Create()
    {
        IHandlebars handlebars = Handlebars.Create(new HandlebarsConfiguration()
        {
            FileSystem = new DiskFileSystem()
        });
        TemplateDictionary handlbarHandlers = [];

        List<string> templates = [];
        foreach (string template in Directory.EnumerateFiles("Views", "*.hbs", SearchOption.AllDirectories))
        {
            if (template.Contains("partials"))
            {
                string fileName = Path.GetFileNameWithoutExtension(template);
                logger.LogDebug("Adding template {Template} as partial {Name}", template, fileName);

                handlebars.RegisterTemplate(fileName, File.ReadAllText(template));
            }
            else
            {
                templates.Add(template);
            }
        }

        foreach (string template in templates)
        {
            try
            {
                HandlebarsTemplate handler = handlebars.CompileView(template);
                string fileName = Path.GetFileNameWithoutExtension(template);

                logger.LogDebug("Adding template {Template} under key {Key}", template, fileName);
                handlbarHandlers.Add(fileName, handler);
            }
            catch (HandlebarsCompilerException)
            {
                logger.LogError("Handlebars template {Template} failed to compile", template);
                throw;
            }
            catch (Exception e) when (e is ArgumentException or ArgumentNullException)
            {
                string fileName = Path.GetFileName(template);
                logger.LogError("Handlebars file name {Name} is not unique", fileName);
                throw new ArgumentException($"Handlebars file name {fileName} is not unique");
            }
        }

        return new View(handlbarHandlers);
    }
}

public class View(TemplateDictionary templateDictionary)
{
    private string RenderTemplate(string templateName, object? data)
    {
        return templateDictionary[templateName](data ??= new { });
    }

    public IResult Render(string template, object? data = null)
    {
        return Results.Extensions.Html(RenderTemplate(template, data));
    }
}
