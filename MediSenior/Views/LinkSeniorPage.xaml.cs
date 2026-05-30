using MediSenior.ViewModels;

namespace MediSenior.Views;

public partial class LinkSeniorPage : ContentPage
{
    public LinkSeniorPage(LinkSeniorViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
