namespace AuthAspNet.Models
{
    public class User
    {
        public Guid Id { get; } = Guid.NewGuid();
        public string Name { get; set; }
        public string Email { get; set; }
        public string[] Roles { get; set; }
    }
}
