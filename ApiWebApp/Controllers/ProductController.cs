using System.Net;
using System.Threading.Tasks;
using ApiWebApp.Services;
using System;
using ApiWebApp.Models;

namespace ApiWebApp.Controllers
{
    public class ProductController
    {
        private const string ErrorMessage = "An error occured while processing the request";

        // Determine CRUD operation
        public static async Task HandleProductsAsync(HttpListenerContext context)
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
                ApiResult<string> apiResult = await ProductService.GetProductsAsync();
                await HelperController.HandleApiResult(response, apiResult);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in HandleReadOperationAsync() : {ex.Message}");
                await HelperController.SendResponseAsync(response, HttpStatusCode.InternalServerError, ErrorMessage);
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

            string productId = request.QueryString["id"];

            try
            {
                ApiResult<string> apiResult = await ProductService.GetProductByIdAsync(productId);
                await HelperController.HandleApiResult(response, apiResult);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in HandleReadOperationByIdAsync() : {ex.Message}");
                await HelperController.SendResponseAsync(response, HttpStatusCode.InternalServerError, ErrorMessage);
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
            
            string productRequestBody = await HelperController.ReadRequestBodyAsync(request);

            try
            {
                ApiResult<string> apiResult = await ProductService.CreateProductAsync(productRequestBody);
                await HelperController.HandleApiResult(response, apiResult);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in HandleCreateOperationAsync() : {ex.Message}");
                await HelperController.SendResponseAsync(response, HttpStatusCode.InternalServerError, ErrorMessage);
            }
            finally
            {
                response.Close();
            }
        }

        // Handle UPDATE operation
        private static async Task HandleUpdateOperationAsync(HttpListenerContext context)
        {
            HttpListenerRequest request = context.Request;
            HttpListenerResponse response = context.Response;

            string route = request.Url.AbsolutePath;
            string productIdToUpdate = HelperController.GetIdAccountFromUrl(route);
            string productRequestBody = await HelperController.ReadRequestBodyAsync(request);
            
            try
            {
                ApiResult<string> apiResult = await ProductService.UpdateProductAsync(productIdToUpdate, productRequestBody);
                await HelperController.HandleApiResult(response, apiResult);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in HandleUpdateOperationAsync() : {ex.Message}");
                await HelperController.SendResponseAsync(response, HttpStatusCode.InternalServerError, ErrorMessage);
            }
            finally
            {
                response.Close();
            }
        }

        // Handle DELETE operation
        private static async Task HandleDeleteOperationAsync(HttpListenerContext context)
        {
            HttpListenerRequest request = context.Request;
            HttpListenerResponse response = context.Response;

            string route = request.Url.AbsolutePath;
            string productIdToDelete = HelperController.GetIdAccountFromUrl(route);

            try
            {
                ApiResult<string> apiResult = await ProductService.DeleteProductAsync(productIdToDelete);
                await HelperController.HandleApiResult(response, apiResult);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in HandleDeleteOperationAsync() : {ex.Message}");
                await HelperController.SendResponseAsync(response, HttpStatusCode.InternalServerError, ErrorMessage);
            }
            finally
            {
                response.Close();
            }
        }
    }
}
