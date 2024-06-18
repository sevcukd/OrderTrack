using OrderTrack.Models;
using OrderTrack.Services;
using System.Collections.ObjectModel;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using Timer = System.Timers.Timer;

namespace OrderTrack.ViewModel
{
    public class MainViewModel
    {
        public ObservableCollection<Order> ActiveOrders { get; set; }
        public ObservableCollection<Order> ReadyOrders { get; set; }

        private Timer _timer;

        private readonly OrderRepository _orderRepository;


        public MainViewModel()
        {
            //ActiveOrders = new ObservableCollection<Order>
            //{
            //    new Order {  Status = eStatus.Waiting },
            //    new Order {  Status = eStatus.Preparing }
            //};

            //ReadyOrders = new ObservableCollection<Order>
            //{
            //    new Order {  Status = eStatus.Ready }
            //};

            var aa = Global.SQLiteDatabasePath;

            _orderRepository = new OrderRepository();

            CreateDailyDatabase();

            var newOrders = new List<Order>
        {
            new Order { IdWorkplace = 1,Status=eStatus.Ready,  CodePeriod = 13, CodeReceipt = 56, CodeWares = 89, DateCreate = DateTime.Now, DateStart = DateTime.Now, DateEnd = DateTime.Now, Type = "Type1", JSON = "{}" },
            new Order { IdWorkplace = 2,Status=eStatus.Waiting,  CodePeriod = 123, CodeReceipt = 456, CodeWares = 789, DateCreate = DateTime.Now, DateStart = DateTime.Now, DateEnd = DateTime.Now, Type = "Type1", JSON = "{}" },
            new Order { IdWorkplace = 3,Status=eStatus.Preparing, CodePeriod = 124, CodeReceipt = 457, CodeWares = 790, DateCreate = DateTime.Now, DateStart = DateTime.Now, DateEnd = DateTime.Now, Type = "Type2", JSON = "{}" }
        };

            //AddOrdersToDatabase(newOrders);

            ActiveOrders = GetActiveOrders();
            ReadyOrders = GetReadyOrders();


            _timer = new Timer(60000); // 1 minute interval
            _timer.Elapsed += OnTimerElapsed;
            _timer.Start();
        }
        public void AddOrdersToDatabase(IEnumerable<Order> orders)
        {
            _orderRepository.AddOrders(orders);
        }

        public ObservableCollection<Order> GetActiveOrders ()
        {
            return new ObservableCollection<Order>(_orderRepository.GetActiveOrders());
        }
        public ObservableCollection<Order> GetReadyOrders()
        {
            return new ObservableCollection<Order>(_orderRepository.GetReadyOrders());
        }
        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var ordersToRemove = ReadyOrders.Where(o => o.DateCreate.AddMinutes(5) <= DateTime.Now).ToList();
                foreach (var order in ordersToRemove)
                {
                    ReadyOrders.Remove(order);
                }
            });
        }
        private void CreateDailyDatabase()
        {
            DatabaseService.CreateDailyDatabase();
            var _msSqlRepository = new MsSqlRepository();
            var users = _msSqlRepository.SqlGetUser();
            _orderRepository.ReplaceUser(users);

        }
    }
}
