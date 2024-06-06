using OrderTrack.Models;
using OrderTrack.Services;
using System.Collections.ObjectModel;
using System.Timers;
using System.Windows;
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
            ActiveOrders = new ObservableCollection<Order>
            {
                new Order { OrderNumber = 1, Status = "Waiting", StatusIcon = "Icons/waiting.png" },
                new Order { OrderNumber = 2, Status = "Preparing", StatusIcon = "Icons/preparing.png" }
            };

            ReadyOrders = new ObservableCollection<Order>
            {
                new Order { OrderNumber = 3, Status = "Ready", StatusIcon = "Icons/ready.png" }
            };

            _orderRepository = new OrderRepository();
            DatabaseService.CreateDailyDatabase();

            _timer = new Timer(60000); // 1 minute interval
            _timer.Elapsed += OnTimerElapsed;
            _timer.Start();
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var ordersToRemove = ReadyOrders.Where(o => o.TimeAdded.AddMinutes(5) <= DateTime.Now).ToList();
                foreach (var order in ordersToRemove)
                {
                    ReadyOrders.Remove(order);
                }
            });
        }
    }
}
