using System;
using System.Data.SQLite;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ApiWebApp
{
    class Program
    {
        static async Task Main()
        {
            // Create a TCP listener that listens on the loopback address (localhost) and on the port 8080
            TcpListener listener = new TcpListener(System.Net.IPAddress.Parse("127.0.0.1"), 8080);
            listener.Start();
            
            Console.WriteLine("Listening for requests...");

            while (true)
            {
                // Asynchronously accept a TCP client.
                TcpClient client = await listener.AcceptTcpClientAsync();

                // _ is a convention to show that the result is intentionally being ignored
                _ = RouteAndHandleRequestAsync(client);
            }
        }
        
        static async Task RouteAndHandleRequestAsync(TcpClient client)
        {
            using (NetworkStream stream = client.GetStream())
            using (StreamReader reader = new StreamReader(stream))
            using (StreamWriter writer = new StreamWriter(stream))
            {
                // Read and parse the request 
                string request = await reader.ReadLineAsync();
                Console.WriteLine($"Received request: {request}");

                string route = DetermineRoute(request);

                switch (route)
                {
                    case "/users":
                        string result = ReadDataFromDatabase();
                        string response = "HTTP/1.1 200 OK\r\nContent-Type: text/html\r\n\r\n<html><body>" + result + "</body></html>";

                        // Write the HTTP response to the client
                        await writer.WriteAsync(response);
                        
                        break;
                    default:
                        await writer.WriteAsync("HTTP/1.1 404 Not Found\r\n\r\n");
                        break;
                }
            }
            
            client.Close();
        }
        
        static string DetermineRoute(string request)
        {
            // Splits request line into parts using space as a delimiter
            string[] requestParts = request.Split(' ');
            
            // Check if they're enough parts in request to extract the route
            if (requestParts.Length > 1)
            {
                // Return route, typically the second part in a request
                return requestParts[1];
            }

            // Default to root route if not specifed route is identified
            return "/";
        }

        static string ReadDataFromDatabase()
        {
            string databasePath = Path.Combine(GetProjectRoot(), "db", "db.sqlite");

            using (SQLiteConnection connection = new SQLiteConnection($"Data Source ={databasePath}; Version = 3;"))
            {
                connection.Open();
                
                using (SQLiteCommand command = new SQLiteCommand("SELECT * FROM account", connection))
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    StringBuilder result = new StringBuilder();

                    while (reader.Read())
                    {
                        string username = reader.GetString(1);
                        string email = reader.GetString(2);
                        string password = reader.GetString(3);

                        result.AppendLine($"Username : {username}, Email : {email}, Password : {password} \n");
                    }

                    connection.Close();
                    return result.ToString();
                }
            }
        }

        // Returns project root path
        static string GetProjectRoot()
        {
            string executableDirectory = AppDomain.CurrentDomain.BaseDirectory;
            return Path.Combine(executableDirectory, "../../../../");
        }
    }
}
