using System;

namespace Server
{
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;

    class Program
    {
        static int Main(string[] args)
        {
            Server.StartListening();
            return 0;
        }
    }

    public class Server
    {
        public static string data = null;
        public static Dictionary<string, string> state = new Dictionary<string, string>();

        public static void StartListening()
        {
            var bytes = new byte[1024];

            var ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            var ipAddress = ipHostInfo.AddressList[0];
            var localEndPoint = new IPEndPoint(ipAddress, 11000);

            var listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(10);

                while (true)
                {
                    Console.WriteLine("Waiting for a connection ....");
                    var handler = listener.Accept();
                    data = null;

                    while (true)
                    {
                        var bytesRec = handler.Receive(bytes);
                        data = Encoding.ASCII.GetString(bytes, 0, bytesRec);
                        Console.WriteLine($"Text received : {data}");

                        var dataArr = data.Split(" ");
                        var command = dataArr[0];
                        var key = dataArr[1];
                        var value = dataArr[2];
                        
                        var msg = Encoding.ASCII.GetBytes(data);
                        handler.Send(msg);
                        if (data.Contains("<EOF>"))
                        {
                            Console.WriteLine("break condition hit");
                            break;
                        }
                    }
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                    break;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            
            Console.WriteLine("\nPress ENTER to continue ....");
            Console.Read();

        }
    }
}