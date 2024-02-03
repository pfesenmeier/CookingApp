using System.Net.Mime;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Http.Metadata;

namespace CookingApp.Web;

// https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/responses?view=aspnetcore-8.0#customizing-responses
public static class ResultsExtensions
{
    public static IResult Html(this IResultExtensions resultExtensions, string html)
    {
        ArgumentNullException.ThrowIfNull(resultExtensions);

        return new HtmlResult(html);
    }
}

public class HtmlResult(string html) : IResult, IEndpointMetadataProvider
{
    public Task ExecuteAsync(HttpContext httpContext)
    {
        httpContext.Response.ContentType = MediaTypeNames.Text.Html;
        httpContext.Response.ContentLength = Encoding.UTF8.GetByteCount(html);
        return httpContext.Response.WriteAsync(html);
    }

    public static void PopulateMetadata(MethodInfo method, EndpointBuilder builder)
    {
        builder.Metadata.Add(new ProducesHtmlMetadata());
    }
}

internal sealed class ProducesHtmlMetadata : IProducesResponseTypeMetadata
{
    public Type? Type => null;

    public int StatusCode => 200;

    public IEnumerable<string> ContentTypes { get; } = new[] { MediaTypeNames.Text.Html };
}
