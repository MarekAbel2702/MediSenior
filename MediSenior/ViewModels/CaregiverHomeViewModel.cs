using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediSenior.Models;
using MediSenior.Services;
using MediSenior.Views;
using System.Collections.ObjectModel;

namespace MediSenior.ViewModels;

public partial class CaregiverHomeViewModel : ObservableObject
{
    private readonly MedicineService _medicineService;
    private readonly UserService _userService;
    private readonly SessionService _session;
    private readonly FirebaseAuthService _authService;

    public ObservableCollection<MedicineTodayItem> Medicines { get; } = new();

    [ObservableProperty]
    private string title = "Panel opiekuna";

    [ObservableProperty]
    private string selectedSeniorId = "";

    [ObservableProperty]
    private string selectedSeniorName = "";

    [ObservableProperty]
    private bool hasSenior;

    [ObservableProperty]
    private bool isBusy;

    public CaregiverHomeViewModel(
        MedicineService medicineService,
        UserService userService,
        SessionService session,
        FirebaseAuthService authService)
    {
        _medicineService = medicineService;
        _userService = userService;
        _session = session;
        _authService = authService;
    }

    [RelayCommand]
    private async Task LoadAsync()
    {
        if (IsBusy)
            return;

        if (_session.CurrentUser == null)
            return;

        try
        {
            IsBusy = true;
            Medicines.Clear();

            HasSenior = _session.CurrentUser.SeniorIds.Count > 0;

            if (!HasSenior)
            {
                Title = "Brak połączonego seniora";
                SelectedSeniorName = "";
                return;
            }

            SelectedSeniorId = _session.CurrentUser.SeniorIds.First();
            var senior = await _userService.GetUserByIdAsync(SelectedSeniorId);
            SelectedSeniorName = senior?.Name ?? "Senior";
            Title = $"Podopieczny: {SelectedSeniorName}";

            var items = await _medicineService.GetTodayMedicinesForSeniorAsync(SelectedSeniorId);

            foreach (var item in items)
                Medicines.Add(item);
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

    [RelayCommand]
    private Task LinkSeniorAsync()
    {
        return Shell.Current.GoToAsync(nameof(LinkSeniorPage));
    }

    [RelayCommand]
    private Task AddMedicineAsync()
    {
        if (string.IsNullOrWhiteSpace(SelectedSeniorId))
            return Shell.Current.DisplayAlert("Brak seniora", "Najpierw połącz konto z seniorem.", "OK");

        return Shell.Current.GoToAsync($"{nameof(MedicineFormPage)}?seniorId={SelectedSeniorId}");
    }

    [RelayCommand]
    private async Task DeleteMedicineAsync(MedicineTodayItem item)
    {
        if (item == null)
            return;

        var confirm = await Shell.Current.DisplayAlert(
            "Usuń lek",
            $"Czy na pewno chcesz usunąć lek: {item.Name}?",
            "Tak",
            "Nie"
        );

        if (!confirm)
            return;

        try
        {
            await _medicineService.DeleteMedicineAsync(item);
            await LoadAsync();
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Błąd", ex.Message, "OK");
        }
    }

    [RelayCommand]
    private async Task ShowDetailsAsync(MedicineTodayItem item)
    {
        if (item == null)
            return;

        var message =
            $"Nazwa: {item.Name}\n" +
            $"Dawka: {item.Dose}\n" +
            $"Godzina tej dawki: {item.Time}\n" +
            $"Uwagi: {item.Notes}\n" +
            $"Status: {item.StatusText}";

        await Shell.Current.DisplayAlert("Szczegóły leku", message, "OK");
    }

    [RelayCommand]
    private async Task LogoutAsync()
    {
        var confirm = await Shell.Current.DisplayAlert(
            "Wylogowanie",
            "Czy na pewno chcesz się wylogować?",
            "Tak",
            "Nie"
        );

        if (!confirm)
            return;

        await _authService.LogoutAsync();

        await Shell.Current.GoToAsync("//LoginPage");
    }
}
