using System;
using KeyForge.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;

namespace KeyForge.ViewModels;

public class HomeViewModel : ViewModelBase
{
    public ObservableCollection<VaultEntry> Data { get; }
    private readonly Action _navigateToAdd;
    public IRelayCommand NavigateToAddCommand { get; }

    public HomeViewModel(Action navigateToAdd) //https://docs.avaloniaui.net/docs/reference/controls/datagrid/
    {
        //Add Button
        NavigateToAddCommand = new RelayCommand(navigateToAdd);
        _navigateToAdd = navigateToAdd;

        //Table
        var data = new List<VaultEntry>
        {
            new VaultEntry("youtube.com", "Doe"),
            new VaultEntry("github.com", "Doe"),
            new VaultEntry("youtrack.cloud", "Smith")
        };

        Data = new ObservableCollection<VaultEntry>(data);
    }

    private void NavigateToAdd()
    {
        _navigateToAdd();
    }
}
