namespace KeyForge.Models;


public class VaultEntry
{
    public int Id { get; set; }

    public string Website { get; set; }

    public string Websiteusername { get; set; }

    public int UserId { get; set; }

    public string Password { get; set; }

    public User? User { get; set; }

    public VaultEntry(int id, string website, string websiteusername ,int userId, string password)
    {
        Id = id;
        Website = website;
        Websiteusername = websiteusername;
        UserId = userId;
        Password = password;
    }
}
