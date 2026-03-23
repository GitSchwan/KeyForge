using System.ComponentModel;
using Avalonia.Controls;
using Avalonia.Media;
using KeyForge.ViewModels;

namespace KeyForge.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        Opened += (_, _) => AttachToViewModel();
        DataContextChanged += (_, _) => AttachToViewModel();
    }

    private void AttachToViewModel()
    {
        if (DataContext is not MainWindowViewModel vm)
            return;

        vm.PropertyChanged -= MainWindowViewModelOnPropertyChanged;
        vm.PropertyChanged += MainWindowViewModelOnPropertyChanged;

        UpdateWindowAppearance(vm.CurrentViewModel);
    }
    
    private void MainWindowViewModelOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (sender is not MainWindowViewModel vm)
            return;

        if (e.PropertyName == nameof(MainWindowViewModel.CurrentViewModel))
        {
            UpdateWindowAppearance(vm.CurrentViewModel);
        }
    }
    
    private void UpdateWindowAppearance(ViewModelBase? currentViewModel)
    {
        if (currentViewModel is LoginViewModel or CreateUserViewModel)
        {
            TransparencyLevelHint =
            [
                WindowTransparencyLevel.AcrylicBlur,
                WindowTransparencyLevel.Blur,
                WindowTransparencyLevel.Transparent
            ];
            Background = Brushes.Transparent;
        }
        else
        {
            TransparencyLevelHint = [WindowTransparencyLevel.None];
            Background = new LinearGradientBrush
            {
                StartPoint = new Avalonia.RelativePoint(0, 0, Avalonia.RelativeUnit.Relative),
                EndPoint = new Avalonia.RelativePoint(1, 1, Avalonia.RelativeUnit.Relative),
                GradientStops =
                {
                    new GradientStop { Color = Color.FromRgb(128, 0, 128), Offset = 0 },
                    new GradientStop { Color = Color.FromRgb(151, 17, 56), Offset = 1 }
                }
            };
        }
    }
}