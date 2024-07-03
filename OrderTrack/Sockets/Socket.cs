﻿using Newtonsoft.Json;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Utils;

namespace OrderTrack.Sockets
{
    public class SocketServer
    {
        Func<string, Status> Action;
        bool IsStart = false;
        Socket listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        IPEndPoint ipPoint = new IPEndPoint(IPAddress.Any, 3443);//(IPAddress.Parse(IP), IpPort);

        public SocketServer(int pPort, Func<string, Status> pS)
        {
            Action = pS;
            ipPoint = new IPEndPoint(IPAddress.Any, pPort);
        }

        public async Task StartAsync()
        {
            try
            {
                // связываем сокет с локальной точкой, по которой будем принимать данные
                listenSocket.Bind(ipPoint);

                // начинаем прослушивание
                listenSocket.Listen(10);
                //Console.WriteLine("Сервер запущен. Ожидание подключений...");

                IsStart = true;
                while (IsStart)
                {
                    Socket handler = await listenSocket.AcceptAsync();
                    Console.WriteLine("Connect");
                    try
                    {
                        // получаем сообщение
                        StringBuilder builder = new StringBuilder();
                        int bytes = 0; // количество полученных байтов
                        byte[] data = new byte[1024]; // буфер для получаемых данных

                        do
                        {
                            bytes = await handler.ReceiveAsync(data, SocketFlags.None);
                            builder.Append(Encoding.UTF8.GetString(data, 0, bytes));
                            Console.WriteLine(builder.ToString());
                        }
                        while (handler.Available > 0);

                        var res = Action(builder.ToString());//
                        //Console.WriteLine(DateTime.Now.ToShortTimeString() + ": " + builder.ToString());

                        data = Encoding.UTF8.GetBytes(res.ToJSON());

                        //Console.WriteLine("Відправляємо відповідь");
                        await handler.SendAsync(data, SocketFlags.None);
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            Status res = new Status(ex);
                            var data = Encoding.UTF8.GetBytes(res.ToJSON());
                            await handler.SendAsync(data, SocketFlags.None);
                        }
                        catch (Exception) { };
                        //FileLogger.WriteLogMessage(this, "StartAsync", ex);
                    }
                    finally
                    {
                        // закрываем сокет
                        handler.Shutdown(SocketShutdown.Both);
                        handler.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void Stop()
        {
            IsStart = false;
        }
    }

    public class SocketClient
    {
        Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Any, 3443);//(IPAddress.Parse(IP), IpPort);

        public SocketClient(IPAddress pIP, int pPort)
        {
            ipEndPoint = new IPEndPoint(pIP, pPort);
        }

        public async Task<Status> StartAsync(string pData)
        {
            Status res = null;
            try
            {
                await client.ConnectAsync(ipEndPoint);
                var messageBytes = Encoding.UTF8.GetBytes(pData);
                var aa = await client.SendAsync(messageBytes, SocketFlags.None);

                // Receive ack.
                var buffer = new byte[10000];

                var received = client.ReceiveAsync(buffer, SocketFlags.None);
                received.Wait(5000);
                if (received.IsCompleted)
                {
                    var r = Encoding.UTF8.GetString(buffer, 0, received.Result);
                    res = JsonConvert.DeserializeObject<Status>(r);
                }
                else
                { res = new(-1, "TimeOut"); }
            }
            catch (Exception ex)
            {
                res = new(ex);
            }
            finally
            {
                client.Shutdown(SocketShutdown.Both);
            }
            return res;
        }

        public async Task<Status<D>> StartAsync<D>(string pData, bool IsResultStatus = true)
        {
            Status<D> res = null;
            try
            {
                await client.ConnectAsync(ipEndPoint);
                var messageBytes = Encoding.UTF8.GetBytes(pData);
                var aa = await client.SendAsync(messageBytes, SocketFlags.None);

                // Receive ack.
                var buffer = new byte[10000];

                var received = client.ReceiveAsync(buffer, SocketFlags.None);
                //received.Wait(5000);
                if (received.IsCompleted)
                {
                    var r = Encoding.UTF8.GetString(buffer, 0, received.Result);
                    if (IsResultStatus)
                        res = JsonConvert.DeserializeObject<Status<D>>(r);
                    else
                        res = new() { Data = JsonConvert.DeserializeObject<D>(r) };
                }
                else
                { res = new(-1, "TimeOut"); }
            }
            catch (Exception ex)
            {
                res = new(ex);
            }
            finally
            {
                client.Shutdown(SocketShutdown.Both);
            }
            return res;
        }

        public async Task<(Status, byte[])> StartAsyn(byte[] pData)
        {
            Status res = null;
            // Receive ack.
            var buffer = new byte[10000];
            try
            {
                await client.ConnectAsync(ipEndPoint);
                //var messageBytes = Encoding.UTF8.GetBytes(pData);
                var aa = await client.SendAsync(pData, SocketFlags.None);

                var received = client.ReceiveAsync(buffer, SocketFlags.None);

                if (received.IsCompleted)
                {
                    var r = Encoding.UTF8.GetString(buffer, 0, received.Result);
                    res = JsonConvert.DeserializeObject<Status>(r);
                }
                else
                { res = new(-1, "TimeOut"); }
            }
            catch (Exception ex)
            {
                res = new(ex);
            }
            finally
            {
                client.Shutdown(SocketShutdown.Both);
            }
            return (res, buffer);
        }

    }
}
