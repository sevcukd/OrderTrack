using System.Data.SQLite;
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
                        JSON TEXT
                    );
        CREATE TABLE USER(
            CODE_USER INTEGER NOT NULL,
            NAME_USER TEXT NOT NULL,
            BAR_CODE TEXT NOT NULL,
            Type_User INTEGER NOT NULL,
            LOGIN TEXT NOT NULL,
            PASSWORD TEXT NOT NULL
        );
        CREATE TABLE Order_Wares(
            ID_WORKPLACE INTEGER  NOT NULL,
            CODE_PERIOD INTEGER  NOT NULL,
            CODE_RECEIPT INTEGER  NOT NULL,
            CODE_WARES INTEGER  NOT NULL,
            QUANTITY NUMBER   NOT NULL,
            SORT INTEGER  NOT NULL,
            DATE_CREATE DATETIME NOT NULL DEFAULT (datetime('now','localtime')),
            USER_CREATE INTEGER  NOT NULL
            );
CREATE TABLE OrderReceiptLink(
            IdWorkplace INTEGER  NOT NULL,
            CodePeriod INTEGER  NOT NULL,
            CodeReceipt INTEGER  NOT NULL,
            CodeWares INTEGER NOT NULL,
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
