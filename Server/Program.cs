using System.IO;

namespace Server
{
    using System;
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
                var kvStore = new KeyValueStore();
                string logsPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\..\Logs"));
                Catchup(logsPath, kvStore);
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
                        var response = "";

                        response = kvStore.Execute(data);

                        var msg = Encoding.ASCII.GetBytes(response);
                        handler.Send(msg);
                        if (data.Contains("<EOF>"))
                        {
                            Console.WriteLine("break condition hit");
                            break;
                        }

                        using (var outputFile = new StreamWriter(Path.Combine(logsPath, "commands.txt"), true))
                        {
                            outputFile.WriteLine(data);
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

        private static void Catchup(string basePath, KeyValueStore kvStore)
        {
            var file  = Path.Combine(basePath, "commands.txt");
            if (!File.Exists(file))
            {
                return;
            }

            string[] operations = File.ReadAllLines(file);
            foreach (var operation in operations)
            {
                kvStore.Execute(operation);
            }

        }
    }
}