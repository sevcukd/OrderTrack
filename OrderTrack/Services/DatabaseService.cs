using System;
using System.Data.SQLite;
using System.IO;
namespace OrderTrack.Services
{
    public static class DatabaseService
    {
        public static string GetDatabaseName()
        {
            string today = DateTime.Now.ToString("ddMMyy");
            return $"Orders_{today}.db";
        }

        public static string GetConnectionString()
        {
            string databaseName = GetDatabaseName();
            return $"Data Source={databaseName};Version=3;";
        }

        public static void CreateDailyDatabase()
        {
            string databaseName = GetDatabaseName();
            string connectionString = GetConnectionString();

            if (!File.Exists(databaseName))
            {
                SQLiteConnection.CreateFile(databaseName);

                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    string sql = @"
                    CREATE TABLE Orders (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        IdWorkplace INTEGER NOT NULL,
                        CodePeriod INTEGER NOT NULL,
                        CodeReceipt INTEGER NOT NULL,
                        CodeWares INTEGER NOT NULL,
                        DateCreate DATETIME,
                        DateStart DATETIME,
                        DateEnd DATETIME,
                        Type TEXT,
                        JSON TEXT
                    );";

                    using (var command = new SQLiteCommand(sql, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}
