using Lab2.ViewModels;

namespace Lab2.Views;

public partial class MainPage : ContentPage
{
    public MainPage(MainViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    protected override bool OnBackButtonPressed()
    {
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            bool close = await DisplayAlert("Підтвердження", "Чи дійсно ви хочете завершити роботу з програмою?", "Так", "Ні");
            if (close)
                await Navigation.PopAsync();
        });
        return true;
    }
}
