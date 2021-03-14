using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using HW5.DB;

namespace HW5
{
    class Server
    {
        static int port;
        static Socket listeningSocket; // Сокет
        static Context context = new Context();
        private static bool isRunning = true;
        static List<IPEndPoint> clients = new List<IPEndPoint>(); 

        static ConcurrentQueue<(string, IPEndPoint)> queue = new ConcurrentQueue<(string, IPEndPoint)>();
        
        static async Task Main(string[] args)
        {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            context.SaveChanges();
            Console.WriteLine("SERVER");
            Console.WriteLine("to stop server input STOP");
            port = Int32.Parse("8081");
            Console.WriteLine();
            
            try
            {
                listeningSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp); 
                Task runqueueTask = RequestsAsync(); 
                Task broadcastTask = BroadcastAsync();
                while (isRunning)
                {
                    if (Console.ReadLine() == "STOP")
                    {
                        isRunning = false;
                        Close();
                        await runqueueTask;
                        await broadcastTask;
                        
                    }
                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Close();
            }
        }
        
        private static void Listen()
        {
            try
            {
                IPEndPoint localIP = new IPEndPoint(IPAddress.Parse("0.0.0.0"), port);
                listeningSocket.Bind(localIP);
                while (isRunning)
                {
                    
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0; 
                    byte[] data = new byte[256];
                    EndPoint remoteIp = new IPEndPoint(IPAddress.Any, 0);

                    do
                    {
                        bytes = listeningSocket.ReceiveFrom(data, ref remoteIp);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    }
                    while (listeningSocket.Available > 0);
                    IPEndPoint remoteFullIp = remoteIp as IPEndPoint; 
                    
                    Console.WriteLine("{0}:{1} - {2}", remoteFullIp.Address.ToString(), remoteFullIp.Port, builder.ToString()); 
                    queue.Enqueue((builder.ToString(), remoteFullIp));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                
                Close();
            }
        }
        
        private static void BroadcastMessage()
        {
            IPEndPoint client = null;
            string message = null;
            string resultRequest = "";
            (string, IPEndPoint) result = new (message, client);
            while (isRunning)
            {
                if (queue.TryDequeue(out result))
                {
                    if (result.Item1 == "1")
                    {
                        Console.WriteLine("REQUEST 1");
                        Console.WriteLine(result);
                        IQueryable<Product> products = from product in context.Products
                            select product;
                        foreach (Product product in products)
                        {
                            resultRequest = resultRequest + product.ProductId + "\n";
                        }
                    }
                    else if (result.Item1 == "2")
                    {
                        Console.WriteLine("REQUEST 2");
                        Console.WriteLine(result);
                        IQueryable<User> users = from user in context.Users
                            select user;
                        foreach (User user in users)
                        {
                            resultRequest = resultRequest + user.UserId + "\n";
                        }
                    }
                    else
                    {
                        resultRequest = "Nothong found";
                    }
                    byte[] data = Encoding.Unicode.GetBytes(resultRequest);
                    listeningSocket.SendTo(data, result.Item2); 
                }
            }
        }
        
        static async Task RequestsAsync()
        {
            await Task.Run(() => Listen());
        }
        
        static async Task BroadcastAsync()
        {
            await Task.Run(() => BroadcastMessage());
        }
        
        private static void Close()
        {
            context.Dispose();
            if (listeningSocket != null)
            {
                listeningSocket.Shutdown(SocketShutdown.Both);
                listeningSocket.Close();
                listeningSocket = null;
            }
            Console.WriteLine("Сервер остановлен!");
        }
    }
}