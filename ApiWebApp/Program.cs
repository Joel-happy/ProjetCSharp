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
            // Create a TCP listener that listens on the loopback address and on the port 8080
            TcpListener listener = new TcpListener(System.Net.IPAddress.Parse("127.0.0.1"), 8080);
            listener.Start();

            Console.WriteLine("Listening for requests...");

            // Continuously accept incoming TCP clients
            while (true)
            {
                // Asynchronously accept a TCP client.
                TcpClient client = await listener.AcceptTcpClientAsync();

                // Start processing the request in a separate asynchronous method
                _ = ProcessRequestAsync(client);
            }
        }

        static async Task ProcessRequestAsync(TcpClient client)
        {
            // Open a network stream for reading and writing data to the client.
            using (NetworkStream stream = client.GetStream())

            // Create a StreamReader for reading data from the network stream.
            using (StreamReader reader = new StreamReader(stream))

            // Create a StreamWriter for writing data to the network stream.
            using (StreamWriter writer = new StreamWriter(stream))
            {
                // Read the first line of the HTTP request
                string request = await reader.ReadLineAsync();

                // Display the received request in the console
                Console.WriteLine($"Received request: {request}");

                // Simple HTML response to the client (for testing purposes)
                string response = "HTTP/1.1 200 OK\r\nContent-Type: text/html\r\n\r\n<html><body><h1>Hello, World!</h1></body></html>";

                // Write the HTTP response to the client
                await writer.WriteAsync(response);
            }

            client.Close();
        }
    }
}
