using ApiWebApp.Models;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Threading.Tasks;

namespace ApiWebApp.DataAccess
{
    public class AccountRepository
    {
        public static async Task<ApiResult<List<Account>>> GetAccountsDatabaseAsync()
        {
            try
            {
                // Construct full path to the SQLite database file
                string databasePath = Path.Combine(GetProjectRoot(), "db", "db.sqlite");

                List<Account> accounts = new List<Account>();

                using (SQLiteConnection connection = new SQLiteConnection($"Data Source ={databasePath}; Version = 3;"))
                {
                    await connection.OpenAsync();

                    using (SQLiteCommand command = new SQLiteCommand("SELECT * FROM account", connection))
                    using (SQLiteDataReader reader = (SQLiteDataReader)await command.ExecuteReaderAsync())
                    {
                        // Iterate through the results of the query
                        while (await reader.ReadAsync())
                        {
                            // Extract values from the database record
                            int id = reader.GetInt32(0);
                            string username = reader.GetString(1);
                            string email = reader.GetString(2);
                            string password = reader.GetString(3);

                            // Create new 'account'
                            Account account = new Account
                            {
                                Id = id,
                                Username = username,
                                Email = email,
                                Password = password
                            };
                            accounts.Add(account);
                        }
                        connection.Close();
                    }
                }
                return new ApiResult<List<Account>> { Result = accounts };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAccountsDatabaseAsync() : {ex.Message}");
                return new ApiResult<List<Account>> { ErrorMessage = "An error occured while processing the request" };
            }
        }

        // Get project root path
        static string GetProjectRoot()
        {
            string executableDirectory = AppDomain.CurrentDomain.BaseDirectory;
            return Path.Combine(executableDirectory, "../../../../");
        }
    }
}