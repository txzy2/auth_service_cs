namespace MyMicroservice.Domain.Entities;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Login { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string ExtId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Навигационные свойства
    public int? RoleId { get; set; }
    public Roles? Role { get; set; }
    public ICollection<PermissionSettings> PermissionSettings { get; set; } = new List<PermissionSettings>();
}