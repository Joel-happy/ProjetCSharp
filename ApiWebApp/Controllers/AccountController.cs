using System.Net;
using System.Threading.Tasks;
using ApiWebApp.Services;
using System;
using ApiWebApp.Models;

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
                    if (request.QueryString["id"] != null) {
                        await HandleReadOperationByIdAsync(context);
                    } else {
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
                await HelperController.HandleApiResult(response, apiResult);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in HandleReadOperationAsync() : {ex.Message}");
                await HelperController.SendResponseAsync(response, HttpStatusCode.InternalServerError, "An error occured while processing the request");
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
                await HelperController.HandleApiResult(response, apiResult);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in HandleReadOperationByIdAsync() : {ex.Message}");
                await HelperController.SendResponseAsync(response, HttpStatusCode.InternalServerError, "An error occured while processing the request");
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
            
            string accountRequestBody = await HelperController.ReadRequestBodyAsync(request);

            try
            {
                ApiResult<string> apiResult = await AccountService.CreateAccountAsync(accountRequestBody);
                await HelperController.HandleApiResult(response, apiResult);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in HandleCreateOperationAsync() : {ex.Message}");
                await HelperController.SendResponseAsync(response, HttpStatusCode.InternalServerError, "An error occured while processing the request");
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
    }
}
