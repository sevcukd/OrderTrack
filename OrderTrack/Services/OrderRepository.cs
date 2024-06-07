using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using Dapper;
using OrderTrack.Models;

namespace OrderTrack.Services
{
    public class OrderRepository
    {
        private readonly string _connectionString;

        public OrderRepository()
        {
            _connectionString = DatabaseService.GetConnectionString();
        }

        public IEnumerable<Order> GetAllOrders()
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                return connection.Query<Order>("SELECT * FROM Orders").ToList();
            }
        }
        public IEnumerable<Order> GetActiveOrders()
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                return connection.Query<Order>($"SELECT * FROM Orders where Status <{(int)eStatus.Ready};").ToList();
            }
        }
        public IEnumerable<Order> GetReadyOrders()
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                return connection.Query<Order>($"SELECT * FROM Orders where Status = {(int)eStatus.Ready};").ToList();
            }
        }
        /// для тесту 
        public void AddOrders(IEnumerable<Order> orders)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                string sql = $@"INSERT INTO Orders (IdWorkplace, Status, CodePeriod, CodeReceipt, CodeWares, DateCreate, DateStart, DateEnd, Type, JSON)
                               VALUES (@{nameof(Order.IdWorkplace)},@{nameof(Order.Status)}, @{nameof(Order.CodePeriod)}, @{nameof(Order.CodeReceipt)}, @{nameof(Order.CodeWares)}, @{nameof(Order.DateCreate)}, @{nameof(Order.DateStart)}, @{nameof(Order.DateEnd)}, @{nameof(Order.Type)}, @{nameof(Order.JSON)})";
                connection.Execute(sql, orders);
            }
        }
        public void AddOrder(Order order)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                string sql = $@"INSERT INTO Orders (IdWorkplace, Status, CodePeriod, CodeReceipt, CodeWares, DateCreate, DateStart, DateEnd, Type, JSON)
                               VALUES (@{nameof(Order.IdWorkplace)},@{nameof(Order.Status)}, @{nameof(Order.CodePeriod)}, @{nameof(Order.CodeReceipt)}, @{nameof(Order.CodeWares)}, @{nameof(Order.DateCreate)}, @{nameof(Order.DateStart)}, @{nameof(Order.DateEnd)}, @{nameof(Order.Type)}, @{nameof(Order.JSON)})";
                connection.Execute(sql, order);
            }
        }

        public void DeleteOrder(int id)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                string sql = "DELETE FROM Orders WHERE Id = @Id";
                connection.Execute(sql, new { Id = id });
            }
        }
    }
}
