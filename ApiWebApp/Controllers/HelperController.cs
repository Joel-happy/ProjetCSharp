using System.Net;
using System.Threading.Tasks;
using System.Net.Mime;
using System.IO;
using ApiWebApp.Models;
using System;

namespace ApiWebApp.Controllers
{
    public class HelperController
    {
        //
        // SYNC
        // 

        public static string GetIdAccountFromUrl(string route)
        {
            // URL pattern is "/accounts/{id}
            // As to have a successfull Uri object, the "http://localhost" has to be added
            var segments = new Uri("http://localhost" + route).Segments;

            // segments[0] = /              (1st segment)
            // segments[1] = accounts/      (2nd segment)
            // segments[2] = 123/ (or 123)  (3rd segment)

            // Check if there are enough segments in the URL path
            if (segments.Length >= 3)
            {
               return segments[2].TrimEnd('/');
            }
            
            return null;
        }

        //
        // ASYNC
        // 

        // Send appropriate response based on the service response
        public static async Task HandleApiResult(HttpListenerResponse response, ApiResult<string> apiResult)
        {
            if (apiResult.IsSuccess) {
                await SendResponseAsync(response, apiResult.StatusCode, apiResult.Result);
            } else {
                await SendResponseAsync(response, apiResult.StatusCode, apiResult.ErrorMessage);
            }
        }
        
        // Send a HTTP response to the user
        public static async Task SendResponseAsync(HttpListenerResponse response, HttpStatusCode statusCode, string result)
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
        public static async Task<string> ReadRequestBodyAsync(HttpListenerRequest request)
        {
            string requestBody;
            using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
            {
                requestBody = await reader.ReadToEndAsync();
            }
            return requestBody;
        }
    }
}