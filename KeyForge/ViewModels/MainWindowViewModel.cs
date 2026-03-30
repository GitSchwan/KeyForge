using KeyForge.Data;
using KeyForge.Services;

namespace KeyForge.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly ILoginService _loginService;
    private readonly ICryptoService _cryptoService;
    private readonly KeyForgeDbContext _dbContext;
    private readonly SessionService _sessionService;

    private ViewModelBase? _currentViewModel;
    public ViewModelBase? CurrentViewModel
    {
        get => _currentViewModel;
        set => SetProperty(ref _currentViewModel, value);
    }

    public MainWindowViewModel(ILoginService loginService, 
        ICryptoService cryptoService, 
        KeyForgeDbContext dbContext, SessionService sessionService)
    {
        _loginService = loginService;
        _cryptoService = cryptoService;
        _dbContext = dbContext;
        _sessionService = sessionService;

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
        CurrentViewModel = new HomeViewModel(NavigateToAdd, new VaultService(_dbContext), _sessionService, _cryptoService);
    }

    private void NavigateToAdd()
    {
        CurrentViewModel = new AddViewModel(NavigateToHome, _cryptoService, _sessionService);
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
