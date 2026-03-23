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

    public MainWindowViewModel(ILoginService loginService, ICryptoMasterService cryptoMasterService)
    {
        if (loginService.HasUsers())
        {
            CurrentViewModel = new LoginViewModel(loginService, NavigateToHome);
        }
        else
        {
            CurrentViewModel = new CreateUserViewModel(cryptoMasterService, NavigateToHome);
        }
    }

    private void NavigateToHome()
    {
        CurrentViewModel = new HomeViewModel(NavigateToAdd);
    }

    private void NavigateToAdd()
    {
        CurrentViewModel = new AddViewModel(NavigateToHome);
    }

    private void NavigateToLogin()
    {
        CurrentViewModel = new LoginViewModel(new LoginService(new Data.KeyForgeDbContext()), NavigateToHome);
    }
}
