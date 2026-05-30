using MediSenior.ViewModels;

namespace MediSenior.Views;

public partial class CaregiverHomePage : ContentPage
{
    private readonly CaregiverHomeViewModel _viewModel;

    public CaregiverHomePage(CaregiverHomeViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadCommand.ExecuteAsync(null);
    }
}
