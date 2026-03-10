
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace KeyForge.ViewModels;

public class HomeViewModel : ViewModelBase
{
    public ObservableCollection<Sites> Data { get; }

    public HomeViewModel()
    {
        var data = new List<Sites>
        {
            new Sites("youtube.com", "Doe"),
            new Sites("github.com", "Doe"),
            new Sites("youtrack.cloud", "Smith")
        };
        Data = new ObservableCollection<Sites>(data);
    }
}


public class Sites
{

    public string Website { get; set; }
    public string Username { get; set; }
    public Sites(string website , string username)
    {
        Website = website;
        Username = username;
    }
}