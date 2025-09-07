using System;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using RentAPlace.Api.Data;

namespace RentAPlace.Tests.TestUtilities
{
    /// <summary>
    /// Creates a unique SQL Server test database and drops it on dispose.
    /// Change 'Server' if your SQL Server instance is different.
    /// </summary>
    public sealed class SqlServerDb : IDisposable
    {
        // Windows LocalDB default (Visual Studio/SDK). If not available, use "localhost" or "."
        private const string Server = @"LAPTOP-MIKLI6T8\SQLEXPRESS";

        private readonly string _dbName = $"RentAPlace_Test_{Guid.NewGuid():N}";
        private bool _created;

        public DbContextOptions<AppDbContext> Options { get; }

        public SqlServerDb()
        {
            var master = new SqlConnectionStringBuilder
            {
                DataSource = Server,
                InitialCatalog = "master",
                IntegratedSecurity = true,
                TrustServerCertificate = true,
                MultipleActiveResultSets = true
            }.ConnectionString;

            using (var conn = new SqlConnection(master))
            using (var cmd = conn.CreateCommand())
            {
                conn.Open();
                cmd.CommandText = $"IF DB_ID('{_dbName}') IS NULL CREATE DATABASE [{_dbName}];";
                cmd.ExecuteNonQuery();
                _created = true;
            }

            var testConn = new SqlConnectionStringBuilder
            {
                DataSource = Server,
                InitialCatalog = _dbName,
                IntegratedSecurity = true,
                TrustServerCertificate = true,
                MultipleActiveResultSets = true
            }.ConnectionString;

            Options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer(testConn)
                .EnableSensitiveDataLogging()
                .Options;

            using var ctx = new AppDbContext(Options);
            ctx.Database.EnsureCreated(); // builds schema from your EF model
        }

        public AppDbContext NewContext() => new AppDbContext(Options);

        public void Dispose()
        {
            if (!_created) return;

            var master = new SqlConnectionStringBuilder
            {
                DataSource = Server,
                InitialCatalog = "master",
                IntegratedSecurity = true,
                TrustServerCertificate = true
            }.ConnectionString;

            try
            {
                using var conn = new SqlConnection(master);
                conn.Open();
                using var cmd = conn.CreateCommand();
                cmd.CommandText = $@"
IF DB_ID('{_dbName}') IS NOT NULL
BEGIN
  ALTER DATABASE [{_dbName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
  DROP DATABASE [{_dbName}];
END";
                cmd.ExecuteNonQuery();
            }
            catch
            {
                // ignore cleanup issues
            }
        }
    }
}
