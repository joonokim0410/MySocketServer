using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Example
{
    class program
    {
        static void Main(string[] args)
        {
            var ipep = new IPEndPoint(IPAddress.Any, 10000);

            using (Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                server.Bind(ipep);
                server.Listen(20);
                Console.WriteLine($"Server Start... Listen port {ipep.Port}");

                using (var client = server.Accept())
                {
                    var ip = client.RemoteEndPoint as IPEndPoint;
                    Console.WriteLine($"Client : (From: {ip.Address.ToString()}:{ip.Port}, Connection time : {DateTime.Now}");
                    client.Send(Encoding.ASCII.GetBytes("Welcome server!\r\n>"));

                    var sb = new StringBuilder();
                    var binary = new Byte[1024];
                    while (true)
                    {
                        client.Receive(binary);
                        var data = Encoding.ASCII.GetString(binary);
                        sb.Append(data.Trim('\0'));
                        if (sb.Length > 2 && sb[sb.Length - 2] == '\r' && sb[sb.Length - 1] == '\n')
                        {
                            data = sb.ToString().Replace("\n", "").Replace("\r", "");
                            if (String.IsNullOrWhiteSpace(data))
                            {
                                continue;
                            }
                            if ("EXIT".Equals(data, StringComparison.OrdinalIgnoreCase))
                            {
                                break;
                            }
                            Console.WriteLine("Message = " + data);
                            Console.WriteLine("Byte= " + binary);
                            sb.Length = 0;

                            var sendMsg = Encoding.ASCII.GetBytes("ECHO : " + data + "\r\n");
                            client.Send(sendMsg);
                        }
                    }

                    Console.WriteLine($"Disconnected : (From: {ip.Address.ToString()}:{ip.Port}, Connection time : {DateTime.Now}");
                }
            }

            Console.WriteLine("Press Any Key....");
            Console.ReadLine();
        }
    }
}