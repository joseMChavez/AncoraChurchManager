using AncoraChurchManager.ViewModels;
using AncoraChurchManager.ViewModels.Church;
using AncoraChurchManager.ViewModels.Member;
using Core.Services;
using Core.Services.Repository;
using Microsoft.Extensions.Logging;

namespace AncoraChurchManager;

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
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif
        #region CONFIGURE SERVICES

        
            // Database - path depends on platform
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "church_app.db");

            builder.Services.AddSingleton<DatabaseService>(
                new DatabaseService(dbPath)
            );
           //Repository
           builder.Services.AddSingleton< IMemberRepository,  ChurchRepository>();
           builder.Services.AddSingleton<IChurchRepository, ChurchRepository>();
            // Business services (Singleton because DatabaseService handles async connections)
            builder.Services.AddSingleton<ChurchService>();
            builder.Services.AddSingleton<MemberService>();
            // ============= REGISTER VIEWS AND VIEWMODELS =============

            // Churches
          //
            builder.Services.AddSingleton<ChurchesListViewModel>();

          //  builder.Services.AddTransient<CreateChurchPage>();
            builder.Services.AddTransient<CreateChurchViewModel>();

            // Members
          //  builder.Services.AddTransient<MembersListPage>();
            builder.Services.AddTransient<MembersListViewModel>();

           // builder.Services.AddTransient<AddMemberPage>();
            builder.Services.AddTransient<AddMemberViewModel>();
        #endregion
        // Initialize database on startup
        var app = builder.Build();
        
        MainThread.BeginInvokeOnMainThread(async void () =>
        {
            var db = app.Services.GetRequiredService<DatabaseService>();
            await db.InitializeAsync();
        });

        return app;
    }
}