using System.ComponentModel.DataAnnotations;

namespace Staybnb.Models;
public class PropertyImage
{
    public int Id { get; set; }
    public int PropertyId { get; set; }
    public Property? Property { get; set; }
    [Required] public string ImagePath { get; set; } = "";
}
