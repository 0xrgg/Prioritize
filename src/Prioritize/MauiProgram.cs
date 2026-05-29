using CommunityToolkit.Maui;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Prioritize;
using Prioritize.Core.Services;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        SQLitePCL.Batteries_V2.Init();

        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        builder.Services.AddMauiBlazorWebView();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

#if ANDROID
        builder.Services.AddTransient<INotificationManagerService, Prioritize.Platforms.Android.NotificationManagerService>();
#elif IOS
        builder.Services.AddTransient<INotificationManagerService, Prioritize.Platforms.iOS.NotificationManagerService>();
#elif WINDOWS
        builder.Services.AddTransient<INotificationManagerService, Prioritize.Platforms.Windows.NotificationManagerService>();
#endif

        // Remove this line - it's not being used
        // var dbPath = DatabasePath.GetDatabasePath();

        builder.Services.AddDbContext<AppDbContext>(options =>
        {
            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "app.db");
            options.UseSqlite($"Data Source={dbPath}");
        });

        builder.Services.AddSingleton<ITaskService, TaskService>();

        var app = builder.Build();

        ApplyMigrations(app);

        return app;
    }

    private static void ApplyMigrations(MauiApp app)
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        try
        {
            var connString = db.Database.GetConnectionString();

            var optionsDebug = db.Database.ProviderName;

            db.Database.Migrate();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"ERROR: {ex.Message}");
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


