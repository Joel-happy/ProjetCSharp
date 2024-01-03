using System;
using System.IO;
namespace ApiWebApp.DataAccess
{
    public class HelperRepository
    {
        // Get database file path
        public static string GetDatabaseFilePath()
        {
            string dir = "db";
            string fileName = "db.db";

            return Path.Combine(GetProjectRoot(), dir, fileName);
        }

        // Get project root path
        public static string GetProjectRoot()
        {
            string executableDirectory = AppDomain.CurrentDomain.BaseDirectory;
            return Path.Combine(executableDirectory, "../../../../");
        }
    }
}