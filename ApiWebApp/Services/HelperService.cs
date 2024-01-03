using ApiWebApp.Models;
using System.Text.Json;
using System.Net;

namespace ApiWebApp.Services
{
    public class HelperService
    {
        // For small JSON payloads like in our case, it's acceptable for the function to be synchronous
        // This is the case for other functions which involve simple null and string checks

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

        // Check if account is null / not found
        public static bool AllPropertiesAreNull(Account account)
        {
            return account.Id == 0 && account.Username == null && account.Email == null && account.Password == null;
        }

        // Check if account is valid for use
        public static bool IsAccountValid(Account account)
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
