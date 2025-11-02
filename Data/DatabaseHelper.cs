using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Data.Sqlite;
using EnergyApp.Models;

namespace EnergyApp.Data
{
    public static class DatabaseHelper
    {
        private static readonly string DbFile = "energy_data.db";
        private static readonly string ConnectionString = $"Data Source={DbFile}";

        public static void InitializeDatabase()
        {
            // Создать файл БД, если нет
            if (!File.Exists(DbFile))
                File.Create(DbFile).Dispose();

            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            // Создаём таблицу
            string createTable = @"
                CREATE TABLE IF NOT EXISTS EnergyRecords (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Date TEXT NOT NULL,
                    Consumption REAL NOT NULL,
                    PricePerKwh REAL NOT NULL,
                    Cost REAL NOT NULL,
                    Comment TEXT
                );";
            using var cmd = new SqliteCommand(createTable, connection);
            cmd.ExecuteNonQuery();
        }

        public static void AddRecord(EnergyRecord record)
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            string insertQuery = @"
                INSERT INTO EnergyRecords (Date, Consumption, PricePerKwh, Cost, Comment)
                VALUES (@Date, @Consumption, @PricePerKwh, @Cost, @Comment);";

            using var cmd = new SqliteCommand(insertQuery, connection);
            cmd.Parameters.AddWithValue("@Date", record.Date.ToString("yyyy-MM-dd"));
            cmd.Parameters.AddWithValue("@Consumption", record.Consumption);
            cmd.Parameters.AddWithValue("@PricePerKwh", record.PricePerKwh);
            cmd.Parameters.AddWithValue("@Cost", record.Cost);
            cmd.Parameters.AddWithValue("@Comment", record.Comment ?? "");
            cmd.ExecuteNonQuery();
        }

        public static List<EnergyRecord> GetRecords()
        {
            var records = new List<EnergyRecord>();
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            string selectQuery = "SELECT Id, Date, Consumption, PricePerKwh, Cost, Comment FROM EnergyRecords ORDER BY Date DESC;";
            using var cmd = new SqliteCommand(selectQuery, connection);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                records.Add(new EnergyRecord
                {
                    Id = reader.GetInt32(0),
                    Date = DateTime.Parse(reader.GetString(1)).Date,
                    Consumption = reader.GetDouble(2),
                    PricePerKwh = reader.GetDouble(3),
                    Cost = reader.GetDouble(4),
                    Comment = reader.IsDBNull(5) ? "" : reader.GetString(5)
                });
            }

            return records;
        }

        public static void DeleteRecord(int id)
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            string deleteQuery = "DELETE FROM EnergyRecords WHERE Id = @Id;";
            using var cmd = new SqliteCommand(deleteQuery, connection);
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.ExecuteNonQuery();
        }

        // Получить последнюю цену (если хочешь подставлять её в AddRecordWindow по умолчанию)
        public static double GetLastPrice()
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            string query = "SELECT PricePerKwh FROM EnergyRecords ORDER BY Id DESC LIMIT 1;";
            using var cmd = new SqliteCommand(query, connection);
            var result = cmd.ExecuteScalar();
            return result == null || result == DBNull.Value ? 5.0 : Convert.ToDouble(result);
        }
    }
}