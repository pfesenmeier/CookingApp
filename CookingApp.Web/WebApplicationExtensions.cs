namespace CookingApp.Web;

public static class WebApplicationExtensions
{
    public static WebApplication UseAppFeatures(this WebApplication app)
    {
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

        return app;
    }
}
