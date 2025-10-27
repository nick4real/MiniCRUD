using System.ComponentModel.DataAnnotations;

namespace MiniCRUD.Domain.Models;

public class Product
{
    [Key]
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Adjective { get; set; } = string.Empty;
    public string Material { get; set; } = string.Empty;
    public decimal Price { get; set; }
}
