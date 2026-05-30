using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediSenior.Services;

namespace MediSenior.ViewModels;

public partial class LinkSeniorViewModel : ObservableObject
{
    private readonly UserService _userService;

    [ObservableProperty]
    private string seniorCode = "";

    [ObservableProperty]
    private bool isBusy;

    public LinkSeniorViewModel(UserService userService)
    {
        _userService = userService;
    }

    [RelayCommand]
    private async Task LinkAsync()
    {
        if (IsBusy)
            return;

        if (string.IsNullOrWhiteSpace(SeniorCode))
        {
            await Shell.Current.DisplayAlert("Błąd", "Podaj kod seniora.", "OK");
            return;
        }

        try
        {
            IsBusy = true;
            await _userService.LinkCaregiverWithSeniorAsync(SeniorCode);
            await Shell.Current.DisplayAlert("Gotowe", "Połączono z seniorem.", "OK");
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Błąd", ex.Message, "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }
}
