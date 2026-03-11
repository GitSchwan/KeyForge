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
        if (currentViewModel is LoginViewModel)
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
            Background = Brushes.Black;
        }
    }
}