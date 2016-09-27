using System;
using System.Text;
using System.Threading;
using QuaternionProtocol.Protocol.Binary;
using SocketServer;
using Transport.Connections;
using Transport.Events;

namespace SocketServerLauncher
{
    class Program
    {
        public static readonly ManualResetEvent Done = new ManualResetEvent(false);
        static void Main(string[] args)
        {
            var tcpServer = new TcpSocketServer();

            tcpServer.OnConnected += TcpServerOnConnected;
            tcpServer.OnReceive += TcpServerOnReceive;
            tcpServer.Start();
            Console.WriteLine("Server started");
            Done.WaitOne();
        }

        private static void TcpServerOnReceive(object sender, ServerGenericEventArgs<byte[]> serverGenericEventArgs)
        {
            var bp = new BinaryProtocol(serverGenericEventArgs.DataReceived);
            var msg = Encoding.UTF8.GetString(bp.GetBody());
            Console.WriteLine($"{msg} received");
        }

        private static void TcpServerOnConnected(object sender, ServerGenericEventArgs<ServerConnectionBase> serverGenericEventArgs)
        {
            Console.WriteLine($"{serverGenericEventArgs.ServerConnectionBase.RemoteAddressInfo} connected");
        }
    }
}
