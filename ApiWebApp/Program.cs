using ApiWebApp.Controllers;
using System;
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
                HttpListenerContext context = listener.GetContext();

                // _ is a convention to show that the result is intentionally being ignored
                _ = RouteAndHandleRequestAsync(context);
            }
        }
        
        static async Task RouteAndHandleRequestAsync(HttpListenerContext context)
        {
            HttpListenerRequest request = context.Request;
            HttpListenerResponse response = context.Response;
            
            string route = request.RawUrl;
            
            switch (route)
            {
                case "/accounts":
                    await AccountController.HandleAccountsAsync(context);
                    break;
                case "/products":
                    // await ProductController.HandleProductsAsync(context);
                    break;
                default:
                    // 404 error if route doesn't exist
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    break;
            }
   
            response.Close();
        }
    }
}
