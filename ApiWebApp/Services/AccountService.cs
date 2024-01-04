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
        private const string ErrorMessage = "An error occured while processing the request";
        private const string InvalidIdErrorMessage = "Invalid 'id'";

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

                    return new ApiResult<string> { 
                        Result = jsonResult, 
                        StatusCode = apiResult.StatusCode 
                    };
                } 
                else
                {
                    return new ApiResult<string> { 
                        ErrorMessage = apiResult.ErrorMessage, 
                        StatusCode = apiResult.StatusCode 
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAccountsAsync() : {ex.Message}");
                return new ApiResult<string>
                {
                    ErrorMessage = ErrorMessage,
                    StatusCode = HttpStatusCode.InternalServerError
                };
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
                    return new ApiResult<string> { 
                        ErrorMessage = InvalidIdErrorMessage, 
                        StatusCode = HttpStatusCode.BadRequest 
                    };
                }
                
                // Convert parameter to int
                int intAccountId = int.Parse(accountId);

                ApiResult<Account> apiResult = await AccountRepository.GetAccountByIdRepositoryAsync(intAccountId);
                
                if (apiResult.IsSuccess)
                {
                    if (!HelperService.AllPropertiesAreNull(apiResult.Result))
                    {
                        string jsonResult = JsonSerializer.Serialize(apiResult.Result, new JsonSerializerOptions
                        {
                            WriteIndented = true
                        });
                        
                        return new ApiResult<string> { 
                            Result = jsonResult, 
                            StatusCode = HttpStatusCode.OK 
                        };
                    }
                    else
                    {
                        // Account not found
                        return new ApiResult<string> { 
                            ErrorMessage = "Account not found", 
                            StatusCode = HttpStatusCode.NotFound 
                        };
                    }
                }
                else
                {
                    return new ApiResult<string> { 
                        ErrorMessage = apiResult.ErrorMessage, 
                        StatusCode = apiResult.StatusCode 
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAccountByIdAsync() : {ex.Message}");
                return new ApiResult<string>
                {
                    ErrorMessage = ErrorMessage,
                    StatusCode = HttpStatusCode.InternalServerError
                };
            }
        }
        
        // Service CREATE
        public static async Task<ApiResult<string>> CreateAccountAsync(string accountRequestBody)
        {
            try
            {
                ApiResult<Account> deserializationResult = HelperService.DeserializeAccount(accountRequestBody);

                if (deserializationResult.IsSuccess)
                {
                    Account account = deserializationResult.Result;
                    
                    if (HelperService.IsAccountValid(account) &&
                        await HelperService.IsUsernameAvailableAsync(account.Username) && 
                        await HelperService.IsEmailAvailableAsync(account.Email))
                    {
                        account.Password = HelperService.HashPassword(account.Password);

                        ApiResult<string> apiResult = await AccountRepository.CreateAccountRepositoryAsync(account);

                        if (apiResult.IsSuccess)
                        {
                            return new ApiResult<string> { 
                                Result = apiResult.Result, 
                                StatusCode = apiResult.StatusCode
                            };
                        }
                        else
                        {
                            return new ApiResult<string> { 
                                ErrorMessage = apiResult.ErrorMessage, 
                                StatusCode = apiResult.StatusCode 
                            };
                        }
                    }
                    else
                    {
                        return new ApiResult<string> { 
                            ErrorMessage = "Invalid 'account' OR username / email is already in use", 
                            StatusCode = HttpStatusCode.BadRequest 
                        };
                    }
                }
                else
                {
                    return new ApiResult<string> {
                        ErrorMessage = deserializationResult.ErrorMessage, 
                        StatusCode = deserializationResult.StatusCode 
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CreateAccountAsync() : {ex.Message}");
                return new ApiResult<string> { 
                    ErrorMessage = ErrorMessage, 
                    StatusCode = HttpStatusCode.InternalServerError 
                };
            }
        }

        // Service UPDATE
        public static async Task<ApiResult<string>> UpdateAccountAsync(string accountIdToUpdate, string accountRequestBody)
        {
            try
            {
                if (string.IsNullOrEmpty(accountIdToUpdate) || !int.TryParse(accountIdToUpdate, out _))
                {
                    return new ApiResult<string>
                    {
                        ErrorMessage = InvalidIdErrorMessage,
                        StatusCode = HttpStatusCode.BadRequest
                    };
                }

                ApiResult<Account> deserializationResult = HelperService.DeserializeAccount(accountRequestBody);

                if (deserializationResult.IsSuccess)
                {
                    Account account = deserializationResult.Result;

                    if (HelperService.IsAccountValid(account))
                    {
                        account.Password = HelperService.HashPassword(account.Password);
                        int intAccountIdToUpdate = int.Parse(accountIdToUpdate);

                        ApiResult<string> apiResult = await AccountRepository.UpdateAccountRepositoryAsync(intAccountIdToUpdate, account);

                        if (apiResult.IsSuccess)
                        {
                            return new ApiResult<string>
                            {
                                Result = apiResult.Result,
                                StatusCode = apiResult.StatusCode
                            };
                        }
                        else
                        {
                            return new ApiResult<string>
                            {
                                ErrorMessage = apiResult.ErrorMessage,
                                StatusCode = apiResult.StatusCode
                            };
                        }
                    }
                    else
                    {
                        return new ApiResult<string>
                        {
                            ErrorMessage = "Invalid 'account'",
                            StatusCode = HttpStatusCode.BadRequest
                        };
                    }
                }
                else
                {
                    return new ApiResult<string>
                    {
                        ErrorMessage = deserializationResult.ErrorMessage,
                        StatusCode = deserializationResult.StatusCode
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in UpdateAccountAsync() : {ex.Message}");
                return new ApiResult<string>
                {
                    ErrorMessage = ErrorMessage,
                    StatusCode = HttpStatusCode.InternalServerError
                };
            }
        }
        
        // Service DELETE
        public static async Task<ApiResult<string>> DeleteAccountAsync(string accountIdToDelete)
        {
            try
            {
                // Check if the query parameter is empty or not an int
                if (string.IsNullOrEmpty(accountIdToDelete) || !int.TryParse(accountIdToDelete, out _))
                {
                    return new ApiResult<string> { 
                        ErrorMessage = InvalidIdErrorMessage, 
                        StatusCode = HttpStatusCode.BadRequest
                    };
                }

                // Convert parameter to int
                int intAccountIdToDelete = int.Parse(accountIdToDelete);

                ApiResult<string> apiResult = await AccountRepository.DeleteAccountRepositoryAsync(intAccountIdToDelete);

                if (apiResult.IsSuccess)
                {
                    return new ApiResult<string>
                    {
                        Result = apiResult.Result,
                        StatusCode = apiResult.StatusCode
                    };
                }
                else
                {
                    return new ApiResult<string>
                    {
                        ErrorMessage = apiResult.ErrorMessage,
                        StatusCode = apiResult.StatusCode
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in DeleteAccountAsync() : {ex.Message}");
                return new ApiResult<string>
                {
                    ErrorMessage = ErrorMessage,
                    StatusCode = HttpStatusCode.InternalServerError
                };
            }
        }
    }
}