using KeyForge.Services;

namespace KeyForge.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private ViewModelBase? _currentViewModel;
    public ViewModelBase? CurrentViewModel
    {
        get => _currentViewModel;
        set => SetProperty(ref _currentViewModel, value);
    }

    public MainWindowViewModel(ILoginService loginService)
    {
        //CurrentViewModel = new LoginViewModel(loginService, NavigateToHome);
        CurrentViewModel = new HomeViewModel(NavigateToAdd); // For testing only WIP
    }

    private void NavigateToHome()
    {
        CurrentViewModel = new HomeViewModel(NavigateToAdd);
    }

    private void NavigateToAdd()
    {
        CurrentViewModel = new AddViewModel(NavigateToHome);
    }
}
