using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace KeyForge.ViewModels;

public partial class ConfirmPopUpViewModel : ObservableObject
{
    public string Message { get; }

    [RelayCommand]
    private void Confirm()
    {
        // Wird in der View ausgewertet
    }

    [RelayCommand]
    private void Cancel()
    {
        // Wird in der View ausgewertet
    }

    public ConfirmPopUpViewModel(string message)
    {
        Message = message;
    }
}