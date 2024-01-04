using ApiWebApp.Models;
using ApiWebApp.DataAccess;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using System;
using System.Net;

namespace ApiWebApp.Services
{
    public class ProductService
    {
        private const string ErrorMessage = "An error occured while processing the request";
        private const string InvalidIdErrorMessage = "Invalid 'id'";

        // Service READ
        public static async Task<ApiResult<string>> GetProductsAsync()
        {
            try
            {
                ApiResult<List<Product>> apiResult = await ProductRepository.GetProductsRepositoryAsync();

                if (apiResult.IsSuccess)
                {
                    // Encapsulate list of accounts to provide a structured JSON response with a 'accounts' wrapper
                    ProductList productList = new ProductList
                    {
                        products = apiResult.Result
                    };
                    
                    // Serialize list of accounts to JSON
                    string jsonResult = JsonSerializer.Serialize(productList, new JsonSerializerOptions
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
                Console.WriteLine($"Error in GetProductsAsync() : {ex.Message}");
                return new ApiResult<string>
                {
                    ErrorMessage = ErrorMessage,
                    StatusCode = HttpStatusCode.InternalServerError
                };
            }
        }

        // Service READ by Id
        public static async Task<ApiResult<string>> GetProductByIdAsync(string productId)
        {
            try
            {
                // Check if the query parameter is empty or not an int
                if (string.IsNullOrEmpty(productId) || !int.TryParse(productId, out _))
                {
                    return new ApiResult<string> { 
                        ErrorMessage = InvalidIdErrorMessage, 
                        StatusCode = HttpStatusCode.BadRequest 
                    };
                }
                
                // Convert parameter to int
                int intProductId = int.Parse(productId);

                ApiResult<Product> apiResult = await ProductRepository.GetProductByIdRepositoryAsync(intProductId);
                
                if (apiResult.IsSuccess)
                {
                    if (!HelperService.IsProductNull(apiResult.Result))
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
                            ErrorMessage = "Product not found", 
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
                Console.WriteLine($"Error in GetProductByIdAsync() : {ex.Message}");
                return new ApiResult<string>
                {
                    ErrorMessage = ErrorMessage,
                    StatusCode = HttpStatusCode.InternalServerError
                };
            }
        }
        
        // Service CREATE
        public static async Task<ApiResult<string>> CreateProductAsync(string productRequestBody)
        {
            try
            {
                ApiResult<Product> deserializationResult = HelperService.DeserializeProduct(productRequestBody);

                if (deserializationResult.IsSuccess)
                {
                    Product product = deserializationResult.Result;
                    
                    if (HelperService.IsProductValid(product) && await HelperService.IsProductNameAvailableAsync(product.Name))
                    {
                        ApiResult<string> apiResult = await ProductRepository.CreateProductRepositoryAsync(product);

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
                            ErrorMessage = "Invalid 'product' OR name is already in use", 
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
                Console.WriteLine($"Error in CreateProductAsync() : {ex.Message}");
                return new ApiResult<string> { 
                    ErrorMessage = ErrorMessage, 
                    StatusCode = HttpStatusCode.InternalServerError 
                };
            }
        }

        // Service UPDATE
        public static async Task<ApiResult<string>> UpdateProductAsync(string productIdToUpdate, string productRequestBody)
        {
            try
            {
                if (string.IsNullOrEmpty(productIdToUpdate) || !int.TryParse(productIdToUpdate, out _))
                {
                    return new ApiResult<string>
                    {
                        ErrorMessage = InvalidIdErrorMessage,
                        StatusCode = HttpStatusCode.BadRequest
                    };
                }

                ApiResult<Product> deserializationResult = HelperService.DeserializeProduct(productRequestBody);

                if (deserializationResult.IsSuccess)
                {
                    Product product = deserializationResult.Result;

                    if (HelperService.IsProductValid(product))
                    {
                        int intProductIdToUpdate = int.Parse(productIdToUpdate);

                        ApiResult<string> apiResult = await ProductRepository.UpdateProductRepositoryAsync(intProductIdToUpdate, product);

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
                            ErrorMessage = "Invalid 'product'",
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
                Console.WriteLine($"Error in UpdateProductAsync() : {ex.Message}");
                return new ApiResult<string>
                {
                    ErrorMessage = ErrorMessage,
                    StatusCode = HttpStatusCode.InternalServerError
                };
            }
        }
        
        // Service DELETE
        public static async Task<ApiResult<string>> DeleteProductAsync(string productIdToDelete)
        {
            try
            {
                // Check if the query parameter is empty or not an int
                if (string.IsNullOrEmpty(productIdToDelete) || !int.TryParse(productIdToDelete, out _))
                {
                    return new ApiResult<string> { 
                        ErrorMessage = InvalidIdErrorMessage, 
                        StatusCode = HttpStatusCode.BadRequest
                    };
                }

                // Convert parameter to int
                int intProductIdToDelete = int.Parse(productIdToDelete);

                ApiResult<string> apiResult = await ProductRepository.DeleteProductRepositoryAsync(intProductIdToDelete);

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
                Console.WriteLine($"Error in DeleteProductAsync() : {ex.Message}");
                return new ApiResult<string>
                {
                    ErrorMessage = ErrorMessage,
                    StatusCode = HttpStatusCode.InternalServerError
                };
            }
        }
    }
}