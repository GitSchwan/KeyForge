using Avalonia.Controls;
using Avalonia.Interactivity;
using KeyForge.ViewModels;

namespace KeyForge.Views;

public partial class ConfirmPopUpView : Window
{
    public ConfirmPopUpView()
    {
        InitializeComponent();
    }

    private void Confirm_Click(object? sender, RoutedEventArgs e)
    {
        Close(true);
    }

    private void Cancel_Click(object? sender, RoutedEventArgs e)
    {
        Close(false);
    }
}