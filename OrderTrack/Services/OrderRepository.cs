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

        public IEnumerable<OrderEntity> GetAllOrders()
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                return connection.Query<OrderEntity>("SELECT * FROM Orders").ToList();
            }
        }

        public void AddOrder(OrderEntity order)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                string sql = @"INSERT INTO Orders (IdWorkplace, CodePeriod, CodeReceipt, CodeWares, DateCreate, DateStart, DateEnd, Type, JSON)
                               VALUES (@IdWorkplace, @CodePeriod, @CodeReceipt, @CodeWares, @DateCreate, @DateStart, @DateEnd, @Type, @JSON)";
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
