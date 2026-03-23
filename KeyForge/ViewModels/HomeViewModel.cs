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
            new VaultEntry(1, "youtube.com","James",0, "DontLook"),
            new VaultEntry(2, "github.com", "Tommy",0, "DontLook"),
            new VaultEntry(3, "youtrack.cloud", "Giraffe02",0, "DontLook")
        };

        Data = new ObservableCollection<VaultEntry>(data);
    }

    private void NavigateToAdd()
    {
        _navigateToAdd();
    }
}
