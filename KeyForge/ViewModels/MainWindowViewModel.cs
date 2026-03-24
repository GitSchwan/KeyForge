using KeyForge.Services;

namespace KeyForge.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly ILoginService _loginService;
    private readonly ICryptoService _cryptoService;

    private ViewModelBase? _currentViewModel;
    public ViewModelBase? CurrentViewModel
    {
        get => _currentViewModel;
        set => SetProperty(ref _currentViewModel, value);
    }

    public MainWindowViewModel(ILoginService loginService, ICryptoService cryptoService)
    {
        _loginService = loginService;
        _cryptoService = cryptoService;

        if (_loginService.HasUsers())
        {
            CurrentViewModel = new LoginViewModel(_loginService, NavigateToHome, NavigateToCreateUser);
        }
        else
        {
            CurrentViewModel = new CreateUserViewModel(_cryptoService, NavigateToHome);
        }
    }

    private void NavigateToHome()
    {
        CurrentViewModel = new HomeViewModel(NavigateToAdd);
    }

    private void NavigateToAdd()
    {
        CurrentViewModel = new AddViewModel(NavigateToHome, _cryptoService);
    }

    private void NavigateToCreateUser()
    {
        CurrentViewModel = new CreateUserViewModel(_cryptoService, NavigateToLogin);
    }
    
    private void NavigateToLogin()
    {
        CurrentViewModel = new LoginViewModel(_loginService, NavigateToHome, NavigateToCreateUser);
    }
}
