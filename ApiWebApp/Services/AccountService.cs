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
        public static async Task<string> GetAccountsAsync()
        {
            try
            {
                // Business logic (if needed)

                List<Account> accounts = await AccountRepository.GetAccountsDatabaseAsync();

                // Encapsulate list of accounts to provide a structured JSON response with a 'accounts' wrapper
                AccountList accountList = new AccountList
                {
                    accounts = accounts
                };

                // Serialize list of accounts to JSON
                string jsonResult = JsonSerializer.Serialize(accountList, new JsonSerializerOptions
                {
                    // Format to improve readability
                    WriteIndented = true
                });

                return jsonResult;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAccountsAsync() : {ex.Message}");
                return "An error occured while processing the request";
            }
        }
    }
}
