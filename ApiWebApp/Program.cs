using System;
using System.Data.SQLite;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ApiWebApp
{
    class Program
    {
        static Task Main()
        {
            // Create a Http Listener that listens on the loopback address (localhost), on the port 8080
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
            // Extract the request and response objects
            HttpListenerRequest request = context.Request;
            HttpListenerResponse response = context.Response;

            string route = request.RawUrl;
            
            switch (route)
            {
                case "/users":
                    //
                    // PUT THIS INTO 'CONTROLLERS/'
                    //

                    string result = GetUsers();
                    string responseBody = $"<html><body>{result}</body></html>";

                    // Write the HTTP response to the client
                    byte[] responseBytes = System.Text.Encoding.UTF8.GetBytes(responseBody);
                    response.ContentLength64 = responseBytes.Length;
                    Stream output = response.OutputStream;

                    await output.WriteAsync(responseBytes, 0, responseBytes.Length);

                    break;
                default:
                    // 404 error if route doesn't exist
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    break;
            }

            
            response.Close();
        }

        static string GetUsers()
        {
            // Construct full path to the SQLite database file
            string databasePath = Path.Combine(GetProjectRoot(), "db", "db.sqlite");

            using (SQLiteConnection connection = new SQLiteConnection($"Data Source ={databasePath}; Version = 3;"))
            {
                connection.Open();

                using (SQLiteCommand command = new SQLiteCommand("SELECT * FROM account", connection))
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    // StringBuilder to store the result from the database
                    StringBuilder result = new StringBuilder();

                    // Iterate through the results of the query
                    while (reader.Read())
                    {
                        // Extract values from the database record
                        string username = reader.GetString(1);
                        string email = reader.GetString(2);
                        string password = reader.GetString(3);

                        // Append the formatted result to the StringBuilder
                        result.AppendLine($"Username : {username}, Email : {email}, Password : {password} \n");
                    }

                    connection.Close();

                    return result.ToString();
                }
            }
        }

        // Get project root path
        static string GetProjectRoot()
        {
            string executableDirectory = AppDomain.CurrentDomain.BaseDirectory;
            return Path.Combine(executableDirectory, "../../../../");
        }
    }
}
