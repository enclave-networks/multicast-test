using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace multicast_test
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Simple Multicast Testing Tool for Windows");
            Console.WriteLine("=========================================\n");
            Console.WriteLine("Interface list:\n");
            Console.WriteLine($"    0: {"0.0.0.0",-40} Any");

            AddressDictionary.Add(0, IPAddress.Any);

            // enumerate available interfaces
            var i = 1;
            foreach (var iface in NetworkInterface.GetAllNetworkInterfaces().Where(n => n.SupportsMulticast && n.OperationalStatus == OperationalStatus.Up))
            {
                foreach (var ip in iface.GetIPProperties().UnicastAddresses)
                {
                    Console.WriteLine($"   {i,2}: {ip.Address,-40} {iface.Name} ({iface.Description})");
                    AddressDictionary.Add(i, ip.Address);
                    i++;
                }
            }

            // prompt user to select an interface
            var selection = -1;
            while (selection == -1)
            {
                try
                {
                    Console.Write("\nSelect interface: ");
                    selection = Convert.ToInt32(Console.ReadLine());

                    // prevent user selecting a number beyond the range of display interfaces
                    if (selection > i - 1) selection = -1;

                    // select binding address from the interface dictionary
                    _bindingAddress = AddressDictionary[selection];
                }
                catch (Exception)
                {
                    // swallow
                }
            }

            // reset selection variable
            selection = -1;

            Console.WriteLine();
            Console.WriteLine("    1: Multicast sender (transmit data)");
            Console.WriteLine("    2: Multicast subscriber (listen socket, receive data)");
            Console.WriteLine("    9: Exit");
            Console.WriteLine();

            // prompt to select an action
            while (selection == -1)
            {
                try
                {
                    Console.Write("Select action: ");
                    selection = Convert.ToInt32(Console.ReadLine());

                    switch (selection)
                    {
                        case 9:
                        {
                            return;
                        }
                        case 1:
                        {
                            try
                            {
                                using (var client = new UdpClient())
                                {
                                    client.JoinMulticastGroup(MulticastAddress, _bindingAddress);

                                    Console.WriteLine($"\nJoined multicast group {MulticastAddress}");
                                    Console.WriteLine();

                                    ulong n = 0;
                                    while (true)
                                    {
                                        SendMessage(client, $"Simple Multicast Testing Tool for Windows @ {DateTime.Now.ToLongTimeString()}");

                                        Console.WriteLine($"Message {n,-5} sent to {MulticastAddress}:{MulticastPort}  TTL: {client.Ttl}");
                                            Thread.Sleep(1000);
                                        n++;
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                            }

                            break;
                        }
                        case 2:
                        {
                            Listen();
                            break;
                        }
                        default:
                        {
                            selection = -1;
                            break;
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        public static void Listen()
        {
            _udpClient = new UdpClient(MulticastPort);
            _udpClient.JoinMulticastGroup(MulticastAddress, _bindingAddress);

            var receiveThread = new Thread(Receive);
            receiveThread.Start();

            Console.WriteLine($"\nBound udp listener on {_bindingAddress}. Joined multicast group {MulticastAddress}. Waiting to receive data...\n");
        }

        public static void Receive()
        {
            while (true)
            {
                var ipEndPoint = new IPEndPoint(IPAddress.Any, 0);

                var data = _udpClient.Receive(ref ipEndPoint);
                var message = Encoding.Default.GetString(data);

                Console.WriteLine($"Received {message.Length} bytes from {ipEndPoint}: \"{message}\"");
            }
        }


        public static void SendMessage(UdpClient client, string message)
        {
            var data = Encoding.Default.GetBytes(message);

            var ipEndPoint = new IPEndPoint(MulticastAddress, MulticastPort);

            client.Send(data, data.Length, ipEndPoint);
        }

        private static IPAddress _bindingAddress;

        private static readonly IPAddress MulticastAddress = IPAddress.Parse("239.0.1.2");

        private const int MulticastPort = 20480;

        private static readonly Dictionary<int, IPAddress> AddressDictionary = new Dictionary<int, IPAddress>();

        private static UdpClient _udpClient;
    }
}