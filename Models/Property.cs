using System.ComponentModel.DataAnnotations;

namespace Staybnb.Models;

public class Property
{
    public int Id { get; set; }
    [Required, MaxLength(150)] public string Title { get; set; } = "";
    [Required, MaxLength(2000)] public string Description { get; set; } = "";
    [Required, MaxLength(100)] public string City { get; set; } = "";
    [Required, MaxLength(500)] public string Address { get; set; } = "";
    [Required, MaxLength(80)] public string PropertyType { get; set; } = "";
    [Range(1, 100000)] public decimal PricePerNight { get; set; }
    [Range(0, 100000)] public decimal CleaningFee { get; set; }
    [Range(0, 100000)] public decimal ServiceFee { get; set; }
    public bool IsActive { get; set; }
    public string HostId { get; set; } = "";
    public ApplicationUser? Host { get; set; }
    public ICollection<PropertyImage> Images { get; set; } = new List<PropertyImage>();
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
    public CheckInProcess? CheckInProcess { get; set; }
}
