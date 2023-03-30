using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            var ipep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 10000);

            using (Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                client.Connect(ipep);

                new Task(() =>
                {
                    try
                    {
                        while (true)
                        {
                            var binary = new Byte[1024];
                            client.Receive(binary);
                            var data = Encoding.ASCII.GetString(binary).Trim('\0');
                            if (String.IsNullOrEmpty(data))
                                continue;

                            Console.Write(data);
                        }
                    }
                    catch (SocketException e)
                    {
                        Console.WriteLine($"Exception : {e}");
                    }
                }).Start();

                while (true)
                {
                    var msg = Console.ReadLine();
                    client.Send(Encoding.ASCII.GetBytes(msg + "\r\n"));

                    if ("EXIT".Equals(msg, StringComparison.OrdinalIgnoreCase))
                        break;

                }

                Console.WriteLine("Disconnected");
            }
            Console.WriteLine("Press Any Key...");
            Console.ReadLine();
        }
    }
}