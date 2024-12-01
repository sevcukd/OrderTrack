using Dapper;
using OrderTrack.Models;
using System.Data.SQLite;

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
           return GetOrders("SELECT * FROM Orders");
        }
        public IEnumerable<Order> GetActiveOrders()
        {
            return GetOrders($"SELECT * FROM Orders where Status <{(int)eStatus.Ready};");
        }

        private IEnumerable<Order> GetOrders(string sqlOrders)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                string sqlOrderWares = "SELECT * FROM OrderWares WHERE CodeReceipt = @CodeReceipt";
                string sqlOrderReceiptLinks = "SELECT * FROM OrderReceiptLink WHERE CodeReceipt = @CodeReceipt";

                var orders = connection.Query<Order>(sqlOrders).ToList();

                foreach (var order in orders)
                {
                    // Load OrderWares for each order
                    var orderWares = connection.Query<OrderWares>(sqlOrderWares, new { CodeReceipt = order.CodeReceipt }).ToList();
                    foreach (var wares in orderWares)
                    {
                        // Load ReceiptLinks for each OrderWares
                        wares.ReceiptLinks = connection.Query<OrderReceiptLink>(sqlOrderReceiptLinks, new { CodeReceipt = wares.CodeReceipt }).ToList();
                    }
                    order.Wares = orderWares;
                }

                return orders;
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
            foreach (var order in orders)
            {
                AddOrder(order); //TMP - для тесту
            }
            //using (var connection = new SQLiteConnection(_connectionString))
            //{
            //    string sql = $@"INSERT INTO Orders (IdWorkplace, Status, CodePeriod, CodeReceipt,  DateCreate, DateStart, DateEnd, Type, JSON)
            //                   VALUES (@{nameof(Order.IdWorkplace)},@{nameof(Order.Status)}, @{nameof(Order.CodePeriod)}, @{nameof(Order.CodeReceipt)},  @{nameof(Order.DateCreate)}, @{nameof(Order.DateStart)}, @{nameof(Order.DateEnd)}, @{nameof(Order.Type)}, @{nameof(Order.JSON)})";
            //    connection.Execute(sql, orders);
            //}
        }
        public int AddOrder(Order order)
        {
            AddWaresOrder(order.Wares);
            using (var connection = new SQLiteConnection(_connectionString))
            {
                string sql = $@"INSERT INTO Orders (IdWorkplace, Status, CodePeriod, CodeReceipt,  DateCreate, DateStart, DateEnd, Type, JSON)
                               VALUES (@{nameof(Order.IdWorkplace)},@{nameof(Order.Status)}, @{nameof(Order.CodePeriod)}, @{nameof(Order.CodeReceipt)},  @{nameof(Order.DateCreate)}, @{nameof(Order.DateStart)}, @{nameof(Order.DateEnd)}, @{nameof(Order.Type)}, @{nameof(Order.JSON)});
                                SELECT last_insert_rowid();";
                return connection.ExecuteScalar<int>(sql, order);
            }
        }
        public void AddWaresOrder(IEnumerable<OrderWares> orderWares)
        {
            foreach (var wares in orderWares)
                AddLinkWares(wares.ReceiptLinks);


            using (var connection = new SQLiteConnection(_connectionString))
            {
                string sql = $@"
                INSERT INTO OrderWares (IdWorkplace, CodePeriod, CodeReceipt,  CodeWares, NameWares, Quantity, Sort, DateCreate, UserCreate)
                VALUES (@{nameof(OrderWares.IdWorkplace)}, @{nameof(OrderWares.CodePeriod)}, @{nameof(OrderWares.CodeReceipt)}, @{nameof(OrderWares.CodeWares)}, @{nameof(OrderWares.NameWares)}, @{nameof(OrderWares.Quantity)}, @{nameof(OrderWares.Sort)}, @{nameof(OrderWares.DateCreate)}, @{nameof(OrderWares.UserCreate)});";

                connection.Execute(sql, orderWares);
            }
        }
        public void AddLinkWares(IEnumerable<OrderReceiptLink> orderReceiptLink)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                string sql = $@"
                INSERT INTO OrderReceiptLink (IdWorkplace, CodePeriod, Name, CodeReceipt, CodeWares, Quantity, CodeWaresTo, Sort)
                VALUES (@{nameof(OrderReceiptLink.IdWorkplace)}, @{nameof(OrderReceiptLink.CodePeriod)}, @{nameof(OrderReceiptLink.Name)}, @{nameof(OrderReceiptLink.CodeReceipt)}, @{nameof(OrderReceiptLink.CodeWares)}, @{nameof(OrderReceiptLink.Quantity)}, @{nameof(OrderReceiptLink.CodeWaresTo)}, @{nameof(OrderReceiptLink.Sort)});";

                connection.Execute(sql, orderReceiptLink);
            }

        }
        public IEnumerable<int> GetOrderId(Order order)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                string sql = $"SELECT Id FROM Orders WHERE CodeReceipt = @{nameof(Order.CodeReceipt)}";
                var a = connection.Query<int>(sql, order).ToList();
                return a;

            }
        }
        public Order UpdateOrder(UpdateModel order)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                string sql = @"
                UPDATE Orders 
                SET 
                    Status = @Status
                WHERE Id = @Id;
                SELECT * FROM Orders WHERE Id = @Id;
                ";

                return connection.QuerySingle<Order>(sql, order);
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
        public void ReplaceUser(IEnumerable<User> pUser)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                string SqlReplaceUser = "replace into User(CODE_USER, NAME_USER, BAR_CODE, Type_User, LOGIN, PASSWORD) values(@CodeUser, @NameUser, @BarCode, @TypeUser, @Login, @PassWord);";
                connection.Execute(SqlReplaceUser, pUser);
            }
        }
    }
}
