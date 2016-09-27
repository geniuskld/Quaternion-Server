using QuaternionProtocol.Protocol;
using QuaternionProtocol.Protocol.Binary;
using QuaternionProtocol.Serializers;
using Transport.Client;

namespace Transport.Connections
{
    public static class ClientConnectionFactory
    {
        public static ClientConnectionBase Create(IClientTransport transport)
        {
            ClientConnectionBase result = null;
            switch (transport.TransportName)
            {
                case "TCPIPV4":
                case "UDPIPV4":
                    var transp = (ClientTransportBase<byte[]>)transport;
                    var conn = new ClientConnection<byte[]>(transp, new BinaryProtocol(new byte[50]))
                    {
                        DeSerializer = new BinaryFormaterGenericSerializer<object>()
                    };
                    result = conn;
                    break;
            }

            return result;
        }
    }
}