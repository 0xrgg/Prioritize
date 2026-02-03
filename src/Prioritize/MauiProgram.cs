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
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        builder.Services.AddMauiBlazorWebView();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
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

        System.Diagnostics.Debug.WriteLine($"=== BEFORE GETTING DBCONTEXT ===");

        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        System.Diagnostics.Debug.WriteLine($"=== GOT DBCONTEXT ===");

        try
        {
            // Get connection string FIRST before doing anything
            var connString = db.Database.GetConnectionString();
            System.Diagnostics.Debug.WriteLine($"Connection string from DbContext: '{connString}'");

            // Also check if DbContext has options configured
            var optionsDebug = db.Database.ProviderName;
            System.Diagnostics.Debug.WriteLine($"Provider name: {optionsDebug}");

            db.Database.Migrate();

        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"ERROR: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"Stack: {ex.StackTrace}");
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


