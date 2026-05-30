using MediSenior.Services;
using MediSenior.ViewModels;
using MediSenior.Views;
using Microsoft.Extensions.Logging;

namespace MediSenior
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
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            builder.Services.AddSingleton<HttpClient>();

            builder.Services.AddSingleton<SessionService>();
            builder.Services.AddSingleton<FirebaseAuthService>();
            builder.Services.AddSingleton<FirestoreService>();
            builder.Services.AddSingleton<UserService>();
            builder.Services.AddSingleton<MedicineService>();
            builder.Services.AddSingleton<NotificationService>();

            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<RegisterViewModel>();
            builder.Services.AddTransient<SeniorHomeViewModel>();
            builder.Services.AddTransient<CaregiverHomeViewModel>();
            builder.Services.AddTransient<MedicineFormViewModel>();
            builder.Services.AddTransient<LinkSeniorViewModel>();

            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<RegisterPage>();
            builder.Services.AddTransient<SeniorHomePage>();
            builder.Services.AddTransient<CaregiverHomePage>();
            builder.Services.AddTransient<MedicineFormPage>();
            builder.Services.AddTransient<LinkSeniorPage>();


#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
