using System.ComponentModel.DataAnnotations;
namespace Staybnb.Models;
public class Notification
{
    public int Id { get; set; }
    [Required] public string UserId { get; set; } = "";
    [Required, MaxLength(200)] public string Title { get; set; } = "";
    [Required, MaxLength(1000)] public string Message { get; set; } = "";
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
