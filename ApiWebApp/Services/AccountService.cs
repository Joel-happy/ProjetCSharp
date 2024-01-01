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
        public static async Task<ApiResult<string>> GetAccountsAsync()
        {
            try
            {
                // Business logic (if needed)

                ApiResult<List<Account>>  apiResult = await AccountRepository.GetAccountsDatabaseAsync();

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
    }
}
