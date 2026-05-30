using MediSenior.ViewModels;

namespace MediSenior.Views;

public partial class MedicineFormPage : ContentPage
{
    public MedicineFormPage(MedicineFormViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
