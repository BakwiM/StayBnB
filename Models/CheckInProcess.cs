using System.ComponentModel.DataAnnotations;
namespace Staybnb.Models;
public class CheckInProcess
{
    public int Id { get; set; }
    public int PropertyId { get; set; }
    public Property? Property { get; set; }
    [Required, MaxLength(2000)] public string Rules { get; set; } = "";
    [Required, MaxLength(1000)] public string RequiredDocuments { get; set; } = "ID or Passport";
}
