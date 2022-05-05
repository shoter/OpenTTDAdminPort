using Microsoft.Extensions.Logging;

using OpenTTDAdminPort;
using OpenTTDAdminPort.Events;
using OpenTTDAdminPort.Game;
using OpenTTDAdminPort.Messages;
using System;
using System.Collections.Concurrent;
using System.Dynamic;
using System.Threading.Tasks;

namespace _2_ChitChat
{
    class Program
    {
        public static ConcurrentQueue<IAdminEvent> EventQueue { get; } = new ConcurrentQueue<IAdminEvent>(); 
        static async Task Main(string[] args)
        {
            var client = new AdminPortClient(AdminPortClientSettings.Default, new ServerInfo(
                "127.0.0.1", 3977, "admin_pass"), builder => {
                    builder.ClearProviders();
                    builder.AddConsole();
                    builder.SetMinimumLevel(LogLevel.Trace);
                });

            // This will be running on different thread - it is important to use multi-thread safe constructs that will enable sharing data between client and our thread.
            client.SetAdminEventHandler(ev =>
            {
                EventQueue.Enqueue(ev);
            });

            await client.Connect();

            while(true)
            {
                Console.WriteLine("Write chat message: (blank message to receive new messages from server)");
                string line = Console.ReadLine();

                while(EventQueue.TryDequeue(out IAdminEvent ev))
                {
                    if (ev is AdminChatMessageEvent chatEvent)
                    {
                        Console.WriteLine($"{chatEvent.Player.Name}: {chatEvent.Message}");
                    }
                }

                if(string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                client.SendMessage(new AdminChatMessage(NetworkAction.NETWORK_ACTION_CHAT, ChatDestination.DESTTYPE_BROADCAST, 0, line));
            }
        }
    }
}
