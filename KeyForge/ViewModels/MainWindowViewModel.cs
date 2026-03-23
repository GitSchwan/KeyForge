using KeyForge.Services;

namespace KeyForge.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly ILoginService _loginService;
    private readonly ICryptoMasterService _cryptoMasterService;

    private ViewModelBase? _currentViewModel;
    public ViewModelBase? CurrentViewModel
    {
        get => _currentViewModel;
        set => SetProperty(ref _currentViewModel, value);
    }

    public MainWindowViewModel(ILoginService loginService, ICryptoMasterService cryptoMasterService)
    {
        _loginService = loginService;
        _cryptoMasterService = cryptoMasterService;

        if (_loginService.HasUsers())
        {
            CurrentViewModel = new LoginViewModel(_loginService, NavigateToHome, NavigateToCreateUser);
        }
        else
        {
            CurrentViewModel = new CreateUserViewModel(_cryptoMasterService, NavigateToHome);
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

    private void NavigateToCreateUser()
    {
        CurrentViewModel = new CreateUserViewModel(_cryptoMasterService, NavigateToHome);
    }
}
