using System.ComponentModel.DataAnnotations;
namespace Staybnb.Models;
public class Message
{
    public int Id { get; set; }
    [Required] public string SenderId { get; set; } = "";
    public ApplicationUser? Sender { get; set; }
    [Required] public string RecipientId { get; set; } = "";
    public ApplicationUser? Recipient { get; set; }
    [Required, MaxLength(200)] public string Subject { get; set; } = "";
    [Required, MaxLength(4000)] public string Body { get; set; } = "";
    public bool IsRead { get; set; }
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
}
