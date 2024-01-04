using ApiWebApp.Models;
using System.Text.Json;
using System.Net;
using System.Threading.Tasks;
using System.Security.Cryptography;
using ApiWebApp.DataAccess;
using System.Text;
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

        // Deserialize product from request body 
        public static ApiResult<Product> DeserializeProduct(string requestBody)
        {
            try
            {
                Product product = JsonSerializer.Deserialize<Product>(requestBody);

                return new ApiResult<Product> { Result = product };
            }
            catch (JsonException)
            {
                return new ApiResult<Product> { ErrorMessage = "Invalid 'product'", StatusCode = HttpStatusCode.BadRequest };
            }
        }

        // Check if account is valid for use
        public static bool IsAccountValid(Account account)
        {
            if (account == null) return false;

            return IsString(account.Username) && IsString(account.Email) && IsString(account.Password);
        }

        // Check if product is valid for use
        public static bool IsProductValid(Product product)
        {
            if (product == null) return false;

            return IsString(product.Name) && IsString(product.Description) && IsDecimal(product.Price);
        }
        
        private static bool IsString(object value)
        {
            return value is string stringValue && !string.IsNullOrEmpty(stringValue);
        }

        private static bool IsDecimal(object value)
        {
            return value is decimal decimalValue && decimalValue != 0.0M;
        }

        // Check if account is null / not found
        public static bool IsAccountNull(Account account)
        {
            return account.Id == 0 && 
                account.Username == null && 
                account.Email == null && 
                account.Password == null;
        }

        // Check if product is null / not found
        public static bool IsProductNull(Product product)
        {
            return product.Id == 0 && 
                product.Name == null && 
                product.Description == null && 
                product.Price == 0.0M ;
        }

        public static string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                // Hash password from password bytes
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));

                // Convert hashed bytes to a string representation
                string hashedPassword = BitConverter.ToString(hashedBytes);

                return hashedPassword;
            }
        }

        // 
        // ASYNC FUNCTIONS
        //

        // Check if an account's username is already in use
        public static async Task<bool> IsAccountUsernameAvailableAsync(string username)
        {
            bool isUsernameInUse = await HelperRepository.IsAccountUsernameAvailableRepositoryAsync(username);
            return isUsernameInUse;
        }

        // Check if an account's email is already in use
        public static async Task<bool> IsAccountEmailAvailableAsync(string email)
        {
            bool isEmailInUse = await HelperRepository.IsAccountEmailAvailableRepositoryAsync(email);
            return isEmailInUse;
        }


        // Check if an product's name is already in use
        public static async Task<bool> IsProductNameAvailableAsync(string name)
        {
            bool isNameInUse = await HelperRepository.IsProductNameAvailableRepositoryAsync(name);
            return isNameInUse;
        }
    }
}
