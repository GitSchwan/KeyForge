using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.Input;
using KeyForge.Services;

namespace KeyForge.ViewModels;

public class SettingsViewModel : ViewModelBase
{
    private readonly IThemeRegistry _themeRegistry;
    private readonly ThemeService _themeService;
    private readonly ICryptoService _cryptoService;
    private readonly SessionService _sessionService;
    
    

    public RelayCommand ShowHomeViewCommand { get; }
    
    public ObservableCollection<SettingsNavItem> NavItems { get; }
    
    private SettingsNavItem? _selectedNavItem;
    public SettingsNavItem? SelectedNavItem
    {
        get => _selectedNavItem;
        set
        {
            if (SetProperty(ref _selectedNavItem, value))
            {
                CurrentPage = value?.ViewModel;
            }
        }
    }

    private ViewModelBase? _currentPage;
    public ViewModelBase? CurrentPage
    {
        get => _currentPage;
        set => SetProperty(ref _currentPage, value);
    }
    public SettingsViewModel(IThemeRegistry themeRegistry, ThemeService themeService, Action navigateToHomeView, 
        ICryptoService cryptoService, SessionService sessionService, Action NavigateToLogin)
    {
        _themeRegistry = themeRegistry;
        _themeService = themeService;
        _cryptoService = cryptoService;
        _sessionService = sessionService;
        var _navigate = NavigateToLogin;
        ShowHomeViewCommand = new RelayCommand(navigateToHomeView);
        
        NavItems = new ObservableCollection<SettingsNavItem>
        {
            new SettingsNavItem("Aussehen", "M286.837,101.407h-33.58l-98.774-71." +
                                            "912c-3.565-2.594-8.397-2.596-11.965," +
                                            "0l-98.774,71.912h-33.58  C4.551,101.407," +
                                            "0,105.955,0,111.571v147.719c0,5.611,4.551," +
                                            "10.162,10.163,10.162h276.674c5.612,0,10.163-4" +
                                            ".551,10.163-10.162  V111.571C297,105.955,292." +
                                            "449,101.407,286.837,101.407z M148.5,50.284l70." +
                                            "221,51.123H78.279L148.5,50.284z M276.673,121." +
                                            "735v67.074  c-8.491-2.988-20.256-6.449-34.276-8" +
                                            ".789c-0.358-0.061-0.714-0.111-1.069-0.17c-2.768-23." +
                                            "344-22.654-41.514-46.715-41.514  c-25.943,0-47.051," +
                                            "21.125-47.051,47.09c0,1.199,0.221,2.348,0.601,3.416c-4" +
                                            ".567,1.783-9.05,3.768-13.436,5.963  c-54.138,27.098-97" +
                                            ".997,22.705-114.398,19.549v-92.619H276.673z M220.172,177" +
                                            ".596c-17.945-0.924-35.461,0.734-52.12,4.904  c1.459-13" +
                                            ".385,12.813-23.836,26.561-23.836C206.63,158.664,216.817" +
                                            ",166.649,220.172,177.596z M20.327,249.123v-14.061  c6.733" +
                                            ",1.123,15.799,2.121,26.788,2.121c24.366,0,58.15-4.9,96.708-24" +
                                            ".201c28.442-14.236,60.259-18.617,94.568-13.021  c17.389,2.836" +
                                            ",31.067,7.664,38.281,10.578v38.584H20.327z",
                new VisualSettingsViewModel(_themeRegistry, _themeService)),
            
            new SettingsNavItem("Profil verwalten", "M12,4A4,4 0 0,1 16,8A4,4 0 0,1 12,12A4,4 0 0,1 " +
                                                    "8,8A4,4 0 0,1 12,4M12,14C16.42,14 20,15.79 20,18V20H4V18C4," +
                                                    "15.79 7.58,14 12,14Z", 
                new ManageAccountViewModel(_cryptoService, _sessionService, _navigate))
        };

        SelectedNavItem = NavItems.FirstOrDefault();
    }
}