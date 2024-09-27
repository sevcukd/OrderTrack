using ModelMID;
using Newtonsoft.Json;
using OrderTrack.Models;
using OrderTrack.Services;
using OrderTrack.Sockets;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Timers;
using System.Windows;
using Utils;
using Timer = System.Timers.Timer;

namespace OrderTrack.ViewModel
{
    public class MainViewModel: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public ObservableCollection<Order> ActiveOrders { get; set; }
        public ObservableCollection<Order> ReadyOrders { get; set; }

        private Timer _timer;

        private readonly OrderRepository _orderRepository;
        SocketServer _socketServer;

        public MainViewModel()
        {
            if (Global.PortAPI > 0)
            {
                _socketServer = new SocketServer(Global.PortAPI, CallBackApi);
                _ = _socketServer.StartAsync();
            }

            _orderRepository = new OrderRepository();

            CreateDailyDatabase();

            var newOrders = new List<Order>
        {
            new Order { IdWorkplace = 1,Status=eStatus.Ready,  CodePeriod = 13, CodeReceipt = 56,  DateCreate = DateTime.Now, DateStart = DateTime.Now, DateEnd = DateTime.Now, Type = "Type1", JSON = "{}" },
            new Order { IdWorkplace = 2,Status=eStatus.Waiting,  CodePeriod = 123, CodeReceipt = 456, DateCreate = DateTime.Now, DateStart = DateTime.Now, DateEnd = DateTime.Now, Type = "Type1", JSON = "{}" },
            new Order { IdWorkplace = 3,Status=eStatus.Preparing, CodePeriod = 124, CodeReceipt = 457,  DateCreate = DateTime.Now, DateStart = DateTime.Now, DateEnd = DateTime.Now, Type = "Type2", JSON = "{}" }
        };

            AddOrdersToDatabase(newOrders);

            //var test2 = _orderRepository.GetOrderId(order);
            RefreshMenu();


            _timer = new Timer(60000); // 1 minute interval
            _timer.Elapsed += OnTimerElapsed;
            _timer.Start();
        }
        public void RefreshMenu()
        {
            ActiveOrders = GetActiveOrders();
            ReadyOrders = GetReadyOrders();
            OnPropertyChanged(nameof(ActiveOrders));
            OnPropertyChanged(nameof(ReadyOrders));
        }
        public void AddOrdersToDatabase(IEnumerable<Order> orders)
        {
            _orderRepository.AddOrders(orders);
        }

        public ObservableCollection<Order> GetActiveOrders()
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
        /// <summary>
        /// Отримання команди по сокету, її обробка та надсилання відповіді
        /// </summary>
        /// <param name="pDataApi"></param>
        /// <returns></returns>
        Status CallBackApi(string pDataApi)
        {
            Status Res = null;
            try
            {
                CommandAPI<dynamic> pC = JsonConvert.DeserializeObject<CommandAPI<dynamic>>(pDataApi);
                CommandAPI<Receipt> ComandReceipt;
                //CommandAPI<InfoRemoteCheckout> CommandRemoteInfo;
                switch (pC.Command)
                {
                    case eCommand.GetOrderNumber:
                        //код для отримання номера замовлення
                        ComandReceipt = JsonConvert.DeserializeObject<CommandAPI<Receipt>>(pDataApi);
                        int orderNumber = CreateOrder(ComandReceipt.Data, pDataApi);
                        Res = new Status(0, $"{orderNumber}");
                        //MessageBox.Show($"Створено нове замовлення з номером: {orderNumber}");
                        break;
                    default:
                        Res = new Status(0, $"Не існує метода для обробки команди {pC.Command}!");
                        break;
                }
            }
            catch (Exception ex) { Res = new Status(ex); }
            return Res;
        }
        private int CreateOrder(Receipt receipt, string json)
        {
            List<OrderWares> wares = new();
            foreach (var goods in receipt.Wares)
            {
                wares.Add(new OrderWares(goods));
            }
            var order = (new Order { IdWorkplace = receipt.IdWorkplace, Status = eStatus.Waiting, CodePeriod = receipt.CodePeriod, CodeReceipt = receipt.CodeReceipt,  DateCreate = DateTime.Now, DateStart = DateTime.Now,  Type = receipt.TranslationTypeReceipt, JSON =  json, Wares = wares });
            int OrderNumber = _orderRepository.AddOrder(order);
            RefreshMenu();
            return OrderNumber;
        }
        private void OnPropertyChanged(String info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }
    }
}
