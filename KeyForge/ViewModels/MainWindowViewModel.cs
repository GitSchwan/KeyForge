using KeyForge.Data;
using KeyForge.Services;

namespace KeyForge.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly ILoginService _loginService;
    private readonly ICryptoService _cryptoService;
    private readonly KeyForgeDbContext _dbContext;
    private readonly SessionService _sessionService;
    private readonly IThemeRegistry _themeRegistry;
    private readonly ThemeService _themeService;

    private ViewModelBase? _currentViewModel;
    public ViewModelBase? CurrentViewModel
    {
        get => _currentViewModel;
        set => SetProperty(ref _currentViewModel, value);
    }

    public MainWindowViewModel(ILoginService loginService, 
        ICryptoService cryptoService, 
        KeyForgeDbContext dbContext, SessionService sessionService,
        IThemeRegistry themeRegistry, ThemeService themeService)
    {
        _loginService = loginService;
        _cryptoService = cryptoService;
        _dbContext = dbContext;
        _sessionService = sessionService;
        _themeRegistry = themeRegistry;
        _themeService = themeService;

        if (_loginService.HasUsers())
        {
            CurrentViewModel = new LoginViewModel(_cryptoService ,_loginService, _sessionService, NavigateToHome, NavigateToCreateUser);
        }
        else
        {
            CurrentViewModel = new CreateUserViewModel(_cryptoService, NavigateToHome);
        }
    }

    private void NavigateToHome()
    {
        CurrentViewModel = new HomeViewModel(NavigateToAdd, NavigateToSettings, new VaultService(_dbContext), 
            _sessionService, _cryptoService);
    }

    private void NavigateToSettings()
    {
        CurrentViewModel = new SettingsViewModel(_themeRegistry, _themeService, NavigateToHome, 
            _cryptoService, _sessionService, NavigateToLogin);
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
        CurrentViewModel = new LoginViewModel(_cryptoService ,_loginService, _sessionService, NavigateToHome, NavigateToCreateUser);
    }
}
