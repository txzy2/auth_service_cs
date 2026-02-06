namespace MyMicroservice.Domain.Entities
{
    public class Permissions
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        // Навигационное свойство
        public ICollection<PermissionSettings> PermissionSettings { get; set; } = new List<PermissionSettings>();
    }
}