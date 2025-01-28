using Newtonsoft.Json;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Utils;

namespace OrderTrack.Sockets
{
    //public class SocketServer
    //{
    //    private Func<string, Status> Action;
    //    private bool IsStart = false;
    //    private Socket listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    //    private IPEndPoint ipPoint;
    //    private readonly List<Socket> connectedClients = new List<Socket>();
    //    private readonly object clientLock = new object(); // Для потокобезпечної роботи зі списком клієнтів

    //    public SocketServer(int pPort, Func<string, Status> pS)
    //    {
    //        Action = pS;
    //        ipPoint = new IPEndPoint(IPAddress.Any, pPort);
    //    }

    //    public async Task StartAsync()
    //    {
    //        try
    //        {
    //            listenSocket.Bind(ipPoint); // Зв'язуємо сокет з локальною точкою
    //            listenSocket.Listen(10); // Починаємо слухати
    //            IsStart = true;
    //            Console.WriteLine($"Сервер запущено на порту {ipPoint.Port}. Очікування підключень...");

    //            while (IsStart)
    //            {
    //                var handler = await listenSocket.AcceptAsync();
    //                Console.WriteLine("Клієнт підключився.");

    //                lock (clientLock)
    //                {
    //                    connectedClients.Add(handler); // Додаємо клієнта до списку
    //                }

    //                _ = Task.Run(() => HandleClientAsync(handler));
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            Console.WriteLine($"Помилка сервера: {ex.Message}");
    //        }
    //    }

    //    private async Task HandleClientAsync(Socket client)
    //    {
    //        try
    //        {
    //            using (client)
    //            {
    //                // Отримуємо дані від клієнта
    //                StringBuilder builder = new StringBuilder();
    //                byte[] buffer = new byte[1024];

    //                do
    //                {
    //                    int bytesRead = await client.ReceiveAsync(buffer, SocketFlags.None);
    //                    if (bytesRead == 0) break;

    //                    builder.Append(Encoding.UTF8.GetString(buffer, 0, bytesRead));
    //                }
    //                while (client.Available > 0);

    //                Console.WriteLine($"Отримано повідомлення: {builder}");

    //                // Обробляємо повідомлення
    //                var result = Action(builder.ToString());
    //                var responseData = Encoding.UTF8.GetBytes(result.ToJSON());

    //                // Відправляємо відповідь
    //                await client.SendAsync(responseData, SocketFlags.None);
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            Console.WriteLine($"Помилка обробки клієнта: {ex.Message}");
    //        }
    //        finally
    //        {
    //            // Видаляємо клієнта зі списку
    //            lock (clientLock)
    //            {
    //                connectedClients.Remove(client);
    //            }
    //            Console.WriteLine("Клієнт відключився.");
    //        }
    //    }

    //    public async Task NotifyAllClientsAsync(string message)
    //    {
    //        byte[] messageData = Encoding.UTF8.GetBytes(message);

    //        lock (clientLock)
    //        {
    //            foreach (var client in connectedClients.ToList()) // Створюємо копію списку для уникнення помилок
    //            {
    //                _ = Task.Run(async () =>
    //                {
    //                    try
    //                    {
    //                        await client.SendAsync(messageData, SocketFlags.None);
    //                        Console.WriteLine($"Повідомлення надіслано клієнту.");
    //                    }
    //                    catch (Exception ex)
    //                    {
    //                        Console.WriteLine($"Помилка надсилання повідомлення клієнту: {ex.Message}");
    //                        lock (clientLock)
    //                        {
    //                            connectedClients.Remove(client);
    //                        }
    //                    }
    //                });
    //            }
    //        }
    //    }

