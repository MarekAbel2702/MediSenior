using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediSenior.Services;
using MediSenior.Views;

namespace MediSenior.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly FirebaseAuthService _authService;
    private readonly UserService _userService;

    [ObservableProperty]
    private string email = "";

    [ObservableProperty]
    private string password = "";

    [ObservableProperty]
    private bool isBusy;

    public LoginViewModel(FirebaseAuthService authService, UserService userService)
    {
        _authService = authService;
        _userService = userService;
    }

    [RelayCommand]
    private async Task LoginAsync()
    {
        if (IsBusy)
            return;

        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            await Shell.Current.DisplayAlert("Błąd", "Podaj email i hasło.", "OK");
            return;
        }

        try
        {
            IsBusy = true;
            await _authService.LoginAsync(Email.Trim(), Password);
            var user = await _userService.LoadCurrentUserAsync();

            if (user == null)
            {
                await Shell.Current.DisplayAlert("Błąd", "Nie znaleziono profilu użytkownika.", "OK");
                return;
            }

            if (user.Role == "senior")
                await Shell.Current.GoToAsync($"//{nameof(LoginPage)}/{nameof(SeniorHomePage)}");
            else
                await Shell.Current.GoToAsync($"//{nameof(LoginPage)}/{nameof(CaregiverHomePage)}");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Błąd logowania", ex.Message, "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task ResetPasswordAsync()
    {
        if (string.IsNullOrWhiteSpace(Email))
        {
            await Shell.Current.DisplayAlert(
                "Przypomnienie hasła",
                "Wpisz adres email, dla którego chcesz zresetować hasło.",
                "OK");

            return;
        }

        try
        {
            await _authService.SendPasswordResetEmailAsync(Email.Trim());

            await Shell.Current.DisplayAlert(
                "Wysłano wiadomość",
                "Na podany adres email wysłano link do ustawienia nowego hasła.",
                "OK");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert(
                "Błąd",
                ex.Message,
                "OK");
        }
    }

    [RelayCommand]
    private Task GoToRegisterAsync()
    {
        return Shell.Current.GoToAsync(nameof(RegisterPage));
    }
}
