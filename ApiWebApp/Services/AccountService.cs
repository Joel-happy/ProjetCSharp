using ApiWebApp.Models;
using ApiWebApp.DataAccess;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using System;
using System.Net;

namespace ApiWebApp.Services
{
    public class AccountService
    {
        // Service READ
        public static async Task<ApiResult<string>> GetAccountsAsync()
        {
            try
            {
                ApiResult<List<Account>> apiResult = await AccountRepository.GetAccountsRepositoryAsync();

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

                    return new ApiResult<string> { Result = jsonResult, StatusCode = apiResult.StatusCode };
                } 
                else
                {
                    return new ApiResult<string> { ErrorMessage = apiResult.ErrorMessage, StatusCode = apiResult.StatusCode };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAccountsAsync() : {ex.Message}");
                return new ApiResult<string> { ErrorMessage = "An error occured while processing the request", StatusCode = HttpStatusCode.InternalServerError };
            }
        }

        // Service READ by Id
        public static async Task<ApiResult<string>> GetAccountByIdAsync(string accountId)
        {
            try
            {
                // Check if the query parameter is empty or not an int
                if (string.IsNullOrEmpty(accountId) || !int.TryParse(accountId, out _))
                {
                    return new ApiResult<string> { ErrorMessage = "Invalid 'id' parameter", StatusCode = HttpStatusCode.BadRequest };
                }
                
                // Convert parameter to int
                int intAccountId = int.Parse(accountId);

                ApiResult<Account> apiResult = await AccountRepository.GetAccountByIdRepositoryAsync(intAccountId);
                
                if (apiResult.IsSuccess)
                {
                    if (!AllPropertiesAreNull(apiResult.Result))
                    {
                        string jsonResult = JsonSerializer.Serialize(apiResult.Result, new JsonSerializerOptions
                        {
                            WriteIndented = true
                        });
                        
                        return new ApiResult<string> { Result = jsonResult, StatusCode = HttpStatusCode.OK };
                    }
                    else
                    {
                        // Account not found
                        return new ApiResult<string> { ErrorMessage = "Account not found", StatusCode = HttpStatusCode.NotFound };
                    }
                }
                else
                {
                    return new ApiResult<string> { ErrorMessage = apiResult.ErrorMessage, StatusCode = apiResult.StatusCode };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAccountByIdAsync() : {ex.Message}");
                return new ApiResult<string> { ErrorMessage = "An error occured while processing the request", StatusCode = HttpStatusCode.InternalServerError };
            }
        }

        // Service CREATE
        public static async Task<ApiResult<string>> CreateAccountAsync(Account account)
        {
            try
            {
                if (IsAccountValid(account))
                {
                    ApiResult<string> apiResult = await AccountRepository.CreateAccountRepository(account);

                    if (apiResult.IsSuccess)
                    {
                        return new ApiResult<string> { Result = apiResult.Result, StatusCode = apiResult.StatusCode };
                    }
                    else
                    {
                        return new ApiResult<string> { ErrorMessage = apiResult.ErrorMessage, StatusCode = apiResult.StatusCode };
                    }
                }
                else
                {
                    return new ApiResult<string> { ErrorMessage = "Invalid 'account'", StatusCode = HttpStatusCode.BadRequest };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CreateAccountAsync() : {ex.Message}");
                return new ApiResult<string> { ErrorMessage = "An error occured while processing the request", StatusCode = HttpStatusCode.InternalServerError };
            }
        }

        //
        // Helper Functions
        //

        // Check if account is null / not found
        private static bool AllPropertiesAreNull(Account account)
        {
            return account.Id == 0 && account.Username == null && account.Email == null && account.Password == null;
        }

        // Check if account is valid for use
        private static bool IsAccountValid(Account account)
        {
            if (account == null)
            {
                return false;
            }

            return IsString(account.Username) && IsString(account.Email) && IsString(account.Password);
        }

        // Check if a value is a string
        private static bool IsString(object value)
        {
            return value is string stringValue && !string.IsNullOrEmpty(stringValue);
        }
    }
}
