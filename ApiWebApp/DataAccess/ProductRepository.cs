using ApiWebApp.Models;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Net;
using System.Threading.Tasks;

namespace ApiWebApp.DataAccess
{
    public class ProductRepository
    {
        private const string DatabaseErrorMessage = "A database error occured";
        private const string ErrorMessage = "An error occured while processing the request";

        // READ Products
        public static async Task<ApiResult<List<Product>>> GetProductsRepositoryAsync()
        {
            try
            {
                List<Product> products = new List<Product>();
                
                using (SQLiteConnection connection = new SQLiteConnection($"Data Source ={HelperRepository.GetDatabaseFilePath()}; Version = 3;"))
                {
                    await connection.OpenAsync();

                    using (SQLiteCommand command = new SQLiteCommand("SELECT * FROM product", connection))
                    {
                        using (SQLiteDataReader reader = (SQLiteDataReader)await command.ExecuteReaderAsync())
                        {
                            // Iterate through the results of the query
                            while (await reader.ReadAsync())
                            {
                                // Extract values from the database record
                                int id = reader.GetInt32(0);
                                string name = reader.GetString(1);
                                string description = reader.GetString(2);
                                decimal price = reader.GetDecimal(3);

                                // Create new 'product'
                                Product product = new Product
                                {
                                    Id = id,
                                    Name = name,
                                    Description = description,
                                    Price = price
                                };
                                products.Add(product);
                            }
                            connection.Close();
                        }
                    }
                }
                return new ApiResult<List<Product>> { 
                    Result = products,
                    StatusCode = HttpStatusCode.OK 
                };
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine($"SQLite error in GetProductsRepositoryAsync() : {ex.Message}");
                return new ApiResult<List<Product>> { 
                    ErrorMessage = DatabaseErrorMessage, 
                    StatusCode = HttpStatusCode.InternalServerError 
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetProductsRepositoryAsync() : {ex.Message}");
                return new ApiResult<List<Product>> { 
                    ErrorMessage = ErrorMessage, 
                    StatusCode = HttpStatusCode.InternalServerError 
                };
            }
        }

        // READ Product by ID
        public static async Task<ApiResult<Product>> GetProductByIdRepositoryAsync(int productId)
        {
            try
            {
                Product product = new Product();

                using (SQLiteConnection connection = new SQLiteConnection($"Data Source ={HelperRepository.GetDatabaseFilePath()}; Version = 3;"))
                {
                    await connection.OpenAsync();

                    // Use parameterized query to prevent SQL injection
                    using (SQLiteCommand command = new SQLiteCommand($"SELECT * FROM product WHERE product_id = @productId", connection))
                    {
                        command.Parameters.AddWithValue("@productId", productId);

                        using (SQLiteDataReader reader = (SQLiteDataReader)await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                int id = reader.GetInt32(0);
                                string name = reader.GetString(1);
                                string description = reader.GetString(2);
                                decimal price = reader.GetDecimal(3);
                                
                                product = new Product
                                {
                                    Id = id,
                                    Name = name,
                                    Description = description,
                                    Price = price
                                };
                            }
                            connection.Close();
                        }
                    }
                }

                return new ApiResult<Product> { 
                    Result = product, 
                    StatusCode = HttpStatusCode.OK 
                };
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine($"SQLite error in GetProductByIdRepositoryAsync() : {ex.Message}");
                return new ApiResult<Product> { 
                    ErrorMessage = DatabaseErrorMessage, 
                    StatusCode = HttpStatusCode.InternalServerError 
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetProductByIdRepositoryAsync() : {ex.Message}");
                return new ApiResult<Product> { 
                    ErrorMessage = ErrorMessage, 
                    StatusCode = HttpStatusCode.InternalServerError 
                };
            }
        }

        // CREATE Product
        public static async Task<ApiResult<string>> CreateProductRepositoryAsync(Product product)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection($"Data Source ={HelperRepository.GetDatabaseFilePath()}; Version = 3;"))
                {
                    await connection.OpenAsync();
                    
                    using (SQLiteCommand command = new SQLiteCommand("INSERT INTO product(name,description,price) VALUES(@name, @description, @price)", connection))
                    {
                        // Add parameters to command
                        command.Parameters.AddWithValue("@name", product.Name);
                        command.Parameters.AddWithValue("@description", product.Description);
                        command.Parameters.AddWithValue("@price", product.Price);

                        // Execute the command
                        await command.ExecuteNonQueryAsync();
                    }
                }
                
