using System; 
using System.ComponentModel; 
using Avalonia; 
using Avalonia.Controls; 
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media; using KeyForge.ViewModels; 
using KeyForge.Data; 
using KeyForge.Models; 
using KeyForge.Services; 

namespace KeyForge.Views; 
public partial class MainWindow : Window { 
    
    private readonly ThemeService _themeService; 
    
    public MainWindow(ThemeService themeService) { 
        InitializeComponent(); 
        Opened += (_, _) => AttachToViewModel(); 
        DataContextChanged += (_, _) => AttachToViewModel(); 
        _themeService = themeService ?? throw new ArgumentNullException(nameof(themeService));
        _themeService.ThemeChanged += OnThemeChanged; // Subscribe to theme changes
    }
    
    private void Minimize_Click(object? sender, RoutedEventArgs e)
    {
        this.WindowState = WindowState.Minimized;
    }

    private void Close_Click(object? sender, RoutedEventArgs e)
    {
        this.Close();
    }
    
    private void TitleBar_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        BeginMoveDrag(e);
    }

    private void AttachToViewModel()
    {
        if (DataContext is not MainWindowViewModel vm) return; 
        vm.PropertyChanged -= MainWindowViewModelOnPropertyChanged; 
        vm.PropertyChanged += MainWindowViewModelOnPropertyChanged; 
        UpdateWindowAppearance(vm.CurrentViewModel);
    }

    private void MainWindowViewModelOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (sender is not MainWindowViewModel vm) return;
        if (e.PropertyName == nameof(MainWindowViewModel.CurrentViewModel))
        {
            UpdateWindowAppearance(vm.CurrentViewModel); 
        }
    }

    private void UpdateWindowAppearance(ViewModelBase? currentViewModel)
    {
        if (currentViewModel is LoginViewModel or CreateUserViewModel)
        {
            TransparencyLevelHint = [ 
                WindowTransparencyLevel.AcrylicBlur, 
                WindowTransparencyLevel.Blur, 
                WindowTransparencyLevel.Transparent ]; 
            
            Background = Brushes.Transparent;
        }
        else
        {
            TransparencyLevelHint = [WindowTransparencyLevel.None]; 
            OnThemeChanged(_themeService.GetCurrentTheme());
        }
    }

    private void OnThemeChanged(Theme? theme)
    {
        if (theme is null) return; 
        
        Background = new LinearGradientBrush
        {
            StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative), 
            EndPoint = new RelativePoint(1, 1, RelativeUnit.Relative), 
            GradientStops = { 
                new GradientStop { Color = Color.Parse(theme.StartColor), Offset = 0 }, 
                new GradientStop { Color = Color.Parse(theme.EndColor), Offset = 1 } }
        }; 
        
        Application.Current.Resources["Brush.Foreground"] = new SolidColorBrush(Color.Parse(theme.ForegroundColor));
        Application.Current.Resources["Brush.Primary"] = new SolidColorBrush(Color.Parse(theme.ForegroundColor)); // Not optimal
        Application.Current.Resources["Brush.Secondary"] = new SolidColorBrush(Color.Parse(theme.StartColor)); // Not optimal
    } 
}

