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
                _ = ProcessRequestAsync(client);
            }
        }

        static async Task ProcessRequestAsync(TcpClient client)
        {
            // Open a network stream for reading and writing data to the client
            using (NetworkStream stream = client.GetStream())

            // Create a StreamReader for reading data from the network stream
            using (StreamReader reader = new StreamReader(stream))

            // Create a StreamWriter for writing data to the network stream
            using (StreamWriter writer = new StreamWriter(stream))
            {
                // Read the first line of the HTTP request
                string request = await reader.ReadLineAsync();
               
                Console.WriteLine($"Received request: {request}");
                
                string result = ReadDataFromDatabase();
                string response = "HTTP/1.1 200 OK\r\nContent-Type: text/html\r\n\r\n<html><body>" + result + "</body></html>";

                // Write the HTTP response to the client
                await writer.WriteAsync(response);
            }
            
            client.Close();
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
