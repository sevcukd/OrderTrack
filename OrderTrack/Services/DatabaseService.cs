﻿using System.Data.SQLite;
using System.IO;
namespace OrderTrack.Services
{
    public static class DatabaseService
    {
        public static string GetDatabaseName()
        {
            string today = DateTime.Now.ToString("ddMMyy");
            return Path.Combine(Global.SQLiteDatabasePath, $"Orders_{today}.db");
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
                        Status INTEGER NOT NULL,
                        IdWorkplace INTEGER NOT NULL,
                        CodePeriod INTEGER NOT NULL,
                        CodeReceipt INTEGER NOT NULL,
                        DateCreate DATETIME,
                        DateStart DATETIME,
                        DateEnd DATETIME,
                        Type TEXT,
                        CODE_USER INTEGER NOT NULL DEFAULT 0,
                        JSON TEXT
                    );
        CREATE TABLE ReceiptWaresLink (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    CodeWaresTo INTEGER NOT NULL,
    Sort INTEGER NOT NULL DEFAULT 0,
    Quantity NUMBER NOT NULL DEFAULT 0,
    NameWares TEXT NOT NULL,
    CodeWares INTEGER NOT NULL,
    IdWorkplace INTEGER NOT NULL,
    CodePeriod INTEGER NOT NULL,
    CodeReceipt INTEGER NOT NULL,
    DateCreate DATETIME NOT NULL DEFAULT (datetime('now','localtime'))
);
  
        CREATE TABLE USER(
            CODE_USER INTEGER NOT NULL,
            NAME_USER TEXT NOT NULL,
            BAR_CODE TEXT NOT NULL,
            Type_User INTEGER NOT NULL,
            LOGIN TEXT NOT NULL,
            PASSWORD TEXT NOT NULL
        );

CREATE TABLE OrderWares (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        IdWorkplace INTEGER  NOT NULL,
                        CodePeriod INTEGER  NOT NULL,
                        CodeReceipt INTEGER  NOT NULL,
                        CodeWares INTEGER  NOT NULL,
                        NameWares TEXT NOT NULL,
                        Quantity NUMBER  NOT NULL,
                        Sort INTEGER   NOT NULL,
                        DateCreate DATETIME NOT NULL DEFAULT (datetime('now','localtime')),
                        UserCreate INTEGER
                    );


CREATE TABLE OrderReceiptLink(
            IdWorkplace INTEGER  NOT NULL,
            CodePeriod INTEGER  NOT NULL,
            CodeReceipt INTEGER  NOT NULL,
            CodeWares INTEGER NOT NULL,
            Name TEXT NOT NULL,
            Sort INTEGER NOT NULL default 0,
            CodeWaresTo INTEGER NOT NULL,            
            Quantity NUMBER NOT NULL default 0
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
