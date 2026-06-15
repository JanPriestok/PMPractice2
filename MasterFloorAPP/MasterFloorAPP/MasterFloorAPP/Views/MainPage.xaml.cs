using MasterFloorAPP.ViewModels;

namespace MasterFloorAPP;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
        BindingContext = new MainPageViewModel();
    }
}