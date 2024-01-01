using ApiWebApp.Models;
using ApiWebApp.DataAccess;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using System;

namespace ApiWebApp.Services
{
    public class AccountService
    {
        // Service READ
        public static async Task<ApiResult<string>> GetAccountsAsync()
        {
            try
            {
                ApiResult<List<Account>> apiResult = await AccountRepository.GetAccountsDatabaseAsync();

                if (apiResult.IsSuccess)
                {
                    // Encapsulate list of accounts to provide a structured JSON response with a 'accounts' wrapper
                    AccountList accountList = new AccountList
                    {
                        accounts = apiResult.Result
                    };

                    // Serialize list of accounts to JSON
                    string jsonResult = JsonSerializer.Serialize(accountList, new JsonSerializerOptions
                    {
                        // Format to improve readability
                        WriteIndented = true
                    });

                    return new ApiResult<string> { Result = jsonResult };
                } 
                else
                {
                    return new ApiResult<string> { ErrorMessage = "An error occured while processing the request" };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAccountsAsync() : {ex.Message}");
                return new ApiResult<string> { ErrorMessage = "An error occured while processing the request" };
            }
        }

        // Service READ by Id
        public static async Task<ApiResult<string>> GetAccountByIdAsync(string accountId)
        {
            try
            {
                // Check if parameter is empty or not an int
                if (string.IsNullOrEmpty(accountId) || !int.TryParse(accountId, out _))
                {
                    return new ApiResult<string> { ErrorMessage = "Invalid 'id' parameter" };
                }

                // Convert parameter to int
                int intAccountId = int.Parse(accountId);

                ApiResult<Account> apiResult = await AccountRepository.GetAccountByIdDatabaseAsync(intAccountId);

                if (apiResult.IsSuccess)
                {
                    if (!AllPropertiesAreNull(apiResult.Result))
                    {
                        string jsonResult = JsonSerializer.Serialize(apiResult.Result, new JsonSerializerOptions
                        {
                            WriteIndented = true
                        });

                        return new ApiResult<string> { Result = jsonResult };
                    }
                    else
                    {
                        // Account not found
                        return new ApiResult<string> { ErrorMessage = "Account not found" };
                    }
                }
                else
                {
                    return new ApiResult<string> { ErrorMessage = "An error occured while processing the request" };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAccountByIdAsync() : {ex.Message}");
                return new ApiResult<string> { ErrorMessage = "An error occured while processing the request" };
            }
        }

        // Check if the account is null / not found
        private static bool AllPropertiesAreNull(Account account)
        {
            return account.Id == 0 && account.Username == null && account.Email == null && account.Password == null;
        }
    }
}
