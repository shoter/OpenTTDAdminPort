﻿using System;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using OpenTTDAdminPort;
using OpenTTDAdminPort.Events;
using OpenTTDAdminPort.Messages;

#pragma warning disable SA1300 // Element should begin with upper-case letter
namespace _1_PingPong
#pragma warning restore SA1300 // Element should begin with upper-case letter
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var client = new AdminPortClient(AdminPortClientSettings.Default, new ServerInfo(
                "127.0.0.1", 3977, "admin_pass"), builder =>
                {
                    builder.ClearProviders();
                    builder.AddConsole();
                    builder.SetMinimumLevel(LogLevel.Trace);
                });
            AdminPongEvent pongEvent = null;
            client.SetAdminEventHandler(ev =>
            {
                if (ev is AdminPongEvent pe)
                {
                     pongEvent = pe;
                }
            });

            Console.WriteLine("Connecting ...");
            await client.Connect();
            Console.WriteLine("Sending Ping Message with argument=55 ...");
            client.SendMessage(new AdminPingMessage(55));
            Console.WriteLine("Waiting for Pong Message ...");

            while (pongEvent == null)
            {
                await Task.Delay(1);
            }

            Console.WriteLine($"Received Pong Message with argument={pongEvent.PongValue}");

            // With Wait For event
            var pong = await client.WaitForEvent<AdminPongEvent>(new AdminPingMessage(2137));
            Console.WriteLine($"I've sent ping 2137 and received {pong.PongValue}");
            Console.WriteLine("Ending connection with server");

            // await client.Disconnect();
            // Console.WriteLine("Press any button to quit");
            Console.ReadLine();
        }
    }
}
