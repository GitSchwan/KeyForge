using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace KeyForge.ViewModels;

public partial class ConfirmPopUpViewModel : ObservableObject
{
    public string Message { get; }

    [RelayCommand]
    private void Confirm()
    {
    }

    [RelayCommand]
    private void Cancel()
    {
    }

    public ConfirmPopUpViewModel(string message)
    {
        Message = message;
    }
}