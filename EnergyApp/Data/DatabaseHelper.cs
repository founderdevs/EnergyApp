using EnergyApp.Models;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;

namespace EnergyApp.Data
{
    public class DatabaseHelper
    {
        private static string dbFile = "energy_data.db";
        private static string connectionString = $"Data Source={dbFile};Version=3;";

        public static void InitializeDatabase()
        {
            if (!File.Exists(dbFile))
                SQLiteConnection.CreateFile(dbFile);

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string tableCommand = @"CREATE TABLE IF NOT EXISTS EnergyRecords (
                                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                        Date TEXT NOT NULL,
                                        Consumption REAL NOT NULL,
                                        Cost REAL NOT NULL,
                                        Comment TEXT)";
                SQLiteCommand command = new SQLiteCommand(tableCommand, connection);
                command.ExecuteNonQuery();
            }
        }

        public static void AddRecord(EnergyRecord record)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string insertQuery = "INSERT INTO EnergyRecords (Date, Consumption, Cost, Comment) VALUES (@Date, @Consumption, @Cost, @Comment)";
                using (var command = new SQLiteCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@Date", record.Date.ToString("yyyy-MM-dd"));
                    command.Parameters.AddWithValue("@Consumption", record.Consumption);
                    command.Parameters.AddWithValue("@Cost", record.Cost);
                    command.Parameters.AddWithValue("@Comment", record.Comment);
                    command.ExecuteNonQuery();
                }
            }
        }

        public static List<EnergyRecord> GetRecords()
        {
            var records = new List<EnergyRecord>();
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string selectQuery = "SELECT * FROM EnergyRecords";
                using (var command = new SQLiteCommand(selectQuery, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        records.Add(new EnergyRecord
                        {
                            Id = reader.GetInt32(0),
                            Date = DateTime.Parse(reader.GetString(1)),
                            Consumption = reader.GetDouble(2),
                            Cost = reader.GetDouble(3),
                            Comment = reader.IsDBNull(4) ? "" : reader.GetString(4)
                        });
                    }
                }
            }
            return records;
        }

        public static void DeleteRecord(int id)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string deleteQuery = "DELETE FROM EnergyRecords WHERE Id = @Id";
                using (var command = new SQLiteCommand(deleteQuery, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}