using System;
using System.Data.SQLite;
using System.IO;

namespace ApiWebApp
{
    class Program
    {
        static void Main(string[] args)
        {
            string connectionString = "Data Source=../../../../db/db.sqlite;Version=3;";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    Console.WriteLine("Connection to the database is open");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
        }
    }
}