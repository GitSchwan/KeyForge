using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.Threading;
using KeyForge.Models;

using KeyForge.ViewModels;

namespace KeyForge.Views;

public partial class HomeView : UserControl
{
    private DispatcherTimer? _toastTimer;

    public HomeView()
    {
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;
    }

    private void OnDataContextChanged(object? sender, EventArgs e)
    {
        if (DataContext is HomeViewModel vm)
        {
            vm.ClipboardCopyHappened += (s, msg) => ShowToast(msg);
        }
    }

    private void ShowToast(string message)
    {
        ToastText.Text = message;
        ToastHost.IsVisible = true;

        _toastTimer?.Stop();
        _toastTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(2)
        };

        _toastTimer.Tick += (_, _) =>
        {
            _toastTimer?.Stop();
            ToastHost.IsVisible = false;
        };

        _toastTimer.Start();
    }
}