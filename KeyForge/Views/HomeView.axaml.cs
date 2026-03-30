using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using System.Threading.Tasks;
using KeyForge.Models;

namespace KeyForge.Views;

public partial class HomeView : UserControl
{
    public HomeView()
    {
        InitializeComponent();
    }

    private async void DataGrid_DoubleTapped(object? sender, TappedEventArgs e)
    {
        if (sender is not DataGrid grid)
            return;

        if (grid.SelectedItem is not VaultEntry entry)
            return;

        if (string.IsNullOrWhiteSpace(entry.Password))
            return;

        var topLevel = TopLevel.GetTopLevel(this);
        var clipboard = topLevel?.Clipboard;

        if (clipboard is null)
            return;

        var copiedText = entry.Password;
        await clipboard.SetTextAsync(copiedText);

        _ = Task.Run(async () =>
        {
            await Task.Delay(TimeSpan.FromSeconds(20));

            var currentText = await clipboard.GetTextAsync();
            if (currentText == copiedText)
            {
                await clipboard.SetTextAsync(string.Empty);
            }
        });
    }
}