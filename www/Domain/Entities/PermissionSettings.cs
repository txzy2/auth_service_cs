namespace MyMicroservice.Domain.Entities
{
    public class PermissionSettings
    {
        public int Id { get; set; }
        
        // Foreign Keys
        public int UserId { get; set; }
        public int PermissionId { get; set; }

        // Навигационные свойства
        public User User { get; set; } = null!;
        public Permissions Permission { get; set; } = null!;
    }
}