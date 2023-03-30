using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    class Program
    {
        // Task method
        static async Task Runserver(int port)
        {
            // socket endpoint
            var ipep = new IPEndPoint(IPAddress.Any, 10000);

            // socket instance
            using (Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                // Endpoint setting
                server.Bind(ipep);
                // client socket waiting buffer
                server.Listen(20);
                Console.WriteLine($"Server Start... Listen port {ipep.Port}");

                // server accept를 Task 로 병렬처리 (비동기 처리)
                var task = new Task(() =>
                {
                    while (true)
                    {
                        var client = server.Accept();

                        new Task(() =>
                        {
                            var ip = client.RemoteEndPoint as IPEndPoint;
                            Console.WriteLine($"Client : (From: {ip.Address.ToString()}:{ip.Port}, Connection time : {DateTime.Now}");
                            client.Send(Encoding.ASCII.GetBytes("Welcome server!\r\n>"));

                            var sb = new StringBuilder();
                            using (client)
                            {
                                while (true)
                                {
                                    var binary = new Byte[1024];

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
                                        sb.Length = 0;

                                        var sendMsg = Encoding.ASCII.GetBytes("ECHO : " + data + "\r\n");
                                        client.Send(sendMsg);
                                    }
                                }

                                Console.WriteLine($"Disconnected : (From: {ip.Address.ToString()}:{ip.Port}, Connection time : {DateTime.Now}");
                            }
                        }).Start();
                    }
                });

                task.Start();

                await task;
            }


        }
        static void Main(string[] args)
        {
            Runserver(10000).Wait();

            Console.WriteLine("Press Any Key....");
            Console.ReadLine();
        }
    }
}