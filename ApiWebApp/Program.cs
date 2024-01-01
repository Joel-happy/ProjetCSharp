using ApiWebApp.Controllers;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace ApiWebApp
{
    class Program
    {
        static Task Main()
        {
            // Create a HttpListener that listens on the loopback address (localhost), on the port 8080
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add("http://localhost:8080/");
            listener.Start();

            Console.WriteLine("Listening for requests...");

            while (true)
            {
                try
                {
                    HttpListenerContext context = listener.GetContext();

                    // _ is a convention to show that the result is intentionally being ignored
                    _ = HandleRouteAsync(context);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occured while receiving the request : {ex.Message}");
                }
            }
        }
        
        static async Task HandleRouteAsync(HttpListenerContext context)
        {
            HttpListenerResponse response = context.Response;

            string route = context.Request.Url.AbsolutePath;

            switch (route)
            {
                case "/accounts":
                    await AccountController.HandleAccountsAsync(context);
                    break;
                case "/products":
                    // await ProductController.HandleProductsAsync(context);
                    break;
                default:
                    HandleNotFound(response);
                    break;
            }
            response.Close();
        }

        private static async void HandleNotFound(HttpListenerResponse response)
        {
            // 404 error if route doesn't exist
            response.StatusCode = (int)HttpStatusCode.NotFound;

            // Get the output where data will be written
            using (Stream output = response.OutputStream)
            {
                // Provide an error message in the response body
                string notFoundResponse = "404 Not Found";
                byte[] notFoundResponseBytes = System.Text.Encoding.UTF8.GetBytes(notFoundResponse);
                response.ContentLength64 = notFoundResponseBytes.Length;

                // Write error response
                await output.WriteAsync(notFoundResponseBytes, 0, notFoundResponseBytes.Length);
            }
        }
    }
}
