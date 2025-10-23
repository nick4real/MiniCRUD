using System.ComponentModel.DataAnnotations;

namespace MiniCRUD.Domain.Models
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }
        public string Login { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string AvatarUrl { get; set; } = string.Empty;
    }
}
