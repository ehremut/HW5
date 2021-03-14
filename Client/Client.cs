using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        static int remotePort; // Порт для отправки сообщений
        static IPAddress ipAddress; // IP адрес сервера
        static Socket listeningSocket; // Сокет

        static void Main(string[] args)
        {
            Console.WriteLine("CLIENT");
            //Console.Write("Введите ip адрес получателя: ");
            ipAddress = IPAddress.Parse("127.0.0.1");
            //Console.Write("Введите порт для отправки сообщений: ");
            remotePort = Int32.Parse("8081");
            Console.WriteLine("Write number of request and tap Enter");
            Console.WriteLine();

            try
            {
                listeningSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp); // Создание сокета
                Task listeningTask = new Task(Listen); // Создание потока
                listeningTask.Start(); // Запуск потока

                while (true) // Отправление сообщений серверу в бесконечном цикле
                {
                    string message = Console.ReadLine();
                    var canSend = false;
                    switch (message)
                    {
                        case "1":
                            canSend = true;
                            break;
                        case "2":
                            canSend = true;
                            break;
                        default:
                            canSend = false;
                            break;
                    }

                    if (canSend)
                    {
                        byte[] data = Encoding.Unicode.GetBytes(message);
                        EndPoint remotePoint = new IPEndPoint(ipAddress, remotePort);
                        listeningSocket.SendTo(data, remotePoint);
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

        // Поток для приема подключений
        private static void Listen()
        {
            try
            {
                IPEndPoint localIP = new IPEndPoint(IPAddress.Parse("0.0.0.0"), 0); // Прослушиваем по адресу
                listeningSocket.Bind(localIP);

                while (true)
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
        // закрытие сокета
        private static void Close()
        {
            if (listeningSocket != null)
            {
                listeningSocket.Shutdown(SocketShutdown.Both);
                listeningSocket.Close();
                listeningSocket = null;
            }

            Console.WriteLine("Server stopped!");
        }
    }
}