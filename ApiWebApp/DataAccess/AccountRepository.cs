using ApiWebApp.Models;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Net;
using System.Threading.Tasks;

namespace ApiWebApp.DataAccess
{
    public class AccountRepository
    {
        // Get accounts
        public static async Task<ApiResult<List<Account>>> GetAccountsRepositoryAsync()
        {
            try
            {
                List<Account> accounts = new List<Account>();

                using (SQLiteConnection connection = new SQLiteConnection($"Data Source ={HelperRepository.GetDatabaseFilePath()}; Version = 3;"))
                {
                    await connection.OpenAsync();

                    using (SQLiteCommand command = new SQLiteCommand("SELECT * FROM account", connection))
                    {
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
                }
                return new ApiResult<List<Account>> { Result = accounts, StatusCode = HttpStatusCode.OK };
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine($"SQLite error in GetAccountsRepositoryAsync() : {ex.Message}");
                return new ApiResult<List<Account>> { ErrorMessage = "A database error occurred", StatusCode = HttpStatusCode.InternalServerError };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAccountsRepositoryAsync() : {ex.Message}");
                return new ApiResult<List<Account>> { ErrorMessage = "An error occured while processing the request", StatusCode = HttpStatusCode.InternalServerError };
            }
        }

        // Get account by id
        public static async Task<ApiResult<Account>> GetAccountByIdRepositoryAsync(int accountId)
        {
            try
            {
                Account account = new Account();

                using (SQLiteConnection connection = new SQLiteConnection($"Data Source ={HelperRepository.GetDatabaseFilePath()}; Version = 3;"))
                {
                    await connection.OpenAsync();

                    // Use parameterized query to prevent SQL injection
                    using (SQLiteCommand command = new SQLiteCommand($"SELECT * FROM account WHERE account_id = @AccountId", connection))
                    {
                        command.Parameters.AddWithValue("@AccountId", accountId);

                        using (SQLiteDataReader reader = (SQLiteDataReader)await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                int id = reader.GetInt32(0);
                                string username = reader.GetString(1);
                                string email = reader.GetString(2);
                                string password = reader.GetString(3);

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
                }

                return new ApiResult<Account> { Result = account, StatusCode = HttpStatusCode.OK };
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine($"SQLite error in GetAccountByIdRepositoryAsync() : {ex.Message}");
                return new ApiResult<Account> { ErrorMessage = "A database error occurred", StatusCode = HttpStatusCode.InternalServerError };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAccountByIdRepositoryAsync() : {ex.Message}");
                return new ApiResult<Account> { ErrorMessage = "An error occured while processing the request", StatusCode = HttpStatusCode.InternalServerError };
            }
        }

        // Create account
        public static async Task<ApiResult<string>> CreateAccountRepository(Account account)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection($"Data Source ={HelperRepository.GetDatabaseFilePath()}; Version = 3;"))
                {
                    await connection.OpenAsync();
                    
                    using (SQLiteCommand command = new SQLiteCommand("INSERT INTO account(username,email,password) VALUES(@Username, @Email, @Password)", connection))
                    {
                        // Add parameters to command
                        command.Parameters.AddWithValue("@Username", account.Username);
                        command.Parameters.AddWithValue("@Email", account.Email);
                        command.Parameters.AddWithValue("@Password", account.Password);

                        // Execute the command
                        await command.ExecuteNonQueryAsync();
                    }
                }
                
                return new ApiResult<string> { Result = "Account created successfully", StatusCode = HttpStatusCode.Created };
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine($"SQLite error in CreateAccountRepository() : {ex.Message}");
                return new ApiResult<string> { ErrorMessage = "A database error occurred", StatusCode = HttpStatusCode.InternalServerError };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CreateAccountRepository() : {ex.Message}");
                return new ApiResult<string> { ErrorMessage = "An error occured while processing the request", StatusCode = HttpStatusCode.InternalServerError };
            }
        }
    }
}