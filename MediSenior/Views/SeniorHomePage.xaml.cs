using MediSenior.ViewModels;

namespace MediSenior.Views;

public partial class SeniorHomePage : ContentPage
{
    private readonly SeniorHomeViewModel _viewModel;

    public SeniorHomePage(SeniorHomeViewModel viewModel)
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
