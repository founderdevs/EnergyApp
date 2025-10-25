using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using System.IO;

namespace EnergyApp.Data
{
    public class DatabaseService
    {
        private readonly string _dbPath;

        public DatabaseService()
        {
            _dbPath = Path.Combine(Directory.GetCurrentDirectory(), "energy.db");
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            if (!File.Exists(_dbPath))
            {
                using var connection = new SqliteConnection($"Data Source={_dbPath}");
                connection.Open();

                string createTableQuery = @"
                    CREATE TABLE IF NOT EXISTS Readings (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        CounterName TEXT NOT NULL,
                        Date TEXT NOT NULL,
                        Value TEXT NOT NULL
                    );";

                using var command = new SqliteCommand(createTableQuery, connection);
                command.ExecuteNonQuery();
            }
        }

        public SqliteConnection GetConnection()
        {
            return new SqliteConnection($"Data Source={_dbPath}");
        }
    }
}
