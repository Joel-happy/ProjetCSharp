using System;
using System.Data.SQLite;
using System.IO;
using System.Threading.Tasks;

namespace ApiWebApp.DataAccess
{
    public class HelperRepository
    {
        // 
        // SYNC FUNCTIONS
        //

        // Get database file path
        public static string GetDatabaseFilePath()
        {
            string dir = "db";
            string fileName = "db.db";

            return Path.Combine(GetProjectRoot(), dir, fileName);
        }

        // Get project root path
        public static string GetProjectRoot()
        {
            string executableDirectory = AppDomain.CurrentDomain.BaseDirectory;
            return Path.Combine(executableDirectory, "../../../../");
        }

        // 
        // ASYNC FUNCTIONS
        //

        public static async Task<bool> IsUsernameAvailableRepositoryAsync(string username)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection($"Data Source ={GetDatabaseFilePath()}; Version = 3;"))
                {
                    await connection.OpenAsync();

                    using (SQLiteCommand command = new SQLiteCommand("SELECT COUNT(*) FROM account WHERE username = @username", connection))
                    {
                        command.Parameters.AddWithValue("@username", username);

                        int count = Convert.ToInt32(await command.ExecuteScalarAsync());
                        return count == 0;
                    }
                }
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine($"SQLite error in IsUsernameInUseRepositoryAsync() : {ex.Message}");
                return false;
            }
        }

        public static async Task<bool> IsEmailAvailableRepositoryAsync(string email)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection($"Data Source ={GetDatabaseFilePath()}; Version = 3;"))
                {
                    await connection.OpenAsync();

                    using (SQLiteCommand command = new SQLiteCommand("SELECT COUNT(*) FROM account WHERE email = @email", connection))
                    {
                        command.Parameters.AddWithValue("@email", email);

                        int count = Convert.ToInt32(await command.ExecuteScalarAsync());
                        return count == 0;
                    }
                }
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine($"SQLite error in IsEmailInUseRepositoryAsync() : {ex.Message}");
                return false;
            }
        }
    }
}