    //    public void Stop()
    //    {
    //        IsStart = false;
    //        lock (clientLock)
    //        {
    //            foreach (var client in connectedClients)
    //            {
    //                try
    //                {
    //                    client.Shutdown(SocketShutdown.Both);
    //                    client.Close();
    //                }
    //                catch (Exception ex)
    //                {
    //                    Console.WriteLine($"Помилка закриття сокета клієнта: {ex.Message}");
    //                }
    //            }
    //            connectedClients.Clear();
    //        }
    //        listenSocket.Close();
    //        Console.WriteLine("Сервер зупинено.");
    //    }
    //}


    //public class SocketClient
    //{
    //    Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    //    IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Any, 3443);//(IPAddress.Parse(IP), IpPort);

    //    public SocketClient(IPAddress pIP, int pPort)
    //    {
    //        ipEndPoint = new IPEndPoint(pIP, pPort);
    //    }

    //    public async Task<Status> StartAsync(string pData)
    //    {
    //        Status res = null;
    //        try
    //        {
    //            await client.ConnectAsync(ipEndPoint);
    //            var messageBytes = Encoding.UTF8.GetBytes(pData);
    //            var aa = await client.SendAsync(messageBytes, SocketFlags.None);

    //            // Receive ack.
    //            var buffer = new byte[10000];

    //            var received = client.ReceiveAsync(buffer, SocketFlags.None);
    //            received.Wait(5000);
    //            if (received.IsCompleted)
    //            {
    //                var r = Encoding.UTF8.GetString(buffer, 0, received.Result);
    //                res = JsonConvert.DeserializeObject<Status>(r);
    //            }
    //            else
    //            { res = new(-1, "TimeOut"); }
    //        }
    //        catch (Exception ex)
    //        {
    //            res = new(ex);
    //        }
    //        finally
    //        {
    //            client.Shutdown(SocketShutdown.Both);
    //        }
    //        return res;
    //    }

    //    public async Task<Status<D>> StartAsync<D>(string pData, bool IsResultStatus = true)
    //    {
    //        Status<D> res = null;
    //        try
    //        {
    //            await client.ConnectAsync(ipEndPoint);
    //            var messageBytes = Encoding.UTF8.GetBytes(pData);
    //            var aa = await client.SendAsync(messageBytes, SocketFlags.None);

    //            // Receive ack.
    //            var buffer = new byte[10000];

    //            var received = client.ReceiveAsync(buffer, SocketFlags.None);
    //            //received.Wait(5000);
    //            if (received.IsCompleted)
    //            {
    //                var r = Encoding.UTF8.GetString(buffer, 0, received.Result);
    //                if (IsResultStatus)
    //                    res = JsonConvert.DeserializeObject<Status<D>>(r);
    //                else
    //                    res = new() { Data = JsonConvert.DeserializeObject<D>(r) };
    //            }
    //            else
    //            { res = new(-1, "TimeOut"); }
    //        }
    //        catch (Exception ex)
    //        {
    //            res = new(ex);
    //        }
    //        finally
    //        {
    //            client.Shutdown(SocketShutdown.Both);
    //        }
    //        return res;
    //    }

    //    public async Task<(Status, byte[])> StartAsyn(byte[] pData)
    //    {
    //        Status res = null;
    //        // Receive ack.
    //        var buffer = new byte[10000];
    //        try
    //        {
    //            await client.ConnectAsync(ipEndPoint);
    //            //var messageBytes = Encoding.UTF8.GetBytes(pData);
    //            var aa = await client.SendAsync(pData, SocketFlags.None);

    //            var received = client.ReceiveAsync(buffer, SocketFlags.None);

    //            if (received.IsCompleted)
    //            {
    //                var r = Encoding.UTF8.GetString(buffer, 0, received.Result);
    //                res = JsonConvert.DeserializeObject<Status>(r);
    //            }
    //            else
    //            { res = new(-1, "TimeOut"); }
    //        }
    //        catch (Exception ex)
    //        {
    //            res = new(ex);
    //        }
    //        finally
    //        {
    //            client.Shutdown(SocketShutdown.Both);
    //        }
    //        return (res, buffer);
    //    }

    //}
}
