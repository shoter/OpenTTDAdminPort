using OpenTTDAdminPort;
using OpenTTDAdminPort.Events;
using OpenTTDAdminPort.Messages;
using System;
using System.Threading.Tasks;

namespace _1_PingPong
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var client = new AdminPortClient(AdminPortClientSettings.Default, new ServerInfo(
                "127.0.0.1", 3982, "admin_pass"));
            AdminPongEvent pongEvent = null;
            client.EventReceived += (_, e) =>
            {
                if (e is AdminPongEvent pe)
                    pongEvent = pe;
            };

            Console.WriteLine("Connecting ...");
            await client.Connect();
            Console.WriteLine("Sending Ping Message with argument=55 ...");
            client.SendMessage(new AdminPingMessage(55));
            Console.WriteLine("Waiting for Pong Message ...");

            while (pongEvent == null)
            {
                await Task.Delay(1);
            }

            Console.WriteLine($"Received Pong Message with argument={pongEvent.PongValue}"); ;

            Console.WriteLine("Ending connection with server");
            await client.Disconnect();
            Console.WriteLine("Press any button to quit");
            Console.ReadLine();
        }
    }
}
