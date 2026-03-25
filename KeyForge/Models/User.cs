using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KeyForge.Models;

public class User
{
    [MaxLength(100)]
    public int Id { get; set; }

    [MaxLength(200)]
    public string Name { get; set; }

    [MaxLength(500)]
    public string MasterPassword { get; set; }

    public List<VaultEntry> VaultEntries { get; set; }


    public User(string name, string masterPassword)
    {
        Name = name;
        MasterPassword = masterPassword;
    }
    
}

