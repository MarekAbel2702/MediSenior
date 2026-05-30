using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediSenior.Models;
using MediSenior.Services;

namespace MediSenior.ViewModels;

[QueryProperty(nameof(SeniorId), "seniorId")]
public partial class MedicineFormViewModel : ObservableObject
{
    private readonly MedicineService _medicineService;

    [ObservableProperty]
    private string seniorId = "";

    [ObservableProperty]
    private string name = "";

    [ObservableProperty]
    private string dose = "";

    [ObservableProperty]
    private TimeSpan time1 = new(8, 0, 0);

    [ObservableProperty]
    private TimeSpan time2 = new(14, 0, 0);

    [ObservableProperty]
    private TimeSpan time3 = new(20, 0, 0);

    [ObservableProperty]
    private bool useTime2;

    [ObservableProperty]
    private bool useTime3;

    [ObservableProperty]
    private string notes = "";

    [ObservableProperty]
    private bool isBusy;

    public MedicineFormViewModel(MedicineService medicineService)
    {
        _medicineService = medicineService;
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (IsBusy)
            return;

        if (string.IsNullOrWhiteSpace(Name))
        {
            await Shell.Current.DisplayAlert("Błąd", "Podaj nazwę leku.", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(Dose))
        {
            await Shell.Current.DisplayAlert("Błąd", "Podaj dawkę leku.", "OK");
            return;
        }

        try
        {
            IsBusy = true;

            var times = new List<string>
            {
                Time1.ToString(@"hh\:mm")
            };

            if (UseTime2)
                times.Add(Time2.ToString(@"hh\:mm"));

            if (UseTime3)
                times.Add(Time3.ToString(@"hh\:mm"));

            times = times
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            var medicine = new Medicine
            {
                SeniorId = SeniorId,
                Name = Name.Trim(),
                Dose = Dose.Trim(),
                Notes = Notes.Trim(),
                Times = times,
                IsActive = true
            };

            await _medicineService.AddMedicineAsync(medicine);

            await Shell.Current.DisplayAlert("Zapisano", "Lek został dodany.", "OK");

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