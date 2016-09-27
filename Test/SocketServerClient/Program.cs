using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using QuaternionProtocol.Protocol.Binary;
using QuaternionProtocol.Serializers;

namespace SocketServerClient
{
    class Program
    {
        public static ManualResetEvent Done = new ManualResetEvent(false);
        private static ClientExtensionsBase.Binary.TcpClient _client;
        static void Main(string[] args)
        {
            _client = new ClientExtensionsBase.Binary.TcpClient();
            _client.Connect("192.168.10.162:1000");
            _client.OnConnected += ClientOnOnConnected;
            Done.WaitOne();
        }

        private static void ClientOnOnConnected(object sender, EventArgs eventArgs)
        {
            var str = Encoding.UTF8.GetBytes("Hello server");
            var proto = new BinaryProtocol(str,"Nop");
            _client.SendData(proto);
            Done.WaitOne(1000);
            Done.Set();
        }
    }
}