                return new ApiResult<string> { 
                    Result = "Product created successfully", 
                    StatusCode = HttpStatusCode.Created 
                };
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine($"SQLite error in CreateProductRepositoryAsync() : {ex.Message}");
                return new ApiResult<string>
                {
                    ErrorMessage = DatabaseErrorMessage,
                    StatusCode = HttpStatusCode.InternalServerError
                };

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CreateProductRepositoryAsync() : {ex.Message}");
                return new ApiResult<string> 
                { 
                    ErrorMessage = ErrorMessage, 
                    StatusCode = HttpStatusCode.InternalServerError
                };
            }
        }

        // UPDATE Product
        public static async Task<ApiResult<string>> UpdateProductRepositoryAsync(int productIdToUpdate, Product product)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection($"Data Source ={HelperRepository.GetDatabaseFilePath()}; Version = 3;"))
                {
                    await connection.OpenAsync();
                    
                    using (SQLiteCommand command = new SQLiteCommand("UPDATE product SET name = @name, description = @description, price = @price WHERE product_id = @productId", connection))
                    {
                        command.Parameters.AddWithValue("@name", product.Name);
                        command.Parameters.AddWithValue("@description", product.Description);
                        command.Parameters.AddWithValue("@price", product.Price);
                        command.Parameters.AddWithValue("@productId", productIdToUpdate);
                        
                        int affectedRows = await command.ExecuteNonQueryAsync();
                        
                        if (affectedRows > 0)
                        {
                            return new ApiResult<string>
                            {
                                Result = "Product updated successfully",
                                StatusCode = HttpStatusCode.OK
                            };
                        }
                        else
                        {
                            return new ApiResult<string>
                            {
                                ErrorMessage = "Product not found",
                                StatusCode = HttpStatusCode.NotFound
                            };
                        }
                    }
                }
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine($"SQLite error in UpdateProductRepositoryAsync() : {ex.Message}");
                return new ApiResult<string>
                {
                    ErrorMessage = DatabaseErrorMessage,
                    StatusCode = HttpStatusCode.InternalServerError
                };
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in UpdateProductRepositoryAsync() : {ex.Message}");
                return new ApiResult<string>
                {
                    ErrorMessage = ErrorMessage,
                    StatusCode = HttpStatusCode.InternalServerError
                };
            }
        }

        // DELETE Product
        public static async Task<ApiResult<string>> DeleteProductRepositoryAsync(int productIdToDelete)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection($"Data Source ={HelperRepository.GetDatabaseFilePath()}; Version = 3;"))
                {
                    await connection.OpenAsync();
                    
                    using (SQLiteCommand command = new SQLiteCommand("DELETE FROM product WHERE product_id = @productId", connection))
                    {
                        command.Parameters.AddWithValue("@productId", productIdToDelete);

                        // Execute the command and get the number of affected rows
                        int affectedRows = await command.ExecuteNonQueryAsync();
                        
                        if (affectedRows > 0)
                        {
                            return new ApiResult<string>
                            {
                                Result = "Product deleted successfully",
                                StatusCode = HttpStatusCode.NoContent
                            };
                        }
                        else
                        {
                            // No rows were deleted, indicating that the account didn't exist
                            return new ApiResult<string> { 
                                ErrorMessage = "Product not found", 
                                StatusCode = HttpStatusCode.NotFound 
                            };
                        }
                    }
                }
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine($"SQLite error in DeleteProductRepositoryAsync() : {ex.Message}");
                return new ApiResult<string>
                {
                    ErrorMessage = DatabaseErrorMessage,
                    StatusCode = HttpStatusCode.InternalServerError
                };

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in DeleteProductRepositoryAsync() : {ex.Message}");
                return new ApiResult<string>
                {
                    ErrorMessage = ErrorMessage,
                    StatusCode = HttpStatusCode.InternalServerError
                };
            }
        }
    }
}