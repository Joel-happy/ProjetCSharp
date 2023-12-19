using ApiWebApp.Models;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Net;
using System.Net.Mime;
using System.Text.Json;
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
                case "/users":
                    //
                    // (TEMP) ---> 'CONTROLLERS/'
                    //

                    // Get response body
                    string result = GetUsers();
                    string responseBody = $"{result}";
                    
                    // Get length of response
                    byte[] responseBytes = System.Text.Encoding.UTF8.GetBytes(responseBody);
                    response.ContentLength64 = responseBytes.Length;

                    // Set content type
                    response.ContentType = MediaTypeNames.Application.Json;

                    // Set status code
                    response.StatusCode = (int)HttpStatusCode.OK;

                    // Get the output where data will be written
                    Stream output = response.OutputStream;

                    // Write HTTP response
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

            List<Account> accounts = new List<Account>();


            using (SQLiteConnection connection = new SQLiteConnection($"Data Source ={databasePath}; Version = 3;"))
            {
                connection.Open();

                using (SQLiteCommand command = new SQLiteCommand("SELECT * FROM account", connection))
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    // Iterate through the results of the query
                    while (reader.Read())
                    {
                        // Extract values from the database record
                        string username = reader.GetString(1);
                        string email = reader.GetString(2);
                        string password = reader.GetString(3);

                        // Create new 'account'
                        Account account = new Account
                        {
                            Username = username,
                            Email = email,
                            Password = password
                        };
                        accounts.Add(account);
                    }
                    
                    connection.Close();

                    // Encapsulate list of accounts to provide a structured JSON response with a 'accounts' wrapper
                    AccountList accountList = new AccountList
                    {
                        accounts = accounts
                    };

                    // Serialize list of accounts to JSON
                    string jsonResult = JsonSerializer.Serialize(accountList, new JsonSerializerOptions
                    {
                        // Format to improve readability
                        WriteIndented = true
                    });

                    return jsonResult;
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
