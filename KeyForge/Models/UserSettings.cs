using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KeyForge.Models;

public class UserSettings
{
    [Key]
    [ForeignKey("User")]
    public int UserId { get; set; }

    [MaxLength(100)]
    public string? PreferredThemeId { get; set; }

    public virtual User User { get; set; } = null!;
    
    public UserSettings(int userId, string? preferredThemeId)
    {
        UserId = userId;
        PreferredThemeId = preferredThemeId;
    }
}