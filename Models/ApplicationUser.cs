using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Staybnb.Models;

public class ApplicationUser : IdentityUser
{
    [MaxLength(120)] public string FullName { get; set; } = "";
    public ICollection<Property> Properties { get; set; } = new List<Property>();
    public ICollection<HostApplication> HostApplications { get; set; } = new List<HostApplication>();
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
}
