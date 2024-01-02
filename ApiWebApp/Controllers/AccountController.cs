using System.Net;
using System.Threading.Tasks;
using System.Net.Mime;
using System.IO;
using ApiWebApp.Services;
using System;
using ApiWebApp.Models;
using System.Text.Json;

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
                    if (request.QueryString["id"] != null)
                    {
                        await HandleReadOperationByIdAsync(context);
                    } 
                    else
                    {
                        await HandleReadOperationAsync(context);
                    }
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
                ApiResult<string> apiResult = await AccountService.GetAccountsAsync();
                
                if (apiResult.IsSuccess)
                {
                    SendResponse(response, apiResult.StatusCode, apiResult.Result);
                }
                else
                {
                    // Error coming from previous layer
                    SendResponse(response, apiResult.StatusCode, apiResult.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                // Error coming from current layer
                Console.WriteLine($"Error in HandleReadOperationAsync() : {ex.Message}");
                SendResponse(response, HttpStatusCode.InternalServerError, "An error occured while processing the request");
            }
            finally
            {
                response.Close();
            }
        }

        // Handle READ by Id operation
        private static async Task HandleReadOperationByIdAsync(HttpListenerContext context)
        {
            HttpListenerRequest request = context.Request;
            HttpListenerResponse response = context.Response;

            string accountId = request.QueryString["id"];

            try
            {
                ApiResult<string> apiResult = await AccountService.GetAccountByIdAsync(accountId);

                if (apiResult.IsSuccess)
                {
                    SendResponse(response, apiResult.StatusCode, apiResult.Result);
                }
                else
                {
                    SendResponse(response, apiResult.StatusCode, apiResult.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in HandleReadOperationByIdAsync() : {ex.Message}");
                SendResponse(response, HttpStatusCode.InternalServerError, "An error occured while processing the request");
            }
            finally
            {
                response.Close();
            }
        }

        // Handle CREATE operation
        private static async Task HandleCreateOperationAsync(HttpListenerContext context)
        {
            HttpListenerRequest request = context.Request;
            HttpListenerResponse response = context.Response;

            Account newAccount;

            try
            {
                newAccount = await ReadRequestBodyAccount(request);

            }
            catch (JsonException ex)
            {
                SendResponse(response, HttpStatusCode.BadRequest, "Invalid 'account'");
                return;
            }

            try
            {
                ApiResult<string> apiResult = await AccountService.CreateAccountAsync(newAccount);

                if (apiResult.IsSuccess)
                {
                    SendResponse(response, apiResult.StatusCode, apiResult.Result);
                }
                else
                {
                    SendResponse(response, apiResult.StatusCode, apiResult.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in HandleCreateOperationAsync() : {ex.Message}");
                SendResponse(response, HttpStatusCode.InternalServerError, "An error occured while processing the request");
            }
            finally
            {
                response.Close();
            }
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
        
        //
        // Helper Functions
        //

        // Send a HTTP response to the user
        private static async void SendResponse(HttpListenerResponse response, HttpStatusCode statusCode, string result)
        {
            // Set content type
            response.ContentType = MediaTypeNames.Application.Json;

            // Set status code
            response.StatusCode = (int)statusCode;

            using (Stream output = response.OutputStream)
            {
                byte[] responseBytes = System.Text.Encoding.UTF8.GetBytes(result);
                response.ContentLength64 = responseBytes.Length;
                await output.WriteAsync(responseBytes, 0, responseBytes.Length);
            }
        }

        // Extract account from request body
        private static async Task<Account> ReadRequestBodyAccount(HttpListenerRequest request)
        {
            string requestBody;
            using (var reader = new StreamReader(request.InputStream, request.ContentEncoding)) 
            {
                requestBody = await reader.ReadToEndAsync();
            }

            Account account = JsonSerializer.Deserialize<Account>(requestBody);

            return account;
        }
    }
}
