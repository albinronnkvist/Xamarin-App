using SQLite;

namespace PasswordManager.Models
{
    public class User
    {
        // Unique primary key
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Username { get; set; }
        public string Salt { get; set; }
        public string Password { get; set; }

        public User() { }
        public User(string username, string salt, string password)
        {
            Username = username;
            Salt = salt;
            Password = password;
        }

        public User(int id, string username, string salt, string password)
        {
            Id = id;
            Username = username;
            Salt = salt;
            Password = password;
        }
    }
}
