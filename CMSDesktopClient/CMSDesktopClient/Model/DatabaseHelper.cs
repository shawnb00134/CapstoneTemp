using System;
using System.Threading.Tasks;
using Npgsql;

namespace CMSDesktopClient.Data
{
    public class DatabaseHelper
    {
        private readonly string _connectionString;

        public DatabaseHelper(string host, string database, string username, string password, int port = 5432)
        {
            _connectionString = $"Host={host};Port={port};Username={username};Password={password};Database={database}";
        }

        // Test connection
        public async Task<bool> TestConnectionAsync()
        {
            try
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    await conn.OpenAsync();
                    System.Diagnostics.Debug.WriteLine("✅ Database connection successful!");
                    return true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Connection failed: {ex.Message}");
                return false;
            }
        }

        // Validate user login
        public async Task<bool> ValidateLoginAsync(string username, string password)
        {
            try
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    await conn.OpenAsync();

                    string query = "SELECT COUNT(*) FROM app_user WHERE username = @username AND password = @password";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@password", password);

                        var result = await cmd.ExecuteScalarAsync();
                        int count = Convert.ToInt32(result);

                        return count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Login validation failed: {ex.Message}");
                throw;
            }
        }
    }
}