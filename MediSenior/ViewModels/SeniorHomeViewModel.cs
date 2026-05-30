using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediSenior.Models;
using MediSenior.Services;
using MediSenior.Views;
using System.Collections.ObjectModel;

namespace MediSenior.ViewModels;

public partial class SeniorHomeViewModel : ObservableObject
{
    private readonly MedicineService _medicineService;
    private readonly SessionService _session;
    private readonly FirebaseAuthService _authService;

    public ObservableCollection<MedicineTodayItem> Medicines { get; } = new();

    [ObservableProperty]
    private string title = "Moje leki";

    [ObservableProperty]
    private string seniorCodeText = "";

    [ObservableProperty]
    private bool isBusy;

    public SeniorHomeViewModel(MedicineService medicineService, SessionService session, FirebaseAuthService authService)
    {
        _medicineService = medicineService;
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
            Title = $"Witaj, {_session.CurrentUser.Name}";
            SeniorCodeText = $"Twój kod dla opiekuna: {_session.CurrentUser.SeniorCode}";

            Medicines.Clear();

            var items = await _medicineService.GetTodayMedicinesForSeniorAsync(_session.CurrentUser.Id);

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
    private async Task MarkAsTakenAsync(MedicineTodayItem item)
    {
        if (item == null)
            return;

        if (item.IsTaken)
        {
            await Shell.Current.DisplayAlert(
                "Informacja",
                "Ten lek został już oznaczony jako przyjęty.",
                "OK");

            return;
        }

        try
        {
            await _medicineService.MarkAsTakenAsync(item);
            await LoadAsync();
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Błąd", ex.Message, "OK");
        }
    }

    [RelayCommand]
    private Task AddMedicineAsync()
    {
        return Shell.Current.GoToAsync(nameof(MedicineFormPage));
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
