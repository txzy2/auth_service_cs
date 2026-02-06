namespace MyMicroservice.Domain.Entities
{
    public class Roles
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        // Навигационное свойство
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}       