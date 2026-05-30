using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediSenior.Services;
using MediSenior.Views;

namespace MediSenior.ViewModels;

public partial class RegisterViewModel : ObservableObject
{
    private readonly FirebaseAuthService _authService;
    private readonly UserService _userService;

    [ObservableProperty]
    private string name = "";

    [ObservableProperty]
    private string email = "";

    [ObservableProperty]
    private string password = "";

    [ObservableProperty]
    private bool isSenior = true;

    [ObservableProperty]
    private bool isCaregiver;

    [ObservableProperty]
    private bool isBusy;

    public RegisterViewModel(FirebaseAuthService authService, UserService userService)
    {
        _authService = authService;
        _userService = userService;
    }

    partial void OnIsSeniorChanged(bool value)
    {
        if (value)
            IsCaregiver = false;
    }

    partial void OnIsCaregiverChanged(bool value)
    {
        if (value)
            IsSenior = false;
    }

    [RelayCommand]
    private async Task RegisterAsync()
    {
        if (IsBusy)
            return;

        if (string.IsNullOrWhiteSpace(Name) ||
            string.IsNullOrWhiteSpace(Email) ||
            string.IsNullOrWhiteSpace(Password))
        {
            await Shell.Current.DisplayAlert("Błąd", "Uzupełnij wszystkie pola.", "OK");
            return;
        }

        try
        {
            IsBusy = true;

            await _authService.RegisterAsync(Email.Trim(), Password);
            var role = IsSenior ? "senior" : "caregiver";

            await _userService.CreateUserProfileAsync(Name.Trim(), role);

            if (role == "senior")
                await Shell.Current.GoToAsync($"//{nameof(LoginPage)}/{nameof(SeniorHomePage)}");
            else
                await Shell.Current.GoToAsync($"//{nameof(LoginPage)}/{nameof(CaregiverHomePage)}");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Błąd rejestracji", ex.Message, "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }
}
