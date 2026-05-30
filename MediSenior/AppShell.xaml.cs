using MediSenior.Views;

namespace MediSenior
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(RegisterPage), typeof(RegisterPage));
            Routing.RegisterRoute(nameof(SeniorHomePage), typeof(SeniorHomePage));
            Routing.RegisterRoute(nameof(CaregiverHomePage), typeof(CaregiverHomePage));
            Routing.RegisterRoute(nameof(MedicineFormPage), typeof(MedicineFormPage));
            Routing.RegisterRoute(nameof(LinkSeniorPage), typeof(LinkSeniorPage));
        }
    }
}
