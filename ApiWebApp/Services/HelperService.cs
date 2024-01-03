using ApiWebApp.Models;
using System.Text.Json;
using System.Net;
using System.Threading.Tasks;
using ApiWebApp.DataAccess;
using System;

namespace ApiWebApp.Services
{
    public class HelperService
    {
        // For small JSON payloads like in our case, it's acceptable for the function to be synchronous
        // This is the case for other functions which involve simple null and string checks

        // 
        // SYNC FUNCTIONS
        //

        // Deserialize account from request body 
        public static ApiResult<Account> DeserializeAccount(string requestBody)
        {
            try
            {
                Account account = JsonSerializer.Deserialize<Account>(requestBody);

                return new ApiResult<Account> { Result = account };
            } 
            catch (JsonException)
            {
                return new ApiResult<Account> { ErrorMessage = "Invalid 'account'", StatusCode = HttpStatusCode.BadRequest };
            }
        }

        // Check if account is valid for use
        public static bool IsAccountValid(Account account)
        {
            if (account == null) return false;

            return IsString(account.Username) && IsString(account.Email) && IsString(account.Password);
        }

        private static bool IsString(object value)
        {
            return value is string stringValue && !string.IsNullOrEmpty(stringValue);
        }

        // Check if account is null / not found
        public static bool AllPropertiesAreNull(Account account)
        {
            return account.Id == 0 && account.Username == null && account.Email == null && account.Password == null;
        }

        // 
        // ASYNC FUNCTIONS
        //
        
        // Check if an username is already in use
        public static async Task<bool> IsUsernameAvailableAsync(string username)
        {
            bool isUsernameInUse = await HelperRepository.IsUsernameAvailableRepositoryAsync(username);
            return isUsernameInUse;
        }

        // Check if an email is already in use
        public static async Task<bool> IsEmailAvailableAsync(string email)
        {
            bool isEmailInUse = await HelperRepository.IsEmailAvailableRepositoryAsync(email);
            return isEmailInUse;
        }
    }
}
