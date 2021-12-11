using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using PasswordManager.Models;
using SQLite;

namespace PasswordManager.Services
{
    public static class IdeasService
    {
        private static SQLiteAsyncConnection db;
        private static async Task Init()
        {
            if (db != null)
            {
                return;
            }

            string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Ideas.db");
            db = new SQLiteAsyncConnection(dbPath);

            // Create an ideas table
            await db.CreateTableAsync<Idea>();
        }

        public static async Task<int> CreateIdea(int userId, string title, string description)
        {
            await Init();

            // Create a new idea object
            var idea = new Idea(userId, title, description);

            // Insert the idea object into the ideas table
            int created = await db.InsertAsync(idea);

            // Return number of rows created
            return created;
        }

        public static async Task<IEnumerable<Idea>> ReadIdeas(int userId)
        {
            await Init();

            // Get all rows where the userId parameter matches the foreign key "UserId" in the ideas table.
            // The ToListAsync task returns the result as a List
            var ideas = await db.Table<Idea>().Where(idea => idea.UserId.Equals(userId)).ToListAsync();

            // Return list of ideas
            return ideas;
        }

        public static async Task<int> DeleteIdea(int id)
        {
            await Init();

            // Delete a row in the ideas table where the primary key matches the value of the id parameter
            int deleted = await db.DeleteAsync<Idea>(id);

            // Return number of rows deleted
            return deleted;
        }
    }
}
