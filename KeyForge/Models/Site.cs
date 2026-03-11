namespace KeyForge.Models;


public class Site
{

    public string Website { get; set; }
    public string Username { get; set; }
    public Site(string website , string username)
    {
        Website = website;
        Username = username;
    }
}
