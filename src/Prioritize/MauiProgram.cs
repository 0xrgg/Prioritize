using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Prioritize.Core.Services;

namespace Prioritize
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            var dbPath = DatabasePath.GetDatabasePath();

            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite($"Filename={dbPath}")
            );

            builder.Services.AddSingleton<ITaskService, TaskService>();

            var app = builder.Build();

            ApplyMigrations(app);

            return app;

        }

        private static void ApplyMigrations(MauiApp app)
        {
            using var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Apply any pending migrations
            db.Database.Migrate();
        }

    }
}

public static class DatabasePath
{
    public static string GetDatabasePath()
    {
        return Path.Combine(
            FileSystem.AppDataDirectory,
            "app.db"
        );
    }
}


