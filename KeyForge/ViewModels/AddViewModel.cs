using System;
using CommunityToolkit.Mvvm.Input;
namespace KeyForge.ViewModels;

public partial class AddViewModel : ViewModelBase
{
    private readonly Action _navigateToHome;
    public IRelayCommand ToHomeView { get; }

    public AddViewModel(Action navigateToHome)
    {
        _navigateToHome = navigateToHome;
        ToHomeView = new RelayCommand(GoBackToHomeView);
    }
    private void GoBackToHomeView() 
    { 
        _navigateToHome();
    }
}