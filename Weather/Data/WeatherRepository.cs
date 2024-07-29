using System.Collections.Concurrent;
using Microsoft.Data.SqlClient;

namespace Weather.Data
{
    public class WeatherRepository
    {
        private readonly string _connectionString;
        private readonly string _connectionStringForMigration;
        private ConcurrentDictionary<int, string?> _cache; // It's a thread-safe data structure to store the last weather forecast response. We always store exactly one pair within it (with key = 0).
        public string? Cache
        {
            get => _cache[0];
        }

        public WeatherRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _connectionStringForMigration = configuration.GetConnectionString("MigrationConnection");
            _cache = new ConcurrentDictionary<int, string?>();
            _cache[0] = null;
        }

        public async Task SaveWeatherDataAsync(string weatherData)
        {
            _cache[0] = weatherData;
            using (SqlConnection conn = new(_connectionString))
            {
                await conn.OpenAsync();
                var sql = "UPDATE WeatherData SET Data = @Data";
                using (SqlCommand cmd = new(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Data", weatherData);

                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        // InitializeDatabaseAsync creates the required DB (if not exist) and also provide the required table with a single null record (if not exist).
        public async Task InitializeDatabaseAsync()
        {
            using (SqlConnection conn = new(_connectionStringForMigration))
            {
                await conn.OpenAsync();

                var createDbCommand = @"
                IF DB_ID('WeatherDB') IS NULL
                BEGIN
                    CREATE DATABASE WeatherDB;
                END;";

                using (SqlCommand cmd = new(createDbCommand, conn))
                {
                    await cmd.ExecuteNonQueryAsync();
                }

                conn.ChangeDatabase("WeatherDB");

                var initTableCommand = @"
                IF OBJECT_ID('WeatherData', 'U') IS NULL
                BEGIN
                    CREATE TABLE WeatherData (
                        Data VARCHAR(MAX) NULL
                    );
                    INSERT INTO WeatherData VALUES (NULL);
                END;";

                using (SqlCommand cmd = new(initTableCommand, conn))
                {
                    await cmd.ExecuteNonQueryAsync();
                }

                // Loading Cache from DB
                using (SqlCommand cmd = new("SELECT Data FROM WeatherData", conn))
                {
                    var r = await cmd.ExecuteScalarAsync();
                    if (r is not null && r != DBNull.Value)
                    {
                        _cache[0] = (string)r;
                    }
                }
            }
        }
    }
}
