namespace Staybnb.Models;
public class GuestDocument
{
    public int Id { get; set; }
    public int BookingId { get; set; }
    public Booking? Booking { get; set; }
    public string DocumentPath { get; set; } = "";
    public bool Verified { get; set; }
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
}
