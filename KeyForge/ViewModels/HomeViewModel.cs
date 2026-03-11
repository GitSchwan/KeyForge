using KeyForge.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace KeyForge.ViewModels;

public class HomeViewModel : ViewModelBase
{
    public ObservableCollection<Site> Data { get; }

    public HomeViewModel() //https://docs.avaloniaui.net/docs/reference/controls/datagrid/
    {
        var data = new List<Site>
        {
            new Site("youtube.com", "Doe"),
            new Site("github.com", "Doe"),
            new Site("youtrack.cloud", "Smith")
        };
        
        Data = new ObservableCollection<Site>(data);
    }
}
