using AuthTest.Models;
using System.ComponentModel.DataAnnotations;

namespace AuthTest.Models
{
    public class User
    {
        //Use construtor e set private nas entidades, use metodos pra acessar elas fora do contexto
        public User(string name, string email, string password, ICollection<UserRole> userRoles)
        {
            Name = name;
            Email = email;
            Password = password;
            UserRoles = userRoles;
        }

        [Key]
        public int Id { get; private set; }
        public string Name { get; private set; }
        public string Email { get; private set; }
        public string Password { get; private set; }
        public virtual ICollection<UserRole> UserRoles { get; private set; }

        public void Update(string name, string email, string password, ICollection<UserRole> userRoles)
        {
            Name = name;
            Email = email;
            Password = password;
            UserRoles = userRoles;
        }
    }
}
