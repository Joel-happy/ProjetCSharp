using System.Net;
using System.Threading.Tasks;
using System.Net.Mime;
using System.IO;
using ApiWebApp.Services;
using System;

namespace ApiWebApp.Controllers
{
    public class AccountController
    {
        // Determine CRUD operation
        public static async Task HandleAccountsAsync(HttpListenerContext context)
        {
            HttpListenerRequest request = context.Request;
            HttpListenerResponse response = context.Response;

            switch (request.HttpMethod)
            {
                case "GET":
                    await HandleReadOperationAsync(context);
                    break;
                case "POST":
                    await HandleCreateOperationAsync(context);
                    break;
                case "PUT":
                    await HandleUpdateOperationAsync(context);
                    break;
                case "DELETE":
                    await HandleDeleteOperationAsync(context);
                    break;
                default: 
                    break;
            }

            response.Close();
        }

        // Handle READ operation
        private static async Task HandleReadOperationAsync(HttpListenerContext context)
        {
            HttpListenerResponse response = context.Response;

            try
            {
                // Get result
                string result = await AccountService.GetAccountsAsync();

                // Set content type
                response.ContentType = MediaTypeNames.Application.Json;

                // Set status code
                response.StatusCode = (int)HttpStatusCode.OK;

                // Get the output where data will be written
                using (Stream output = response.OutputStream)
                {
                    // Write HTTP response
                    byte[] responseBytes = System.Text.Encoding.UTF8.GetBytes(result);
                    response.ContentLength64 = responseBytes.Length;
                    await output.WriteAsync(responseBytes, 0, responseBytes.Length);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in HandleReadOperationAsync() : {ex.Message}");
                
                // Set status code for internal server error
                response.StatusCode = (int)HttpStatusCode.InternalServerError;

                using (Stream output = response.OutputStream)
                {
                    // Provide an error message in the response body
                    string errorResponse = "An error occurred while processing the request";
                    byte[] errorResponseBytes = System.Text.Encoding.UTF8.GetBytes(errorResponse);
                    response.ContentLength64 = errorResponseBytes.Length;
                    
                    // Write error response
                    await output.WriteAsync(errorResponseBytes, 0, errorResponseBytes.Length);
                }
            }
            finally
            {
                response.Close();
            }
        }

        // Handle CREATE operation
        private static async Task HandleCreateOperationAsync(HttpListenerContext context)
        {
            // TO DO
            await Task.CompletedTask;
        }

        // Handle UPDATE operation
        private static async Task HandleUpdateOperationAsync(HttpListenerContext context)
        {
            // TO DO
            await Task.CompletedTask;
        }

        // Handle DELETE operation
        private static async Task HandleDeleteOperationAsync(HttpListenerContext context)
        {
            // TO DO
            await Task.CompletedTask;
        }
    }
}
