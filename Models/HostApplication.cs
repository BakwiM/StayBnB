using System.ComponentModel.DataAnnotations;
namespace Staybnb.Models;
public class HostApplication
{
    public int Id { get; set; }
    [Required] public string ApplicantId { get; set; } = "";
    public ApplicationUser? Applicant { get; set; }
    [Required, MaxLength(150)] public string Title { get; set; } = "";
    [Required, MaxLength(2000)] public string Description { get; set; } = "";
    [Required, MaxLength(100)] public string City { get; set; } = "";
    [Required, MaxLength(500)] public string Address { get; set; } = "";
    [Required, MaxLength(80)] public string PropertyType { get; set; } = "";
    [Range(1,100000)] public decimal PricePerNight { get; set; }
    [Range(0,100000)] public decimal CleaningFee { get; set; }
    [Range(0,100000)] public decimal ServiceFee { get; set; }
    [Required] public string ImagePath { get; set; } = "";
    public ApplicationStatus Status { get; set; } = ApplicationStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
