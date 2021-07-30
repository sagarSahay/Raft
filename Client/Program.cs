using System;

namespace Client
{
    using System.Net;
    using System.Net.Sockets;
    using System.Text;

    class Program
    {
        static int Main(string[] args)
        {
            Client.StartClient();
            return 0;
        }
    }

    public class Client
    {
        public static void StartClient()
        {
            var bytes = new byte[1024];

            try
            {
                var ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
                var ipAddress = ipHostInfo.AddressList[0];
                var remoteEP = new IPEndPoint(ipAddress, 11000);

                var sender = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                try
                {
                    sender.Connect(remoteEP);
                    Console.WriteLine($"Socket connected to {sender.RemoteEndPoint.ToString()}");

                    while (true)
                    {
                        var msg = Console.ReadLine();
                        var bytesSent = sender.Send(Encoding.ASCII.GetBytes(msg));
                        var bytesRec = sender.Receive(bytes);
                        Console.WriteLine($"Echoed test = {Encoding.ASCII.GetString(bytes, 0, bytesRec)}");
                        if (msg.Contains("<EOF>", StringComparison.OrdinalIgnoreCase))
                        {
                            break;
                        }
                    }
                    sender.Shutdown(SocketShutdown.Both);
                    sender.Close();
                }
                catch (ArgumentNullException ane)
                {
                    Console.WriteLine($"Argument null exception {ane.ToString()}");
                }
                catch (SocketException se)
                {
                    Console.WriteLine($"SocketException {se.ToString()}");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Unexpected exception {e.ToString()}");
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}