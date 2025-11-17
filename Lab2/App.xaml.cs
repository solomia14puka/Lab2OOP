using Lab2.Views;

    namespace Lab2;

public partial class App : Application
{
    private readonly MainPage _main;

    public App(MainPage mainPage)
    {
        InitializeComponent();
        _main = mainPage;
    }

    protected override Window CreateWindow(IActivationState? activationState)
        => new Window(new NavigationPage(_main));
}