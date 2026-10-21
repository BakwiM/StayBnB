using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
namespace Staybnb.ViewModels;

public class HostApplicationVm
{
    [Required, MaxLength(150)] public string Title { get; set; } = "";
    [Required, MaxLength(2000)] public string Description { get; set; } = "";
    [Required] public string City { get; set; } = "";
    [Required] public string Address { get; set; } = "";
    [Required] public string PropertyType { get; set; } = "Apartment";
    [Range(1,100000)] public decimal PricePerNight { get; set; }
    [Range(0,100000)] public decimal CleaningFee { get; set; }
    [Range(0,100000)] public decimal ServiceFee { get; set; }
    [Required] public IFormFile? Image { get; set; }
}
public class BookingVm
{
    public int PropertyId { get; set; }
    [DataType(DataType.Date)] public DateTime CheckIn { get; set; } = DateTime.Today;
    [DataType(DataType.Date)] public DateTime CheckOut { get; set; } = DateTime.Today.AddDays(1);
    [Range(1,50)] public int Travelers { get; set; } = 1;
}
public class MessageVm
{
    [Required] public string RecipientId { get; set; } = "";
    [Required] public string Subject { get; set; } = "";
    [Required] public string Body { get; set; } = "";
}
