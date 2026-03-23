using System.Collections.Generic;

namespace KeyForge.Models;

public class User
{
    public int Id { get; set; }

    public string Name { get; set; }

    public byte[] MasterPassword { get; set; }

    public List<VaultEntry> VaultEntries { get; set; }


    public User(string name, byte[] masterPassword)
    {
        Name = name;
        MasterPassword = masterPassword;
    }
    
}

