using SQLite;

namespace PasswordManager.Models
{
    public class Idea
    {
        // Unique primary key
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        // Foreign key connected to a users id
        [Indexed]
        public int UserId { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }



        public Idea() { }
        public Idea(int userId, string title, string description)
        {
            UserId = userId;
            Title = title;
            Description = description;
        }

        public Idea(int id, int userId, string title, string description)
        {
            Id = id;
            UserId = userId;
            Title = title;
            Description = description;
        }
    }
}
