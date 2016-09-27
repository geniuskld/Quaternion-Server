using System;
using System.Net.Sockets;
using QuaternionProtocol.Protocol;
using QuaternionProtocol.Protocol.Binary;

namespace ClientExtensionsBase.Binary
{
    public class TcpClient: ClientSocketBase
    {
        public TcpClient() : base(new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp), new BinaryProtocol(new byte[5],"Pings"))
        {
            
        }
        public TcpClient(Socket initializedSocket, ProtocolBase<byte[]> protocol) : base(initializedSocket, protocol)
        {
        }
        
        public override string TransportName => "TcpClient";

        
    }
}