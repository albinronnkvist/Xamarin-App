using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using PasswordManager.Models;
using SQLite;

namespace PasswordManager.Services
{
    public static class UserService
    {
        private static SQLiteAsyncConnection db;
        private static async Task Init()
        {
            // If a database connection already exists, dont create a new one.
            if (db != null)
            {
                return;
            }

            // Create a database path, which is the location of a file on the device where we store our data.
            // In this case the file is called "Ideas.db" and is stored in the system special folder "MyDocuments".
            // The system special folders are located at different paths depending on the users platform(iOS/Android/Windows etc).
            // So we need to specify the unique path for every platform by the help of the Environment class and it's GetFolderPath method + SpecialFolder enum.
            string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Ideas.db");

            // Create a new database connection
            db = new SQLiteAsyncConnection(dbPath);

            // Create a table where we store our users
            await db.CreateTableAsync<User>();
        }

        public static async Task<int> RegisterUser(string username, string password)
        {
            // Init database
            await Init();

            // Generate salt
            // A salt is a random byte array that is used as an additional input to a one-way hash function.
            // A different salt for each user prevents the password from having the same hashed values as other passwords, so it's more difficult to catch common passwords and you can only attack one password at a time.
            // This makes the password safe against rainbow tables which are precomputed dictionaries of plaintext passwords and their corresponding hash values that can be used to find out what plaintext password produces a particular hash.
            // The password is still vulnerable to dictionary attacks since the salt is stored directly in the database.
            RandomNumberGenerator rng = RandomNumberGenerator.Create();
            byte[] saltBytes = new byte[16];
            rng.GetBytes(saltBytes);
            string saltText = Convert.ToBase64String(saltBytes);

            // Generate the salted and hashed password with the password input and the randomly generated salt.
            string securePassword = SaltAndHashPassword(password, saltText);

            // Create a new user object with the username, generated salt and the salted+hashed password
            User user = new User(username, saltText, securePassword);

            // Insert the object into the database with the InsertAsync task
            // This task returns the amount of rows added to the table, which we can store in a variable.
            int createdUser = await db.InsertAsync(user);

            // Return amount of rows added (created users)
            return createdUser;
        }

        public static async Task<User> LoginUser(string username, string password)
        {
            await Init();

            // Get a user object from the database where the input username equals a username in the database
            // The FirstOrDefaultAsync task returns the first element of the query, or null if no element was found.
            User user = await db.Table<User>().Where(u => u.Username.Equals(username)).FirstOrDefaultAsync();

            // If a user was found
            if (user != null)
            {
                // Generate a hashed password with the input password and the users salt which is stored in the database
                string securePassword = SaltAndHashPassword(password, user.Salt);

                // If the generated hashed password matches the hashed password in the database, we return a user object which means the user provided the correct credentials and is logged in.
                // If the passwords do not match, we return null which means the user failed to provide the correct credentials.
                return securePassword == user.Password ? user : null;
            }
            else
            {
                return null;
            }
        }

        public static async Task<int> DeleteUser(int id)
        {
            await Init();

            // Delete a row in the users table where the primary key matches the value of the id parameter
            int deletedUser = await db.DeleteAsync<User>(id);

            // Delete the users ideas
            var query = db.Table<Idea>().Where(idea => idea.UserId.Equals(id));
            await query.DeleteAsync();

            // Return number of deleted users
            return deletedUser;
        }

        // Hash passwords
        // Hashing is a one-way process of scrambling raw information into a value that can't be turned back to it's original value.
        // It takes some text and passes it through a hash function that performs mathematical operations on the text and produces a hash value.
        private static string SaltAndHashPassword(string password, string salt)
        {
            // Create a new instance of the SHA256 class which is a commonly used hashing algorithm.
            SHA256 sha = SHA256.Create();

            // Concatenate the password and the salt
            string saltedPassword = password + salt;

            // Create and return the hash value
            return Convert.ToBase64String(sha.ComputeHash(Encoding.Unicode.GetBytes(saltedPassword)));
        }
    }
}
