using System;
using System.Configuration;

namespace WindowsProgramDesign
{
    public static class DatabaseConfig
    {
        private static string _connectionString = "Server=HP-VICTUS;Database=GymManagementSystem;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true";

        public static string ConnectionString
        {
            get { return _connectionString; }
        }

        public static void SetConnectionString(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentException("Connection string cannot be null or empty.", nameof(connectionString));
            }
            _connectionString = connectionString;
        }
    }
}
