﻿using ModelMID;
using Newtonsoft.Json;
using OrderTrack.Models;
using OrderTrack.Services;
//using OrderTrack.Sockets;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Dynamic;
using System.Timers;
using System.Windows;
using System.Xml.Linq;
using Utils;
using UtilNetwork;
using Timer = System.Timers.Timer;

namespace OrderTrack.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
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
                _socketServer = new SocketServer(Global.PortAPI, CallBackApi, 65536);
                _ = _socketServer.StartAsync();
            }
            FileLogger.Init(Global.PathLog, 0);

            _orderRepository = new OrderRepository();

            CreateDailyDatabase();

            /////// ДЛЯ ТЕСТУ
            /*     var TESTorderWares = new List<OrderWares>
             {
                 new OrderWares
                 {
                     IdWorkplace = 77,
                     CodePeriod = 20241122,
                     CodeReceipt = 456,
                     CodeWares = 189548,
                     NameWares = "Pizza Amatriciano 500g",
                     Quantity = 1,
                     Sort = 1,
                     DateCreate = DateTime.Now,
                     UserCreate = 1,
                     ReceiptLinks = new List<OrderReceiptLink>
                     {
                         new OrderReceiptLink { IdWorkplace = 77, CodePeriod = 20241122, CodeReceipt = 456, CodeWares = 160584,Name = "Добавка до піци Ананас 30г", Quantity = 2m, CodeWaresTo = 189548, Sort = 1 }
                     }
                 },
                 new OrderWares
                 {
                     IdWorkplace = 77,
                     CodePeriod = 20241122,
                     CodeReceipt = 457,
                     CodeWares = 169293,
                     NameWares = "Хот-дог Баварський  шт",
                     Quantity = 4,
                     Sort = 2,
                     DateCreate = DateTime.Now,
                     UserCreate = 1,
                     ReceiptLinks = new List<OrderReceiptLink>
                     {
                         new OrderReceiptLink { IdWorkplace = 77, CodePeriod = 20241122, CodeReceipt = 457, CodeWares = 169269,Name = "Соус 28г", Quantity = 5.0m, CodeWaresTo = 169293, Sort = 2 }
                     }
                 }
             };
                 var newOrders = new List<Order>
             {
                 new Order { IdWorkplace = 1,Status=eStatus.Ready,  CodePeriod = 13, CodeReceipt = 56,  DateCreate = DateTime.Now, DateStart = DateTime.Now, DateEnd = DateTime.Now, Type = "Type1", JSON = "{}", Wares = TESTorderWares },
                 new Order { IdWorkplace = 2,Status=eStatus.Waiting,  CodePeriod = 123, CodeReceipt = 456, DateCreate = DateTime.Now, DateStart = DateTime.Now, DateEnd = DateTime.Now, Type = "Type1", JSON = "{}", Wares = TESTorderWares },
                           new Order { IdWorkplace = 3,Status=eStatus.Preparing, CodePeriod = 124, CodeReceipt = 457,  DateCreate = DateTime.Now, DateStart = DateTime.Now, DateEnd = DateTime.Now, Type = "Type2", JSON = "{}", Wares = TESTorderWares },

                 new Order { IdWorkplace = 3,Status=eStatus.Preparing, CodePeriod = 124, CodeReceipt = 457,  DateCreate = DateTime.Now, DateStart = DateTime.Now, DateEnd = DateTime.Now, Type = "Type2", JSON = "{}", Wares = TESTorderWares }
             };

                 AddOrdersToDatabase(newOrders);*/
            /////// ДЛЯ ТЕСТУ
            RefreshMenu();


            _timer = new Timer(60000); // 1 minute interval
            _timer.Elapsed += OnTimerElapsed;
            _timer.Start();
        }
        public void RefreshMenu()
        {
            ActiveOrders = GetActiveOrders();
            ReadyOrders = new ObservableCollection<Order>(GetReadyOrders().OrderByDescending(p => p.DateEnd));
            OnTimerElapsed(null, null);
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
                var ordersToRemove = ReadyOrders.Where(o => o.DateEnd.AddMinutes(5) <= DateTime.Now).ToList();
                foreach (var order in ordersToRemove)
                {
                    ReadyOrders.Remove(order);
                }
            });
        }
        private void CreateDailyDatabase()
        {
            FileLogger.WriteLogMessage(this, System.Reflection.MethodBase.GetCurrentMethod().Name, "Start create daily DB");
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
                CommandAPI<Order> ComandOrder;
                CommandAPI<UpdateModel> ComandOrder2;


                //CommandAPI<InfoRemoteCheckout> CommandRemoteInfo;
                switch (pC.Command)
                {
                    case eCommand.GetOrderNumber:
                        if (!DatabaseService.IsPresentDB())
                            CreateDailyDatabase();

                        //код для отримання номера замовлення
                        ComandOrder = JsonConvert.DeserializeObject<CommandAPI<Order>>(pDataApi);
                        int orderNumber = CreateOrder(ComandOrder.Data, pDataApi);
                        Res = new Status(0, $"{orderNumber}");
                        //_socketServer.NotifyAllClientsAsync("123");
                        RefreshMenu();
                        FileLogger.WriteLogMessage("GetOrderNumber", System.Reflection.MethodBase.GetCurrentMethod().Name, $"Order Number => {Res.TextState}{Environment.NewLine}{pDataApi}");
                        break;
                    case eCommand.ChangeOrderState:
                        if (!DatabaseService.IsPresentDB())
                            CreateDailyDatabase();
                        ComandOrder2 = JsonConvert.DeserializeObject<CommandAPI<UpdateModel>>(pDataApi);
                        var order = UpdateOrder(ComandOrder2.Data, pDataApi);
                        Status<Order> OrderStatus = new(order);
                        Res = OrderStatus;
                        RefreshMenu();
                        FileLogger.WriteLogMessage("ChangeOrderState", System.Reflection.MethodBase.GetCurrentMethod().Name, $"Зміна стану замовлення №{ComandOrder2.Data.Id} {Environment.NewLine}{pDataApi}");

                        break;
                    case eCommand.GetAllOrders:
                        if (!DatabaseService.IsPresentDB())
                            CreateDailyDatabase();
                        var AllOrders = GetAllOrders(pDataApi);
                        Status<IEnumerable<Order>> AllOrdersStatus = new(AllOrders);
                        Res = AllOrdersStatus;
                        FileLogger.WriteLogMessage("GetAllOrders", System.Reflection.MethodBase.GetCurrentMethod().Name, $"Знайдено {AllOrders.Count()} замовлень{Environment.NewLine}{pDataApi}");
                        break;

                    case eCommand.GetActiveOrders:
                        if (!DatabaseService.IsPresentDB())
                            CreateDailyDatabase();
                        var ActiveOrders = GetActiveOrders(pDataApi);
                        Status<IEnumerable<Order>> ActiveOrdersStatus = new(ActiveOrders);
                        Res = ActiveOrdersStatus;
                        FileLogger.WriteLogMessage("GetActiveOrders", System.Reflection.MethodBase.GetCurrentMethod().Name, $"Знайдено {ActiveOrders.Count()} активних замовлень{Environment.NewLine}{pDataApi}");
                        break;
                    default:
                        Res = new Status(0, $"Не існує метода для обробки команди {pC.Command}!");
                        FileLogger.WriteLogMessage("Error", System.Reflection.MethodBase.GetCurrentMethod().Name, $"Не припустима команда: {pC.Command}{Environment.NewLine}{pDataApi}");
                        break;
                }
            }
            catch (Exception ex)
            {
                Res = new Status(ex);
                FileLogger.WriteLogMessage(this, System.Reflection.MethodBase.GetCurrentMethod().Name, $"{ex}{Environment.NewLine}{pDataApi}");
            }
            return Res;
        }
        private int CreateOrder(Order order, string json)
        {
            // List<OrderWares> wares = new();
            // foreach (var goods in receipt.Wares)
            //{
            //    if (goods.ProductionLocation > 0) // Перевірка чи товар потрібно готувати на якісь із зон
            //        wares.Add(new OrderWares(goods));
            // }
            //var order = (new Order { IdWorkplace = receipt.IdWorkplace, Status = eStatus.Waiting, CodePeriod = receipt.CodePeriod, CodeReceipt = receipt.CodeReceipt, DateCreate = DateTime.Now, DateStart = DateTime.Now, DateEnd = DateTime.Now, Type = receipt.TranslationTypeReceipt, JSON = json, Wares = wares });
            order.JSON = json;
            int OrderNumber = _orderRepository.AddOrder(order);
            RefreshMenu();
            return OrderNumber;
        }
        private Order UpdateOrder(UpdateModel order, string json) // json - в подальшому для логування
        {
            return _orderRepository.UpdateOrder(order);
        }
        private IEnumerable<Order> GetAllOrders(string json)// json - в подальшому для логування
        {
            return _orderRepository.GetAllOrders();
        }
        private IEnumerable<Order> GetActiveOrders(string json)// json - в подальшому для логування
        {
            return _orderRepository.GetActiveOrders();
        }
        private void OnPropertyChanged(String info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }
    }
}
