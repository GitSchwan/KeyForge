using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.Threading;
using KeyForge.Models;

namespace KeyForge.Views;

public partial class HomeView : UserControl
{
    private DispatcherTimer? _toastTimer;

    public HomeView()
    {
        InitializeComponent();
    }

    private async void MenuItem_OnClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (sender is MenuItem menuItem && menuItem.DataContext is VaultEntry entry)
        {
            if (string.IsNullOrWhiteSpace(entry.Password))
                return;

            var topLevel = TopLevel.GetTopLevel(this);
            var clipboard = topLevel?.Clipboard;

            if (clipboard is null)
                return;

            var copiedText = entry.Password;
            await clipboard.SetTextAsync(copiedText);

            ShowToast("In die Zwischenablage kopiert");

            _ = Task.Run(async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(20));

                var currentText = await clipboard.TryGetTextAsync();
                if (currentText == copiedText)
                {
                    await clipboard.SetTextAsync(string.Empty);
                }
            });
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