namespace AuthAspNet.Models
{
    public class User
    {
        public User(string name, string email, string[] roles)
        {
            Name = name;
            Email = email;
            Roles = roles;
        }

        public Guid Id { get; } = Guid.NewGuid();
        public string Name { get; private set; }
        public string Email { get; private set; }
        public string[] Roles { get; private set; }
    }
}
