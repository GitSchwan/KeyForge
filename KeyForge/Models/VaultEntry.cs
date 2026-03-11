namespace KeyForge.Models;


public class VaultEntry
{

    public int Id { get; set; }
    public string Website { get; set; }
    public string Username { get; set; }
    public string Password { get; set; } = string.Empty;
    
    public VaultEntry(string website , string username)
    {
        Website = website;
        Username = username;
    }
}
