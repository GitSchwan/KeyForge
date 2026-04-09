namespace KeyForge.ViewModels;

public class SettingsNavItem
{
    public string Title { get; }
    public string Icon { get; }
    public ViewModelBase ViewModel { get; }

    public SettingsNavItem(string title, string icon, ViewModelBase viewModel)
    {
        Title = title;
        Icon = icon;
        ViewModel = viewModel;
    }
}
