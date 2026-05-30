using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using AColor = Android.Graphics.Color;

namespace MediSenior;

[Activity(
    Theme = "@style/Maui.SplashTheme",
    MainLauncher = true,
    LaunchMode = LaunchMode.SingleTop,
    ConfigurationChanges =
        ConfigChanges.ScreenSize |
        ConfigChanges.Orientation |
        ConfigChanges.UiMode |
        ConfigChanges.ScreenLayout |
        ConfigChanges.SmallestScreenSize |
        ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        SetSystemBarsColors();
    }

    protected override void OnResume()
    {
        base.OnResume();

        SetSystemBarsColors();
    }

    private void SetSystemBarsColors()
    {
        if (Window == null)
            return;

        Window.SetStatusBarColor(AColor.ParseColor("#2563EB"));
        Window.SetNavigationBarColor(AColor.ParseColor("#F1F5F9"));

        if (Build.VERSION.SdkInt >= BuildVersionCodes.R)
        {
            Window.InsetsController?.SetSystemBarsAppearance(
                0,
                (int)WindowInsetsControllerAppearance.LightStatusBars
            );

            Window.InsetsController?.SetSystemBarsAppearance(
                (int)WindowInsetsControllerAppearance.LightNavigationBars,
                (int)WindowInsetsControllerAppearance.LightNavigationBars
            );
        }
        else
        {
            var flags = (int)Window.DecorView.SystemUiVisibility;

            flags &= ~(int)SystemUiFlags.LightStatusBar;

            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
                flags |= (int)SystemUiFlags.LightNavigationBar;

            Window.DecorView.SystemUiVisibility = (StatusBarVisibility)flags;
        }
    }
}