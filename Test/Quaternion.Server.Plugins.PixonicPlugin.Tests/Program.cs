using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GameBasePlugin.Commands.TCP;
using Quaternion.Server.Plugins.PixonicPlugin.Commands.TCP;
using SocketServer;
using Transport.Connections;
using Transport.Events;
using QuaternionProtocol.Protocol.Binary;
using Transport.Server;

namespace Quaternion.Server.Plugins.PixonicPlugin.Tests
{
    class Program
    {
        public static readonly ManualResetEvent Done = new ManualResetEvent(false);
        static void Main(string[] args)
        {
            var app = new PixonicServerPlugin("PixonicApp");
            var transport = new TcpSocketServer(IPAddress.Any, 7);
            app.InitTransports(new List<IServerTransport> {transport});
            app.RegisterReceiveCommand(transport, new MessageRequest());
            app.RegisterReceiveCommand(transport, new JoinRequest());
            app.RegisterReceiveCommand(transport, new LeaveRequest());
            transport.OnConnected += (sender, eventArgs) =>
            {
                Console.WriteLine("Client connected, waiting for JoinCommand to join room");
            };
            Console.WriteLine("TCP Server is listening on port 7, use any suitable client with Quaternion protocol realized to make connection");
            Console.WriteLine("Have fun!");
            Done.WaitOne();
        }
        
    }
}
