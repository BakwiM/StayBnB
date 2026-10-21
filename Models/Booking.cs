using System.ComponentModel.DataAnnotations;
namespace Staybnb.Models;
public class Booking
{
    public int Id { get; set; }
    public int PropertyId { get; set; }
    public Property? Property { get; set; }
    [Required] public string GuestId { get; set; } = "";
    public ApplicationUser? Guest { get; set; }
    [DataType(DataType.Date)] public DateTime CheckIn { get; set; }
    [DataType(DataType.Date)] public DateTime CheckOut { get; set; }
    [Range(1,50)] public int Travelers { get; set; }
    public int Nights => Math.Max(0, (CheckOut.Date - CheckIn.Date).Days);
    public decimal TotalPrice { get; set; }
    public BookingStatus Status { get; set; } = BookingStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
