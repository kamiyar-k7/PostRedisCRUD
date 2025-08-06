using Npgsql;

namespace PostRedisCRUD.Data;

public static class HostExtensions
{
    public static IHost MigrateDatabase<TContext>(this IHost host, int? retry = 0)
    {
        int retryForAvailability = retry ?? 0;

        using var scope = host.Services.CreateScope();
        var services = scope.ServiceProvider;
        var configuration = services.GetRequiredService<IConfiguration>();
        var logger = services.GetRequiredService<ILogger<TContext>>();


        var originalConnString = configuration.GetConnectionString("PostgresqlConnectionString");
        var builder = new NpgsqlConnectionStringBuilder(originalConnString);
        string dbName = builder.Database;

        try
        {
            logger.LogInformation("Starting database migration...");

           
            builder.Database = "postgres";

            using (var conn = new NpgsqlConnection(builder.ToString()))
            {
                conn.Open();

                using var cmd = new NpgsqlCommand($"SELECT 1 FROM pg_database WHERE datname = '{dbName}'", conn);
                var exists = cmd.ExecuteScalar();

                if (exists == null)
                {
                    logger.LogInformation($"📦 Database '{dbName}' does not exist. Creating...");
                    using var createCmd = new NpgsqlCommand($"CREATE DATABASE \"{dbName}\"", conn);
                    createCmd.ExecuteNonQuery();
                    logger.LogInformation($"✅ Database '{dbName}' created.");
                }
                else
                {
                    logger.LogInformation($"✅ Database '{dbName}' already exists.");
                }
            }

   
            using var dbConnection = new NpgsqlConnection(originalConnString);
            dbConnection.Open();

            using var command = new NpgsqlCommand
            {
                Connection = dbConnection
            };

            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS Product (
                    Id SERIAL PRIMARY KEY,
                    Name VARCHAR(200) NOT NULL,
                    Description TEXT,
                    Price DECIMAL
                )";
            command.ExecuteNonQuery();

            // seed data only if empty
            command.CommandText = "SELECT COUNT(*) FROM Product";
            var count = (long)command.ExecuteScalar();
            if (count == 0)
            {
                command.CommandText = @"
                    INSERT INTO Product(Name, Description, Price)
                    VALUES ('Laptop', 'This is a Laptop', 20.2)";
                command.ExecuteNonQuery();
                logger.LogInformation("✅ Seed data inserted.");
            }

            logger.LogInformation("✅ Migration completed.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "❌ Error during migration: {Message}", ex.Message);
            if (retryForAvailability < 50)
            {
                retryForAvailability++;
                Thread.Sleep(200);
                return MigrateDatabase<TContext>(host, retryForAvailability);
            }
        }

        return host;
    }
}
