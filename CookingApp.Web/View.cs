using HandlebarsDotNet;

using Microsoft.AspNetCore.Http.HttpResults;

namespace CookingApp.Web;

public class ViewFactory(ILogger<ViewFactory> logger)
{
    private IHandlebars Handlebars { get; } = HandlebarsDotNet.Handlebars.Create(new HandlebarsConfiguration()
    {
        FileSystem = new DiskFileSystem()
    });

    private static bool IsPartial(string path) => path.Contains("partials");
    private IEnumerable<string> Templates { get; } = Directory.EnumerateFiles("Views", "*.hbs", SearchOption.AllDirectories);

    private void RegisterPartials()
    {
        foreach (string template in Templates)
        {
            if (IsPartial(template))
            {
                string fileName = Path.GetFileNameWithoutExtension(template);
                logger.LogDebug("Adding template {Template} as partial {Name}", template, fileName);

                Handlebars.RegisterTemplate(fileName, File.ReadAllText(template));
            }
        }
    }

    private TemplateDictionary CompileViews()
    {
        TemplateDictionary handlbarHandlers = [];
        foreach (string template in Templates)
        {
            if (!IsPartial(template))
            {
                try
                {
                    HandlebarsTemplate handler = Handlebars.CompileView(template);
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
        }
        return handlbarHandlers;
    }

    public View CreateHotReloadableView()
    {
        TemplateDictionary Reload()
        {
            RegisterPartials();
            return CompileViews();
        }

        return new View(null!, Reload);
    }

    public View Create()
    {
        RegisterPartials();

        return new View(CompileViews());
    }
}

public class View(TemplateDictionary templateDictionary, Func<TemplateDictionary>? reload = null)
{
    private string RenderTemplate(string templateName, object? data)
    {
        if (reload is not null)
        {
            templateDictionary = reload();
        }

        return templateDictionary[templateName](data ??= new { });
    }

    public IResult Render(string template, object? data = null)
    {
        return Results.Extensions.Html(RenderTemplate(template, data));
    }
}

