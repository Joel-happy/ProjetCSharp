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
        // Get accounts from database
        public static async Task<ApiResult<List<Account>>> GetAccountsDatabaseAsync()
        {
            try
            {
                List<Account> accounts = new List<Account>();

                using (SQLiteConnection connection = new SQLiteConnection($"Data Source ={GetDatabaseFilePath()}; Version = 3;"))
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

        // Get account by id from database
        public static async Task<ApiResult<Account>> GetAccountByIdDatabaseAsync(int accountId)
        {
            try
            {
                Account account = new Account();

                using (SQLiteConnection connection = new SQLiteConnection($"Data Source ={GetDatabaseFilePath()}; Version = 3;"))
                {
                    await connection.OpenAsync();

                    using (SQLiteCommand command = new SQLiteCommand($"SELECT * FROM account WHERE account_id={accountId}", connection))
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
                            account = new Account
                            {
                                Id = id,
                                Username = username,
                                Email = email,
                                Password = password
                            };
                        }
                        connection.Close();
                    }
                }
                return new ApiResult<Account> { Result = account };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAccountByIdDatabaseAsync() : {ex.Message}");
                return new ApiResult<Account> { ErrorMessage = "An error occured while processing the request" };
            }
        }

        // Get database file path
        private static string GetDatabaseFilePath()
        {
            string dir = "db";
            string fileName = "db.sqlite";

            return Path.Combine(GetProjectRoot(), dir, fileName);
        }


        // Get project root path
        private static string GetProjectRoot()
        {
            string executableDirectory = AppDomain.CurrentDomain.BaseDirectory;
            return Path.Combine(executableDirectory, "../../../../");
        }
    }
}